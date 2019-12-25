using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpPacheCore.Utils
{
    public class CSharpSoftJson
    {
        public static String ToJson(object obj)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("{");
            Type type = obj.GetType();
            foreach (var prop in type.GetProperties())
            {
                stringBuilder.Append(string.Concat("\"", prop.Name, "\":\"", prop.GetValue(obj, null), "\""));
                if (prop != type.GetProperties()[type.GetProperties().Length - 1])
                {
                    stringBuilder.Append(",");
                }
            }
            stringBuilder.Append("}");

            return stringBuilder.ToString();
        }

        public static T ToObject<T>(String Json)
        {
            return (T)new object();
        }
    }
}
