using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;

using SteamKit2;
using BakaServer;
using Newtonsoft.Json;

using Medusa.utils;
using Medusa.utils.actions;
using static Medusa.utils.actions.ReportInfo;
using static Medusa.utils.actions.CommendInfo;

namespace Medusa
{
    public class Program
    {
        public static readonly int start_time = Utils.Time();

        public static Config config = new Config("config.cfg");

        public static string AccessKey = "";

        private static TimeSpan OnlineStart, OnlineEnd;
        private static MailClient mailClient = null;
        private static AccountManager accountManager = null;
        private static MedusaWebServer web_server = null;

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (sender,ev) =>
            {
                try
                {
                    Logger.Fatal(ev.ExceptionObject);
                }
                catch { }
            };
            if(Environment.OSVersion.Platform == PlatformID.Unix && File.Exists("System.Net.Http.dll"))
            {
                File.Delete("System.Net.Http.dll");
            }
            Logger.Init("medusa");
            Logger.Info("Medusa Report Server " + Assembly.GetExecutingAssembly().GetName().Version + "(" + Server.ServerVersionName + ")");
            // Logger.Info("Build date: " + Logger.GetBuildTime());
            Logger.EnableDebug = config.GetBool("LogDebug",false);
            Logger.EnableColor = config.GetBool("LogColor",true);
            if(config.GetBool("SteamKitDebug",false))
            {
                DebugLog.Enabled = true;
                DebugLog.AddListener(new DebugListener());
            }
            if(config.GetBool("MailClientEnabled",false))
            {
                mailClient = new MailClient();
            }
            if(config["AccessKey",""] == "")
            {
                Logger.Warning("Access key is empty,this means nobody can access the webapi.");
            }
            else
            {
                AccessKey = config["AccessKey"];
                Logger.Info("Access key read successfully(Length:" + AccessKey.Length + ").");
            }
            if(config.GetInt("ServerPort",23336) > 0)
            {
                Logger.Info("Starting Medusa Web Server...");
                web_server = new MedusaWebServer((short)config.GetInt("ServerPort"),config["ServerAddress","localhost"]);
                web_server.Start();
            }
            if(!File.Exists(config["AccountsFile","accounts.json"]))
            {
                File.WriteAllText(config["AccountsFile"],"[]");
            }
            if(!config.GetBool("GameAlwaysOnline",false))
            {
                try
                {
                    var parse = config["GameOnlineStart","9:00:00"].Split(':');
                    OnlineStart = new TimeSpan(int.Parse(parse[0]),parse.Length >= 2 ? int.Parse(parse[1]) : 0,parse.Length >= 3 ? int.Parse(parse[2]) : 0);
                    parse = config["GameOnlineEnd","22:00:00"].Split(':');
                    OnlineEnd = new TimeSpan(int.Parse(parse[0]),parse.Length >= 2 ? int.Parse(parse[1]) : 0,parse.Length >= 3 ? int.Parse(parse[2]) : 0);
                }
                catch
                {
                    Logger.Error("Can't parse game online time range.");
                    OnlineStart = new TimeSpan(9,0,0);
                    OnlineEnd = new TimeSpan(22,0,0);
                }
            }
            accountManager = new AccountManager(File.ReadAllText(config["AccountsFile"]));
            Properties.Resources.Whitelist.Split('\n').ToList().ForEach((id) =>
            {
                if(!id.StartsWith("#"))
                {
                    try
                    {
                        AccountManager.Whitelist.Add(SteamIdUtil.Parse(id.Split(',')[0]).ConvertToUInt64());
                    }
                    catch { }
                }
            });
            Logger.Info("Starting login progress...");
            accountManager.DelayedConnectAll();
            File.WriteAllText("server.pid",Process.GetCurrentProcess().Id.ToString());
            Logger.Info("Enter Main Loop...");
            long currentTick = 0;
            while(true)
            {
                Thread.Sleep(50);
                accountManager.Tick(currentTick++);
            }
        }

        public static int GetUptime()
        {
            return Utils.Time() - start_time;
        }

        public static bool IsOnlineTimeRange()
        {
            if(config.GetBool("GameAlwaysOnline"))
            {
                return true;
            }
            var now = DateTime.Now.TimeOfDay;
            return OnlineStart < now && now < OnlineEnd;
        }

        public static void Pause()
        {
            while(true)
            {
                Thread.Sleep(50);
            }
        }

        public static bool MailCodeRecieved(string username,string code)
        {
            var query = accountManager.Accounts.Values.Where((a) => a.WaitingForCode && a.Username.ToLower() == username);
            if(query.Count() == 0)
            {
                Logger.Warning("[Mail] Account " + username + " isn't waiting for code.");
                return false;
            }
            var account = query.First();
            Logger.Info("[Mail] Recieved code for account " + account.Username);
            account.AuthCode = code;
            account.Connect();
            return true;
        }

        #region Request Processing Functions

        public static bool CheckAccessKey(string data)
        {
            return AccessKey != "" && AccessKey == data;
        }

        public static void ProcessStatus(HttpListenerContext context,IDictionary<string,string> data)
        {
            int online = accountManager.Accounts.Values.Where((a) => a.LoggedIn).Count(), total = accountManager.Accounts.Count;
            Server.SendResult(context,Body: JsonConvert.SerializeObject(new Dictionary<string,object>()
            {
                { "success",true },
                { "accounts",new Dictionary<string,object>()
                    {
                        { "online",online },
                        { "total",total },
                        { "available_keys",accountManager.Accounts.Keys.Where((k)=>accountManager.Accounts[k].LoggedIn) }
                    }
                },
                { "uptime",GetUptime() }
            }));
        }

        public static void ProcessLogs(HttpListenerContext context,IDictionary<string,string> data)
        {
            IDictionary<string,object> result = new Dictionary<string,object>()
            {
                { "success",true }
            };
            if(data.ContainsKey("remove") && int.TryParse(data["remove"],out int remove))
            {
                lock(MedusaWebServer.log)
                {
                    MedusaWebServer.log.RemoveRange(0,remove);
                    result["message"] = "Removed " + remove + " logs.";
                }
            }
            else
            {
                lock(MedusaWebServer.log)
                {
                    result["log"] = MedusaWebServer.log;
                    result["message"] = "";
                }
            }
            Server.SendResult(context,Body: JsonConvert.SerializeObject(result));
        }

        public static void ProcessSubmitReport(HttpListenerContext context,IDictionary<string,string> data)
        {
            IDictionary<string,object> result = new Dictionary<string,object>()
            {
                { "success",false }
            };
            if(!data.ContainsKey("id") || !data.ContainsKey("match") || !data.ContainsKey("account") || !data.ContainsKey("flags") || !int.TryParse(data["flags"],out int flags))
            {
                result["message"] = "参数错误";
            }
            else if(!data["account"].Split(',').All(accountManager.Accounts.ContainsKey))
            {
                result["message"] = "账户不存在";
            }
            else
            {
                SteamID steamID = SteamIdUtil.Parse(data["id"]);
                ulong matchID = 0;
                if(data["match"].StartsWith("steam://"))
                {
                    matchID = ShareCode.Decode(data["match"].Substring(61)).MatchId;
                }
                else if(data["match"].StartsWith("CSGO-"))
                {
                    matchID = ShareCode.Decode(data["match"]).MatchId;
                }
                else
                {
                    ulong.TryParse(data["match"],out matchID);
                }
                if(steamID != null)
                {
                    if(matchID == 0)
                    {
                        result["message"] = "请提供有效的Match ID.";
                    }
                    else if(AccountManager.IsWhitelisted(steamID))
                    {
                        result["message"] = "举报目标在我们的白名单中,这通常说明举报目标不会作弊或是我们的机器人账号之一.";
                    }
                    else
                    {
                        var info = new ReportInfo()
                        {
                            SteamID = steamID,
                            MatchID = matchID,
                            AbusiveText = (flags & FLAG_ABUSIVE_TEXT) == FLAG_ABUSIVE_TEXT,
                            AbusiveVoice = (flags & FLAG_ABUSIVE_VOICE) == FLAG_ABUSIVE_VOICE,
                            Griefing = (flags & FLAG_GRIEFING) == FLAG_GRIEFING,
                            AimHacking = (flags & FLAG_AIM_HACKING) == FLAG_AIM_HACKING,
                            WallHacking = (flags & FLAG_WALL_HACKING) == FLAG_WALL_HACKING,
                            OtherHacking = (flags & FLAG_OTHER_HACKING) == FLAG_OTHER_HACKING
                        };
                        var submitted = data["account"].Split(',');
                        var accounts = accountManager.Accounts.Where((kv) => submitted.Contains(kv.Key));
                        foreach(var kv in accounts)
                        {
                            kv.Value.QueueAction(info);
                        }
                        result["matchid"] = matchID;
                        result["success"] = true;
                        result["message"] = "Abusive Text:" + info.AbusiveText + "<br />" +
                            "Abusive Voice:" + info.AbusiveVoice + "<br />" +
                            "Griefing:" + info.Griefing + "<br />" +
                            "Aim Hacking:" + info.AimHacking + "<br />" +
                            "Wall Hacking:" + info.WallHacking + "<br />" +
                            "Other Hacking:" + info.OtherHacking;
                    }
                }
                else
                {
                    result["message"] = "无法解析Steam ID,请提供有效的SteamID,SteamID3或SteamID64.";
                }
            }
            Server.SendResult(context,Body: JsonConvert.SerializeObject(result));
        }

        public static void ProcessSubmitCommend(HttpListenerContext context,IDictionary<string,string> data)
        {
            IDictionary<string,object> result = new Dictionary<string,object>()
            {
                { "success",false }
            };
            if(!data.ContainsKey("id") || !data.ContainsKey("account") || !data.ContainsKey("flags") || !int.TryParse(data["flags"],out int flags))
            {
                result["message"] = "参数错误";
            }
            else if(!data["account"].Split(',').All(accountManager.Accounts.ContainsKey))
            {
                result["message"] = "账户不存在";
            }
            else
            {
                SteamID steamID = SteamIdUtil.Parse(data["id"]);
                if(steamID != null)
                {
                    var info = new CommendInfo()
                    {
                        SteamID = steamID,
                        Flags = flags,
                        Friendly = (flags & FLAG_FRIENDLY) == FLAG_FRIENDLY,
                        GoodTeacher = (flags & FLAG_GOOD_TEACHER) == FLAG_GOOD_TEACHER,
                        GoodLeader = (flags & FLAG_GOOD_LEADER) == FLAG_GOOD_LEADER
                    };
                    var submitted = data["account"].Split(',');
                    var accounts = accountManager.Accounts.Where((kv) => submitted.Contains(kv.Key));
                    foreach(var kv in accounts)
                    {
                        kv.Value.QueueAction(info);
                    }
                    result["success"] = true;
                    result["message"] = "Friendly:" + info.Friendly + "<br />" +
                        "Good Teacher:" + info.GoodTeacher + "<br />" +
                        "Good Leader:" + info.GoodLeader;
                }
                else
                {
                    result["message"] = "无法解析Steam ID,请提供有效的SteamID,SteamID3或SteamID64.";
                }
            }
            Server.SendResult(context,Body: JsonConvert.SerializeObject(result));
        }

        public static void ProcessSubmitLiveGameInfo(HttpListenerContext context,IDictionary<string,string> data)
        {
            IDictionary<string,object> result = new Dictionary<string,object>()
            {
                { "success",false }
            };
            if(!data.ContainsKey("id") || !data.ContainsKey("account"))
            {
                result["message"] = "参数错误";
            }
            else if(!accountManager.Accounts.ContainsKey(data["account"]))
            {
                result["message"] = "账户不存在";
            }
            else
            {
                SteamID steamID = SteamIdUtil.Parse(data["id"]);
                if(steamID != null)
                {
                    accountManager.Accounts[data["account"]].QueueAction(new GetLiveGameInfo()
                    {
                        SteamID = steamID
                    });
                    result["success"] = true;
                }
                else
                {
                    result["message"] = "无法解析Steam ID,请提供有效的SteamID,SteamID3或SteamID64.";
                }
            }
            Server.SendResult(context,Body: JsonConvert.SerializeObject(result));
        }

        public static void ProcessLoginFailed(HttpListenerContext context,IDictionary<string,string> data)
        {
            var target = accountManager.Accounts.Values.Where((a) => !a.Disabled && !a.LoggedIn && !a.Connected && !AccountManager.DelayedLoginQueue.Contains(a) && a.DelayedActionsEmpty).ToList();
            target.ForEach(AccountManager.DelayedLoginQueue.Enqueue);
            Server.SendResult(context,Body: JsonConvert.SerializeObject(new Dictionary<string,object>()
            {
                { "success",true },
                { "message","Added " + target.Count + " accounts into login queue." }
            }));
        }

        #endregion
    }
}
