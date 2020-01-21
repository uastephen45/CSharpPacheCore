using CSharpPacheCore.Streamers;
using CSharpPacheCore.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace CSharpPacheCore.Handlers
{

        public class HttpRequestHandler
        {

            HttpRequest HttpRequest;
            TcpClient client;
            NetworkStream stream;
            String rawRequest = null;



            public HttpRequestHandler(TcpClient client)
            {
                try
                {
                    this.client = client;
                    stream = client.GetStream();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                }
            }

            void Init()
            {
                Byte[] bytes = new Byte[512];
                HttpRequest = new HttpRequest();
                int i;
                i = stream.Read(bytes, 0, bytes.Length);
                this.rawRequest = System.Text.Encoding.UTF8.GetString(bytes, 0, i);
                if (this.rawRequest == "")
                {
                    client.Close();
                    return;
                }

                char[] whitespace = new char[] { ' ' };
                switch (rawRequest.Split(whitespace)[0])
                {
                    case "GET":
                        this.HttpRequest.WebMethod = HttpMethod.GET;
                        break;
                    case "POST":
                        this.HttpRequest.WebMethod = HttpMethod.POST;
                        break;
                    default:
                        break;
                }
                this.HttpRequest.Url = rawRequest.Split(whitespace)[1];
                Console.WriteLine(string.Concat("URL: ", this.HttpRequest.Url));
                this.HttpRequest.QueryParameters = getQueryParameters();
                this.HttpRequest.RequestBody = "";
                this.HttpRequest.RequestHeaders = new Dictionary<string, string>();
            foreach(String s in rawRequest.Split(Environment.NewLine)){
                if (s.Contains(":"))
                {
                    int pos = s.IndexOf(":");
                    string headername = s.Substring(0, pos).Trim();
                    string headervalue = s.Substring(pos + 1, s.Length - pos - 1).Trim();
                    Console.WriteLine("Header: " + headername + " headervalue: " + headervalue);
                    this.HttpRequest.RequestHeaders.Add(headername, headervalue);
                }
                
            }

            }
            public void Handle()
            {
                Init();
                if (!client.Connected)
                {
                    Console.WriteLine("Bad Request From Client");
                    return;
                }

                HttpResponse responseToSend = GetHttpResponse();
                if (responseToSend != null) { 
                    NetworkStream writer = client.GetStream();
                    HttpResponseHandler responder = new HttpResponseHandler(writer, responseToSend);
                    responder.Respond();
                }
            }   

            public HttpResponse DisgestPostMethod()
            {
                HttpResponse ret = HttpUserCodeHandler.GetResponse(HttpRequest,new CPacheStream(stream)) ;
                return ret;
            }
        
            public HttpResponse DigestGetMethod()
            {
                try
                {
                    string contentType = GetContentType(HttpRequest.Url);
                    if (contentType == "usercode")
                    {
                        HttpResponse ret = HttpUserCodeHandler.GetResponse(HttpRequest,new CPacheStream(stream));
                        return ret;
                    }

                    var dir = AppDomain.CurrentDomain.BaseDirectory;
                    byte[] fileBytes;
                    String text = "";
                    if (contentType == "font/ttf")
                    {
                        string[] parms = string.Concat(@dir, "html/", HttpRequest.Url).Split(new string[] { "?" }, StringSplitOptions.None);
                        var fontHandle = new HttpTtfFontHandler(parms[1], parms[0]);
                        fileBytes = fontHandle.ReadAllBytes();
                        text = Encoding.UTF8.GetString(fileBytes);
                        return new HttpResponse() { ResponseBody = text, SC = StatusCode.Ok, ContentType = contentType, ByteArrayResponseBody = fileBytes };
                    }


                    fileBytes = File.ReadAllBytes(string.Concat(@dir, "html/", HttpRequest.Url));
                    text = Encoding.UTF8.GetString(fileBytes);
                    return new HttpResponse() { ResponseBody = text, SC = StatusCode.Ok, ContentType = contentType, ByteArrayResponseBody = fileBytes };

                }
                catch (Exception ex)    when (ex.Message.Contains("Could not find file"))
                {
                    Console.Write(ex);
                    return new HttpResponse() { SC = StatusCode.NotFound };
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return new HttpResponse() { SC = StatusCode.ServerError, ResponseBody = "Internal Server Error" };
                }

            }

            public Dictionary<string, string> getQueryParameters()
            {
                var ret = new Dictionary<string, string>();
                string[] values;
                try
                {
                    values = this.HttpRequest.Url.Split('?')[1].Split('&');
                    foreach (string val in values)
                    {
                        var v = val.Split('=');
                        ret.Add(v[0], v[1]);
                    }
                }
                catch (Exception ex)
                {
                    return ret;
                }
                return ret;
            }




            HttpResponse GetHttpResponse()
            {

                if (this.HttpRequest.WebMethod == HttpMethod.GET)
                {
                    return DigestGetMethod();
                }

                if (this.HttpRequest.WebMethod == HttpMethod.POST)
                {
                return DisgestPostMethod();
                }

                return new HttpResponse() { SC = StatusCode.NotFound };
            }

            String GetContentType(string url)
            {
                var words = url.Split('.');
                var type = words[words.Length - 1];
                if (type.Contains("ttf?"))
                {
                    type = "ttf";
                }
                switch (type)
                {
                    case "js":
                        return "application/javascript";
                    case "html":
                        return "text/html";
                    case "ico":
                        return "image/x-icon";
                    case "jpg":
                        return "image/jpeg";
                    case "gif":
                        return "image/gif";
                    case "ttf":
                        return "font/ttf";
                    case "css":
                        return "text/css";
                    case "map":
                        return "";




                }
                // must be a user code request
                return "usercode";
            }


        }
    }

