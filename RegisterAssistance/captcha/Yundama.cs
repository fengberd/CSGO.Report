using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;

using RestSharp;
using BakaServer;

namespace RegisterAssistance.captcha
{
    public class Yundama : ICaptchaProcessor
    {
        public const string API_BASE = "http://api.yundama.com/api.php";

        public Config config = new Config("config_yundama.ini");
        public RestClient CLIENT = new RestClient(API_BASE);

        public IDictionary<string,object> request(string method,Action<IRestRequest> paramProcessor)
        {
            var request = new RestRequest(Method.POST);
            request.AddParameter("method",method);
            paramProcessor.Invoke(request);
            try
            {
                return CLIENT.Post<Dictionary<string,object>>(request).Data;
            }
            catch
            {
                return null;
            }
        }

        public string submitImage(Image data)
        {
            using(var ms = new MemoryStream())
            {
                data.Save(ms,ImageFormat.Jpeg);
                var result = request("upload",(request) =>
                {
                    addBasicParams(request);
                    request.AddParameter("timeout",config["Timeout","20"]);
                    request.AddParameter("codetype",config["CodeType","5006"]);
                    request.AddFileBytes("file",ms.ToArray(),"upload.jpg","image/jpeg");
                });
                if(result == null || !result.ContainsKey("ret") || !result.ContainsKey("cid") || result["ret"].ToString() != "0")
                {
                    return null;
                }
                return result["cid"].ToString();
            }
        }

        public string getResult(string identifier)
        {
            var result = request("result",(request) =>
            {
                request.AddParameter("cid",identifier);
            });
            if(result == null || !result.ContainsKey("ret") || !result.ContainsKey("text") || result["ret"].ToString() != "0")
            {
                return null;
            }
            return result["text"].ToString();
        }

        public bool reportSuccess(string identifier)
        {
            var result = request("report",(request) =>
            {
                addBasicParams(request);
                request.AddParameter("flag","1");
                request.AddParameter("cid",identifier);
            });
            if(result == null || !result.ContainsKey("ret") || result["ret"].ToString() != "0")
            {
                return false;
            }
            return true;
        }

        public bool reportError(string identifier)
        {
            var result = request("report",(request) =>
            {
                addBasicParams(request);
                request.AddParameter("flag","0");
                request.AddParameter("cid",identifier);
            });
            if(result == null || !result.ContainsKey("ret") || result["ret"].ToString() != "0")
            {
                return false;
            }
            return true;
        }

        public void addBasicParams(IRestRequest request)
        {
            request.AddParameter("username",config["Username"]);
            request.AddParameter("password",config["Password"]);
            request.AddParameter("appid",config["AppID","4673"]);
            request.AddParameter("appkey",config["AppKey","9027ce24a817991a00c7a955e5ba441d"]);
        }
    }
}
