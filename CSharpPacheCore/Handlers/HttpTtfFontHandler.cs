using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CSharpPacheCore.Handlers
{
    public class HttpTtfFontHandler
    {
        String FontType = "";
        String Dir = "";

        public HttpTtfFontHandler(String fontType, String dir)
        {
            this.FontType = fontType;
            this.Dir = dir;
        }

        public Byte[] ReadAllBytes()
        {
            return File.ReadAllBytes(this.Dir);
        }





    }
}
