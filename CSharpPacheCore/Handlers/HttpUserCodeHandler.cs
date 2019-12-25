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
        public static void setControllers<T>()
        {
            setDependyObjects<T>();
            IEnumerable<AbstractUserController> controllers = typeof(T)
                .Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(AbstractUserController)) && !t.IsAbstract)
                .Select(t => (AbstractUserController)Activator.CreateInstance(t));

            foreach (AbstractUserController abstractUserController in controllers)
            {
                UserControllers.Add(abstractUserController.Config().Url, abstractUserController);
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
            var Controller = UserControllers[req.Url.Split('?')[0]];
            Controller = (AbstractUserController)Activator.CreateInstance(Controller.GetType());
            FieldInfo[] fields = Controller.GetType().GetFields();
            foreach (FieldInfo field in fields)
            {
                // if true then it has a autoconfigure item. 
                if (field.GetCustomAttributes(typeof(AutoConfigureAttribute), true).Length > 0)
                {
                    Console.WriteLine(field.FieldType.Name);
                    Type diType = depencyObjects[field.FieldType.Name];
                    Console.WriteLine(diType.Name);
                    var di = Activator.CreateInstance(diType);
                    field.SetValue(Controller, di);
                }
              
            }



                    var ret = Controller.HttpResponse(req);
            return ret;
        }



    }
}
