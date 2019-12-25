using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace CSharpPacheCore.Streamers
{
    public class CPacheStream
    {
        readonly NetworkStream ByteStream;
        readonly StreamWriter streamWriter;
        public CPacheStream(NetworkStream stream)
        {
            this.ByteStream = stream;
            this.streamWriter = new StreamWriter(stream);
        }

        public void Write(Byte[] array)
        {
            this.ByteStream.Write(array, 0, array.Length);
            if (Encoding.UTF8.GetString(array) != Environment.NewLine)
            {
                //   Console.WriteLine(Encoding.UTF8.GetString(array));
            }
        }
        public void Write(string text)
        {
            this.streamWriter.Write(text);
            if (text != Environment.NewLine)
            {
                // Console.WriteLine(text);
            }
        }
        public void Flush()
        {
            this.streamWriter.Flush();
            this.ByteStream.Flush();
            System.Threading.Thread.Sleep(10);//not sure why i have to do this. 
        }
        public void Close()
        {
            this.ByteStream.Close();
            this.streamWriter.Close();
        }


    }
}
