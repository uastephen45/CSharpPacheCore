﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpPacheCore.Types
{
    public abstract class AbstractUserController
    {
        public abstract UserControllerConfig Config();
        public List<AbstractMiddleware> abstractMiddlewares = new List<AbstractMiddleware>();
        protected abstract HttpResponse Response(HttpRequest req);
        public HttpResponse HttpResponse(HttpRequest req)
        {
            var ret = Response(req);
            if (ret.ByteArrayResponseBody == null)
            {
                if(ret.ResponseBody == null)
                {
                    return ret;
                }
                ret.ByteArrayResponseBody = Encoding.ASCII.GetBytes(ret.ResponseBody);
            }
            return ret;
        }
    }
}
