using System.IO;
using System.Net;
using System.Collections.Generic;

using BakaServer;
using Newtonsoft.Json;

namespace Medusa
{
    public class MedusaWebServer : Server
    {
        public static List<Dictionary<string,object>> log = new List<Dictionary<string,object>>();

        public static void addLog(Dictionary<string,object> data)
        {
            lock(log)
            {
                log.Add(data);
            }
        }

        public MedusaWebServer(short Port = 7655,string Address = "localhost") : base(Port,Address)
        {

        }

        public override void Start()
        {
            base.Start();
            Logger.Info("Medusa Web Server started on port " + Port + " ...");
        }

        protected override void ProcessRequest(HttpListenerContext context)
        {
            var Request = context.Request;
            var url = Request.Url.AbsolutePath;
            if(Request.HttpMethod != "POST")
            {
                switch(url)
                {
                case "/":
                    SendError(context,"520 I Love You");
                    break;
                case "/favicon.ico":
                    if(File.Exists("icon.png"))
                    {
                        context.Response.ContentType = "image/png";
                        SendResult(context,Body: File.ReadAllBytes("icon.png"));
                    }
                    else
                    {
                        SendError(context,StatusCode.E_404);
                    }
                    break;
                case "/log":
                case "/status":
                    SendError(context,StatusCode.E_405);
                    break;
                default:
                    SendError(context,url.StartsWith("/submit/") || url.StartsWith("/login/") ? StatusCode.E_405 : StatusCode.E_404);
                    break;
                }
            }
            else
            {
                var data = ReadData(context);
                if(data != null)
                {
                    if(url == "/" || url == "/favicon.ico")
                    {
                        SendError(context,StatusCode.E_405);
                    }
                    else if(!data.ContainsKey("key") || !Program.CheckAccessKey(data["key"]))
                    {
                        SendResult(context,Body: JsonConvert.SerializeObject(new Dictionary<string,object>()
                        {
                            { "success",false },
                            { "message","Access Denied." },
                        }));
                    }
                    else
                    {
                        switch(url)
                        {
                        case "/log":
                            Program.ProcessLogs(context,data);
                            break;
                        case "/status":
                            Program.ProcessStatus(context,data);
                            break;
                        case "/submit/report":
                            Program.ProcessSubmitReport(context,data);
                            break;
                        case "/submit/commend":
                            Program.ProcessSubmitCommend(context,data);
                            break;
                        case "/submit/livegameinfo":
                            Program.ProcessSubmitLiveGameInfo(context,data);
                            break;
                        case "/login/failed":
                            Program.ProcessLoginFailed(context,data);
                            break;
                        case "/debug_log":
                            // TODO: We may need a shell
                            Logger.EnableDebug = !Logger.EnableDebug;
                            SendResult(context,Body: JsonConvert.SerializeObject(new Dictionary<string,object>()
                            {
                                { "success",true },
                                { "message","Debug Log:" + Logger.EnableDebug }
                            }));
                            break;
                        default:
                            if(url.StartsWith("/login/username/"))
                            {
                                // TODO: Login by username
                                break;
                            }
                            SendError(context,StatusCode.E_404);
                            break;
                        }
                    }
                }
            }
        }
    }
}
