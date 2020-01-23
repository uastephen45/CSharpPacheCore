using CSharpPacheCore.Streamers;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpPacheCore.Types
{
    public abstract class AbstractWebSocket
    {
        public abstract WebSocketConfig Config();
        public List<AbstractMiddleware> abstractMiddlewares = new List<AbstractMiddleware>();
        CPacheStream CPacheStream;
        HttpRequest request;
        private bool connected;
        protected abstract void MessageReceived(String Message);
        protected abstract void ClientStream(HttpRequest request,CPacheStream cPacheStream);
        protected abstract void DisposedClientStream(CPacheStream cPacheStream);
        private void startListening()
        {
            connected = true;
            while (connected) {
                try
                {
                    string message = CPacheStream.webSocketReadData();
                    if (!String.IsNullOrEmpty(message))
                    {
                        MessageReceived(message);
                        //CPacheStream.brodcast(message);
                    }
                }
                catch (Exception)
                {

                    DisposedClientStream(CPacheStream);
                    connected = false;
                }
                           
            }
        }


        public HttpResponse StartWebSocket(HttpRequest req,CPacheStream cpacheStream)
        {
            this.request = req;
            this.CPacheStream = cpacheStream;
            //accept and handle the handshake
            acceptUpgrade();
            try
            {
                ClientStream(this.request,cpacheStream);//
                startListening();
            }
            catch (Exception ex)
            {
                DisposedClientStream(this.CPacheStream);
                Console.WriteLine(ex);
                //RemoveClientStream();
            }
            //handle any clean up
            return new HttpResponse(); 
        }
        private void acceptUpgrade()
        { 
           var key = this.request.RequestHeaders["Sec-WebSocket-Key"]; 
           key = key +"258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
            key = Convert.ToBase64String(
            System.Security.Cryptography.SHA1.Create().ComputeHash(
                Encoding.UTF8.GetBytes(key)));
            this.CPacheStream.Write(Encoding.UTF8.GetBytes("HTTP/1.1 101 SwitchingProtocols"));
            this.CPacheStream.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
            this.CPacheStream.Write(Encoding.UTF8.GetBytes("Upgrade: websocket"));
            this.CPacheStream.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
            this.CPacheStream.Write(Encoding.UTF8.GetBytes("Connection: Upgrade"));
            this.CPacheStream.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
            this.CPacheStream.Write(Encoding.UTF8.GetBytes("Sec-WebSocket-Accept: " + key));
            this.CPacheStream.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
            this.CPacheStream.Write(Encoding.UTF8.GetBytes(Environment.NewLine));
        }
    }
}
