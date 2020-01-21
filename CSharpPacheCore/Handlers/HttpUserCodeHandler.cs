using CSharpPacheCore.Types;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;
using CSharpPacheCore.Streamers;

namespace CSharpPacheCore.Handlers
{
    static class HttpUserCodeHandler
    {
        static Dictionary<string, AbstractUserController> UserControllers = new Dictionary<string, AbstractUserController>();
        static Dictionary<string, AbstractWebSocket> WebSockets = new Dictionary<string, AbstractWebSocket>();
        static IEnumerable<AbstractMiddleware> middlewares;
        public static void setControllers<T>()
        {
            //Injection Object 
            setDependyObjects<T>();
            //Controllers
            IEnumerable<AbstractUserController> controllers = typeof(T)
                .Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(AbstractUserController)) && !t.IsAbstract)
                .Select(t => (AbstractUserController)Activator.CreateInstance(t));
            //Middlewares
            middlewares = typeof(T)
                    .Assembly.GetTypes()
                    .Where(t => t.IsSubclassOf(typeof(AbstractMiddleware)) && !t.IsAbstract)
                    .Select(t => (AbstractMiddleware)Activator.CreateInstance(t));
            //WebSockets
            IEnumerable<AbstractWebSocket> SocketList = typeof(T)
                .Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(AbstractWebSocket)) && !t.IsAbstract)
                .Select(t => (AbstractWebSocket)Activator.CreateInstance(t));


            foreach (AbstractWebSocket abstractSocket in SocketList)
            {
                WebSockets.Add(abstractSocket.Config().Url, abstractSocket);
            }

            foreach (AbstractUserController abstractUserController in controllers)
            {
                UserControllers.Add(abstractUserController.Config().HttpMethod + " "+ abstractUserController.Config().Url, abstractUserController);
            }
        }

        static Dictionary<string, Type> depencyObjects = new Dictionary<string, Type>();
        static void setDependyObjects<T>()
        {            
            foreach (Type type in typeof(T).Assembly.GetTypes())
            {
                if (type.GetCustomAttributes(typeof(AutoConfigureComponentAttribute), true).Length > 0)
                {
                    depencyObjects.Add(type.GetInterfaces()[0].Name, type);
                }
            }
        }
        public static HttpResponse HandleWebSocket(HttpRequest req, CPacheStream cpacheStream){
            //check for websocketroutes 
          
                var WebSocket = (AbstractWebSocket)Activator.CreateInstance(WebSockets[req.Url.Split('?')[0]].GetType());

                foreach (FieldInfo field in WebSocket.GetType().GetFields())
                {
                    // if true then it has an autoconfigure item. 
                    if (field.GetCustomAttributes(typeof(AutoConfigureAttribute), true).Length > 0)
                    {
                        field.SetValue(WebSocket, Activator.CreateInstance(depencyObjects[field.FieldType.Name]));
                    }
                }
            WebSocket.StartWebSocket(req,cpacheStream);
          
            return null;
        }
        public static HttpResponse GetResponse(HttpRequest req, CPacheStream cpacheStream)
        {

            if (req.WebMethod == HttpMethod.GET && WebSockets.ContainsKey(req.Url.Split("?")[0]))
            {
                    var rets = HandleWebSocket(req, cpacheStream);
                return rets;
            }
            var Controller = (AbstractUserController)Activator.CreateInstance(UserControllers[req.WebMethod + " " + req.Url.Split('?')[0]].GetType());

            foreach (FieldInfo field in Controller.GetType().GetFields())
            {
                // if true then it has an autoconfigure item. 
                if (field.GetCustomAttributes(typeof(AutoConfigureAttribute), true).Length > 0)
                {
                    field.SetValue(Controller, Activator.CreateInstance(depencyObjects[field.FieldType.Name]));
                }
            }

            Controller.abstractMiddlewares = new List<AbstractMiddleware>();
            foreach(AbstractMiddleware abstractMiddleware in middlewares)
            {
                if (abstractMiddleware.RouteAcceptance(req))
                {
                    Controller.abstractMiddlewares.Add(abstractMiddleware);
                }
            }

            foreach (var mw in Controller.abstractMiddlewares)
            {
                req = mw.HttpRequest(req);
            }

            var ret = Controller.HttpResponse(req);

            foreach(var mw in Controller.abstractMiddlewares)
            {
                ret = mw.HttpResponse(ret);
            }

            return ret;
        }

        


    }
}
