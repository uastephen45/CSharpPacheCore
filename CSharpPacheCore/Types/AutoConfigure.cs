using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpPacheCore.Types
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class AutoConfigureAttribute : System.Attribute
    {
        public AutoConfigureAttribute()
        {
        }
    }
}
