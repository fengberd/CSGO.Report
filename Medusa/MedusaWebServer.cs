using System.IO;
using System.Net;
using System.Collections.Generic;

using BakaServer;
using Newtonsoft.Json;

namespace Medusa
{
    public class MedusaWebServer : Server
    {
        public static List<Dictionary<string,string>> report_log = new List<Dictionary<string,string>>();

        public static void addReportLog(Dictionary<string,string> data)
        {
            lock(report_log)
            {
                report_log.Add(data);
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
            if(Request.HttpMethod != "POST")
            {
                switch(Request.Url.AbsolutePath)
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
                case "/submit":
                case "/login/failed":
                    SendError(context,StatusCode.E_405);
                    break;
                default:
                    SendError(context,StatusCode.E_404);
                    break;
                }
            }
            else
            {
                var data = ReadData(context);
                if(data != null)
                {
                    if(Request.Url.AbsolutePath == "/" || Request.Url.AbsolutePath == "/favicon.ico")
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
                        switch(Request.Url.AbsolutePath)
                        {
                        case "/log":
                            Program.ProcessLogs(context,data);
                            break;
                        case "/status":
                            Program.ProcessStatus(context,data);
                            break;
                        case "/submit":
                            Program.ProcessSubmit(context,data);
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
                        case "/login_failed":
                            Program.ProcessLoginFailed(context,data);
                            break;
                        default:
                            SendError(context,StatusCode.E_404);
                            break;
                        }
                    }
                }
            }
        }
    }
}
