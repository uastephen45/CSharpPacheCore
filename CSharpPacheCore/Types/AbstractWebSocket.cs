using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpPacheCore.Types
{
    public abstract class AbstractWebSocket
    {
        public abstract UserControllerConfig Config();
        public List<AbstractMiddleware> abstractMiddlewares = new List<AbstractMiddleware>();
        protected abstract HttpResponse Response(HttpRequest req);
        public HttpResponse HttpResponse(HttpRequest req)
        {
            var ret = Response(req);
            if (ret.ByteArrayResponseBody == null)
            {
                ret.ByteArrayResponseBody = Encoding.ASCII.GetBytes(ret.ResponseBody);
            }
            return ret;
        }
    }
}
