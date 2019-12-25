using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpPacheCore.Types
{
        public class HttpRequest
        {
            public String Url;
            public HttpMethod WebMethod;
            public String RequestBody;
            public Dictionary<string, string> QueryParameters;
        }
}

