using CSharpPacheCore.Handlers;
using CSharpPacheCore.Types;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace CSharpPacheCore
{
    public class CPacheService
    {
        
       readonly TcpListener server = null;
        readonly IPAddress localAddr = null;
        public CPacheService(ServiceConfig config)
        {
            localAddr = IPAddress.Parse(config.IpAdress);
            server = new TcpListener(localAddr, config.ListeningPort);
        }
        public void Start<T>()
        {
            HttpUserCodeHandler.setControllers<T>();
            
            try
            {
                server.Start();
                while (true)
                {
                    Console.WriteLine("Waiting for a connection... ");
                    var client = server.AcceptTcpClient();
                    HttpRequestHandler httpRequestHandler = new HttpRequestHandler(client);
                    var handle = new Task(httpRequestHandler.Handle);
                    handle.Start();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
        }
    }
}
    

