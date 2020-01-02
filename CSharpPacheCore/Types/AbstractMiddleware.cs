using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpPacheCore.Types
{
    public abstract class AbstractMiddleware
    {
      
        public virtual HttpRequest HttpRequest(HttpRequest req)
        {
            return req;
        }
        public virtual HttpResponse HttpResponse(HttpResponse res)
        {
            return res;
        }
        public abstract Boolean RouteAcceptance(HttpRequest httpRequest);
    }
}
