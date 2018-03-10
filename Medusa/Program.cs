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
using static Medusa.utils.ReportInfo;

namespace Medusa
{
    public class Program
    {
        private static readonly int start_time = Utils.Time();

        private static string AccessKey = "";

        private static Config config = new Config("config.cfg");
        private static AccountManager accountManager = null;
        private static MedusaWebServer web_server = null;

        static void Main(string[] args)
        {
            Logger.Init("medusa");
            Logger.Info("Medusa Report Server " + Assembly.GetExecutingAssembly().GetName().Version + "(" + Server.ServerVersionName + ")");
            Logger.Info("Build date: " + Logger.GetBuildTime());
#if DEBUG
            Logger.Warning("Debug version is running...");
#endif
            if(!ThreadPool.SetMinThreads(50,50) || !ThreadPool.SetMaxThreads(100,100))
            {
                ThreadPool.GetMaxThreads(out int workerThreads,out int completionPortThreads);
                Logger.Warning("Can't set ThreadPool settings.Worker:" + workerThreads + ",Port:" + completionPortThreads);
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

        public static void Pause()
        {
            while(true)
            {
                Thread.Sleep(50);
            }
        }

        #region Request Processing Functions

        public static bool CheckAccessKey(string data)
        {
            return AccessKey != "" && AccessKey == data;
        }

        public static void ProcessStatus(HttpListenerContext context,IDictionary<string,string> data)
        {
            IDictionary<string,object> result = new Dictionary<string,object>();
            if(!data.ContainsKey("key"))
            {
                result.Add("success",false);
                result.Add("message","参数错误");
            }
            else if(!CheckAccessKey(data["key"]))
            {
                result.Add("success",false);
                result.Add("message","Access Denied.");
            }
            else
            {
                result.Add("success",true);
                int online = 0;
                var groups = new Dictionary<int,int>();
                foreach(var kv in accountManager.AccountGroups)
                {
                    online += kv.Value.AvailableCount;
                    groups.Add(kv.Key,kv.Value.AvailableCount);
                }
                result.Add("groups",groups);
                result.Add("online",online);
                result.Add("uptime",GetUptime());
            }
            Server.SendResult(context,Body: JsonConvert.SerializeObject(result));
        }

        public static void ProcessLogs(HttpListenerContext context,IDictionary<string,string> data)
        {
            IDictionary<string,object> result = new Dictionary<string,object>();
            if(!data.ContainsKey("key"))
            {
                result.Add("success",false);
                result.Add("message","参数错误");
            }
            else if(!CheckAccessKey(data["key"]))
            {
                result.Add("success",false);
                result.Add("message","Access Denied.");
            }
            else
            {
                result.Add("success",true);
                if(data.ContainsKey("remove") && int.TryParse(data["remove"],out int remove))
                {
                    lock(MedusaWebServer.report_log)
                    {
                        MedusaWebServer.report_log.RemoveRange(0,remove);
                        result.Add("message","Removed " + remove + " logs.");
                    }
                }
                else
                {
                    lock(MedusaWebServer.report_log)
                    {
                        result.Add("log",MedusaWebServer.report_log);
                        result.Add("message","");
                    }
                }
            }
            Server.SendResult(context,Body: JsonConvert.SerializeObject(result));
        }
        
        public static void ProcessSubmit(HttpListenerContext context,IDictionary<string,string> data)
        {
            IDictionary<string,object> result = new Dictionary<string,object>();
            if(!data.ContainsKey("key") || !data.ContainsKey("id") || !data.ContainsKey("match") || !data.ContainsKey("group") || !data.ContainsKey("flags"))
            {
                result.Add("success",false);
                result.Add("message","参数错误");
            }
            else if(!CheckAccessKey(data["key"]))
            {
                result.Add("success",false);
                result.Add("message","Access Denied.");
            }
            else if(!int.TryParse(data["group"],out int group) || !int.TryParse(data["flags"],out int flags))
            {
                result.Add("success",false);
                result.Add("message","参数错误");
            }
            else if(group == -1 || !accountManager.AccountGroups.ContainsKey(group))
            {
                result.Add("success",false);
                result.Add("message","账户组不存在");
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
                        result.Add("success",false);
                        result.Add("message","请提供有效的Match ID.");
                    }
                    else if(AccountManager.IsWhitelisted(steamID))
                    {
                        result.Add("success",false);
                        result.Add("message","举报目标在我们的白名单中,这通常说明举报目标不会作弊或是我们的机器人账号之一.");
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
                        var accounts = accountManager.AccountGroups[group];
                        List<string> accounts_name = new List<string>();
                        foreach(var account in accounts)
                        {
                            account.reportQueue.Enqueue(info);
                            accounts_name.Add(account.Username);
                        }
                        result.Add("success",true);
                        result.Add("accounts",accounts_name);
                        result.Add("message","Abusive Text:" + info.AbusiveText + "<br />" +
                            "Abusive Voice:" + info.AbusiveVoice + "<br />" +
                            "Griefing:" + info.Griefing + "<br />" +
                            "Aim Hacking:" + info.AimHacking + "<br />" +
                            "Wall Hacking:" + info.WallHacking + "<br />" +
                            "Other Hacking:" + info.OtherHacking);
                    }
                }
                else
                {
                    result.Add("success",false);
                    result.Add("message","无法解析Steam ID,请提供有效的SteamID,SteamID3或SteamID64.");
                }
            }
            Server.SendResult(context,Body: JsonConvert.SerializeObject(result));
        }

        #endregion
    }
}
