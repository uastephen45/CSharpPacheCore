using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpPacheCore.Types
{
    [System.AttributeUsage(System.AttributeTargets.Class)]

    public class MiddlewareAttribute : System.Attribute
    {
        public MiddlewareType MType;
        public MiddlewareAttribute(MiddlewareType middlewareType)
        {
            this.MType = middlewareType;
        }
    }
    public enum MiddlewareType
    {
        Before,
        After
    }
}
