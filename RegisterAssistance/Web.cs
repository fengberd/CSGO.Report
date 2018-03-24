using System;
using System.IO;
using System.Net;
using System.Web;
using System.Text;
using System.Net.Security;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

public class Web
{
    public static string UrlEncode(string str)
    {
        return HttpUtility.UrlEncode(str,Encoding.UTF8);
    }

    public static string UrlDecode(string str)
    {
        return HttpUtility.UrlDecode(str,Encoding.UTF8);
    }

    public static string cutString(string data,string start,string end)
    {
        int findStart = data.IndexOf(start), findLast = data.IndexOf(end,findStart + 1);
        if(findLast == -1 || findStart == -1)
        {
            return "";
        }
        findStart += start.Length;
        findLast -= findStart;
        return data.Substring(findStart,findLast);
    }

    public static string HttpGet(string url,ref CookieCollection cookies)
    {
        HttpWebResponse resp = HttpWebResponseUtility.CreateGetHttpResponse(url,null,null,cookies);
        Stream respStream = resp.GetResponseStream();
        StreamReader respStreamReader = new StreamReader(respStream,Encoding.UTF8);
        string strBuff = "";
        char[] cbuffer = new char[256];
        int byteRead = 0;
        byteRead = respStreamReader.Read(cbuffer,0,256);
        while(byteRead != 0)
        {
            string strResp = new string(cbuffer,0,byteRead);
            strBuff = strBuff + strResp;
            byteRead = respStreamReader.Read(cbuffer,0,256);
        }
        cookies = resp.Cookies;
        respStream.Close();
        return strBuff;
    }

    public static string HttpGet(string url)
    {
        CookieCollection cookies = new CookieCollection();
        return HttpGet(url,ref cookies);
    }

    public static Stream HttpGetStream(string url,ref CookieCollection cookies)
    {
        HttpWebResponse resp = HttpWebResponseUtility.CreateGetHttpResponse(url,null,null,cookies);
        cookies = resp.Cookies;
        return resp.GetResponseStream();
    }

    public static Stream HttpGetStream(string url)
    {
        CookieCollection cookies = new CookieCollection();
        return HttpGetStream(url,ref cookies);
    }

    public static byte[] HttpGetBytes(string url,ref CookieCollection cookies)
    {
        try
        {
            HttpWebResponse resp = HttpWebResponseUtility.CreateGetHttpResponse(url,null,null,cookies);
            Stream respStream = resp.GetResponseStream();
            StreamReader respStreamReader = new StreamReader(respStream,Encoding.UTF8);
            char[] cbuffer = new char[1];
            int byteRead = 0;
            byteRead = respStreamReader.Read(cbuffer,0,1);
            byte[] buffer = new byte[resp.ContentLength];
            int index = 0;
            while(byteRead != 0)
            {
                buffer[index] = (byte)cbuffer[0];
                byteRead = respStreamReader.Read(cbuffer,0,1);
                index++;
            }
            cookies = resp.Cookies;
            respStream.Close();
            return buffer;
        }
        catch(Exception e)
        {
            Console.WriteLine(e.ToString());
            return null;
        }
    }

    public static byte[] HttpGetBytes(string url)
    {
        CookieCollection cookies = new CookieCollection();
        return HttpGetBytes(url,ref cookies);
    }

    public static string EasyHttpGet(string url)
    {
        WebClient MyWebClient = new WebClient();
        if(url.StartsWith("https://"))
        {
            MyWebClient.Credentials = CredentialCache.DefaultCredentials;
        }
        return Encoding.UTF8.GetString(MyWebClient.DownloadData(url));
    }

    public static string HttpPost(string url,IDictionary<string,string> parameters,ref CookieCollection cookies,string refer = "")
    {
        HttpWebResponse req = HttpWebResponseUtility.CreatePostHttpResponse(url,parameters,3000,HttpWebResponseUtility.DefaultUserAgent,Encoding.UTF8,cookies,refer);
        Stream respStream = req.GetResponseStream();
        StreamReader respStreamReader = new StreamReader(respStream,Encoding.UTF8);
        string strBuff = "";
        char[] cbuffer = new char[256];
        int byteRead = 0;
        byteRead = respStreamReader.Read(cbuffer,0,256);
        while(byteRead != 0)
        {
            string strResp = new string(cbuffer,0,byteRead);
            strBuff = strBuff + strResp;
            byteRead = respStreamReader.Read(cbuffer,0,256);
        }
        cookies = req.Cookies;
        respStream.Close();
        return strBuff;
    }

    public static string HttpPost(string url,IDictionary<string,string> parameters,string refer = "")
    {
        CookieCollection cookies = new CookieCollection();
        return HttpPost(url,parameters,ref cookies,refer);
    }

    public static string HttpPostUploadBitmap(string url,byte[] data,string filename,string cookies = null,string refer = null,string boundary = "----WebKitFormBoundarycmhp91rP3juPNGBN")
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.ContentType = "multipart/form-data; boundary=" + boundary;
        request.Method = "POST";
        if(cookies != null)
        {
            request.Headers.Add("Cookie: " + cookies);
        }
        if(refer != null)
        {
            request.Referer = refer;
        }
        byte[] postHeaderBytes = Encoding.UTF8.GetBytes("--" + boundary + "\nContent-Disposition: form-data; name=\"file\";filename=\"" + filename + "\"\nContent-Type: image/bmp\n\n");
        byte[] boundaryBytes = Encoding.UTF8.GetBytes("\n--" + boundary + "--\n");
        request.ContentLength = postHeaderBytes.Length + data.Length + boundaryBytes.Length;
        Stream requestStream = request.GetRequestStream();
        requestStream.Write(postHeaderBytes,0,postHeaderBytes.Length);
        requestStream.Write(data,0,data.Length);
        requestStream.Write(boundaryBytes,0,boundaryBytes.Length);
        return new StreamReader(request.GetResponse().GetResponseStream()).ReadToEnd();
    }

    public class HttpWebResponseUtility
    {
        public static WebProxy proxy = null;

        public static readonly string DefaultUserAgent = "Mozilla/5.0 (compatible; CSGO.Report - RegisterAssistance)";

        public static HttpWebResponse CreateGetHttpResponse(string url,int? timeout,string userAgent,CookieCollection cookies)
        {
            if(string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            request.UserAgent = DefaultUserAgent;
            request.Credentials = CredentialCache.DefaultCredentials;
            request.AllowAutoRedirect = false;
            if(proxy != null)
            {
                request.Proxy = proxy;
            }
            if(!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }
            if(timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            if(cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            if(url.StartsWith("https://",StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            }
            return request.GetResponse() as HttpWebResponse;
        }

        public static HttpWebResponse CreatePostHttpResponse(string url,IDictionary<string,string> parameters,int? timeout,string userAgent,Encoding requestEncoding,CookieCollection cookies,string refer = "")
        {
            if(string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            if(requestEncoding == null)
            {
                throw new ArgumentNullException("requestEncoding");
            }
            HttpWebRequest request = null;
            //如果是发送HTTPS请求  
            if(url.StartsWith("https",StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            if(proxy != null)
            {
                request.Proxy = proxy;
            }
            if(refer != "")
            {
                request.Referer = refer;
            }
            if(!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }
            else
            {
                request.UserAgent = DefaultUserAgent;
            }

            if(timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            if(cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            //如果需要POST数据  
            if(!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach(string key in parameters.Keys)
                {
                    if(i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}",key,parameters[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}",key,parameters[key]);
                    }
                    i++;
                }
                byte[] data = requestEncoding.GetBytes(buffer.ToString());
                using(Stream stream = request.GetRequestStream())
                {
                    stream.Write(data,0,data.Length);
                }
            }
            return request.GetResponse() as HttpWebResponse;
        }

        private static bool CheckValidationResult(object sender,X509Certificate certificate,X509Chain chain,SslPolicyErrors errors)
        {
            return true; //总是接受  
        }
    }
}
