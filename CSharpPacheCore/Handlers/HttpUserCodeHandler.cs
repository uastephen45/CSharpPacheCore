using CSharpPacheCore.Types;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;

namespace CSharpPacheCore.Handlers
{
    static class HttpUserCodeHandler
    {
        static Dictionary<string, AbstractUserController> UserControllers = new Dictionary<string, AbstractUserController>();
        static IEnumerable<AbstractMiddleware> middlewares;
        public static void setControllers<T>()
        {
            setDependyObjects<T>();
            IEnumerable<AbstractUserController> controllers = typeof(T)
                .Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(AbstractUserController)) && !t.IsAbstract)
                .Select(t => (AbstractUserController)Activator.CreateInstance(t));

            middlewares = typeof(T)
                    .Assembly.GetTypes()
                    .Where(t => t.IsSubclassOf(typeof(AbstractMiddleware)) && !t.IsAbstract)
                    .Select(t => (AbstractMiddleware)Activator.CreateInstance(t));

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

        public static HttpResponse GetResponse(HttpRequest req)
        {
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
