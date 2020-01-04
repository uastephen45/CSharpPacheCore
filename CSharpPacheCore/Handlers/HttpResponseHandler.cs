using CSharpPacheCore.Streamers;
using CSharpPacheCore.Types;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace CSharpPacheCore.Handlers
{
    public class HttpResponseHandler
    {
        readonly CPacheStream cpacheSteam;

        HttpResponse Response;
        public HttpResponseHandler(NetworkStream stream, HttpResponse response)
        {
            cpacheSteam = new CPacheStream(stream);
            this.Response = response;
        }
        public void Respond()
        {
            if (this.Response.SC == StatusCode.NotFound)
            {
                NotFound();
            }
            if (this.Response.SC == StatusCode.ServerError)
            {
                ServerError();
            }
            if (this.Response.SC == StatusCode.Ok)
            {
                Ok();
            }
            if(this.Response.SC == StatusCode.SwitchingProtocols)
            {
                SwitchingProtocols();
            }


        }
        void SwitchingProtocols()
        {
            this.cpacheSteam.Write(Encoding.UTF8.GetBytes("HTTP/1.1 101 SwitchingProtocols"));
            this.cpacheSteam.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
            this.cpacheSteam.Write(Encoding.UTF8.GetBytes("Upgrade: websocket"));
            this.cpacheSteam.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
            this.cpacheSteam.Write(Encoding.UTF8.GetBytes("Connection: Upgrade"));
            this.cpacheSteam.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
            this.cpacheSteam.Write(Encoding.UTF8.GetBytes("Sec-WebSocket-Accept: s3pPLMBiTxaQ9kYGzzhZRbK+xOo="));
            this.cpacheSteam.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
            this.cpacheSteam.Write(Encoding.UTF8.GetBytes("Sec - WebSocket - Protocol: chat"));
            this.cpacheSteam.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
            this.cpacheSteam.Write(Encoding.UTF8.GetBytes(Environment.NewLine));

        }
        void OkWithFont()
        {
            this.cpacheSteam.Write(Encoding.UTF8.GetBytes("HTTP/1.1 200 OK"));
            this.cpacheSteam.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
            this.cpacheSteam.Write(Encoding.UTF8.GetBytes("Last-Modified: " + DateTime.Now.ToLongDateString()));
            this.cpacheSteam.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
            this.cpacheSteam.Write(Encoding.UTF8.GetBytes(string.Concat("Content-Type: ", this.Response.ContentType)));
            this.cpacheSteam.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
            this.cpacheSteam.Write(Encoding.UTF8.GetBytes(string.Concat("Content-Length: ", Response.ByteArrayResponseBody.Length)));
            this.cpacheSteam.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
            this.cpacheSteam.Write(Encoding.UTF8.GetBytes("Server: CPache V0.0.1"));
            this.cpacheSteam.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
            this.cpacheSteam.Write(Encoding.UTF8.GetBytes("Date: " + DateTime.Now.ToLongDateString()));
            this.cpacheSteam.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
            this.cpacheSteam.Write(Encoding.UTF8.GetBytes("Connection: keep-alive"));
            this.cpacheSteam.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
            this.cpacheSteam.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
            this.cpacheSteam.Write(Response.ByteArrayResponseBody);

            this.cpacheSteam.Flush();
            this.cpacheSteam.Close();
        }
        void OkWithImage()
        {
            this.cpacheSteam.Write(Encoding.UTF8.GetBytes("HTTP/1.1 200 OK"));
            this.cpacheSteam.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
            this.cpacheSteam.Write(Encoding.UTF8.GetBytes("Last-Modified: " + DateTime.Now.ToLongDateString()));
            this.cpacheSteam.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
            this.cpacheSteam.Write(Encoding.UTF8.GetBytes(string.Concat("Content-Type: ", this.Response.ContentType)));
            this.cpacheSteam.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
            this.cpacheSteam.Write(Encoding.UTF8.GetBytes(string.Concat("Content-Length: ", Response.ByteArrayResponseBody.Length)));
            this.cpacheSteam.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
            this.cpacheSteam.Write(Encoding.UTF8.GetBytes("Server: CPache V0.0.1"));
            this.cpacheSteam.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
            this.cpacheSteam.Write(Encoding.UTF8.GetBytes("Date: " + DateTime.Now.ToLongDateString()));
            this.cpacheSteam.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
            this.cpacheSteam.Write(Encoding.UTF8.GetBytes("Connection: keep-alive"));
            this.cpacheSteam.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
            this.cpacheSteam.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
            this.cpacheSteam.Write(Response.ByteArrayResponseBody);

            this.cpacheSteam.Flush();

            this.cpacheSteam.Close();
        }
        void StandardOk()
        {
            this.cpacheSteam.Write("HTTP/1.1 200 OK");
            this.cpacheSteam.Write(Environment.NewLine);
            this.cpacheSteam.Write(string.Concat("Content-Type: ", this.Response.ContentType, ""));
            this.cpacheSteam.Write(Environment.NewLine);
            this.cpacheSteam.Write("Content-Length: " + this.Response.ByteArrayResponseBody.Length);
            this.cpacheSteam.Write(Environment.NewLine);
            this.cpacheSteam.Write(Environment.NewLine);
            this.cpacheSteam.Write(this.Response.ResponseBody);
            //       this.cpacheSteam.Write(Environment.NewLine);
            this.cpacheSteam.Flush();
            this.cpacheSteam.Close();
        }


        void Ok()
        {
            if (Response.ContentType.Contains("image"))
            {
                OkWithImage();
                return;
            }
            if (Response.ContentType.Contains("font/ttf"))
            {
                OkWithFont();
                return;
            }
            else
            {
                StandardOk();
            }

        }

        void ServerError()
        {
            this.cpacheSteam.Write("HTTP/1.1 500 Internal Server Error");
            this.cpacheSteam.Write(Environment.NewLine);
            this.cpacheSteam.Write("Content-Type: text/html; charset=UTF-8");
            this.cpacheSteam.Write(Environment.NewLine);
            this.cpacheSteam.Write("Content-Length: " + this.Response.ResponseBody.Length);
            this.cpacheSteam.Write(Environment.NewLine);
            this.cpacheSteam.Write(Environment.NewLine);
            this.cpacheSteam.Write(this.Response.ResponseBody);
            this.cpacheSteam.Flush();
            this.cpacheSteam.Close();
        }

        void NotFound()
        {
            this.cpacheSteam.Write("HTTP/1.1 404 File Not Found");
            this.cpacheSteam.Write(Environment.NewLine);
            this.cpacheSteam.Write("Content-Type: text/plain; charset=UTF-8");
            this.cpacheSteam.Write(Environment.NewLine);
            this.cpacheSteam.Write("Content-Length: " + "SDS Webservice could not find the resource".Length);
            this.cpacheSteam.Write(Environment.NewLine);
            this.cpacheSteam.Write(Environment.NewLine);
            this.cpacheSteam.Write("SDS Webservice could not find the resource");
            this.cpacheSteam.Flush();
            this.cpacheSteam.Close();
        }
    }
}

