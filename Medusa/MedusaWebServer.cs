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
                case "/status":
                    Dictionary<string,object> result = new Dictionary<string,object>
                    {
                        { "success",true },
                        { "message","" },
                        { "time",Utils.Time() },
                    };
                    SendResult(context,Body: JsonConvert.SerializeObject(result));
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
                case "/groups":
                case "/submit":
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
                    switch(Request.Url.AbsolutePath)
                    {
                    case "/log":
                        Program.ProcessLogs(context,data);
                        break;
                    case "/groups":
                        Program.ProcessGroups(context,data);
                        break;
                    case "/submit":
                        Program.ProcessSubmit(context,data);
                        break;
                    case "/":
                    case "/status":
                    case "/favicon.ico":
                        SendError(context,StatusCode.E_405);
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
