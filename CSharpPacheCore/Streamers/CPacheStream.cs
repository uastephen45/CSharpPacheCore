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

        public void brodcast(String mess)
        {
            byte[] rawData = Encoding.UTF8.GetBytes(mess);
            int frameCount = 0;
            byte[] frame = new byte[10];
            frame[0] = (byte)129;
            if (rawData.Length <= 125) {
                frame[1] = (byte)rawData.Length;
                frameCount = 2;
            } else if (rawData.Length >= 126 && rawData.Length <= 65535) {
                frame[1] = (byte)126;
                int len = rawData.Length;
                frame[2] = (byte)((len >> 8) & (byte)255);
                frame[3] = (byte)(len & (byte)255);
                frameCount = 4;
            } else {
                frame[1] = (byte)127;
                int len = rawData.Length;
                frame[2] = (byte)((len >> 56) & (byte)255);
                frame[3] = (byte)((len >> 48) & (byte)255);
                frame[4] = (byte)((len >> 40) & (byte)255);
                frame[5] = (byte)((len >> 32) & (byte)255);
                frame[6] = (byte)((len >> 24) & (byte)255);
                frame[7] = (byte)((len >> 16) & (byte)255);
                frame[8] = (byte)((len >> 8) & (byte)255);
                frame[9] = (byte)(len & (byte)255);
                frameCount = 10;
            }
            int bLength = frameCount + rawData.Length;
            byte[] reply = new byte[bLength];
            int bLim = 0;
            for (int i = 0; i < frameCount; i++) {
                reply[bLim] = frame[i];
                bLim++;
            }
            for (int i = 0; i < rawData.Length; i++) {
                reply[bLim] = rawData[i];
                bLim++;
            } 
            this.Write(reply);
            this.Flush();
}



        public void Write(Byte[] array)
        {
            this.ByteStream.Write(array, 0, array.Length);
            if (Encoding.UTF8.GetString(array) != Environment.NewLine)
            {
                //   Console.WriteLine(Encoding.UTF8.GetString(array));
            }
        }
        public String webSocketReadData()
        {
            String ret = "";
            StreamReader streamReader = new StreamReader(ByteStream);
            Byte[] bytes = new Byte[2000000];
            
            int w;
            w = ByteStream.Read(bytes, 0, bytes.Length);
            bool fin = (bytes[0] & 0b10000000) != 0;    
            bool mask = (bytes[1] & 0b10000000) != 0;
            int opcode = bytes[0] & 0b00001111, // expecting 1 - text message
                    msglen = bytes[1] - 128, // & 0111 1111
                    offset = 2;

            if (msglen == 126)
            {
                // was ToUInt16(bytes, offset) but the result is incorrect
                msglen = BitConverter.ToUInt16(new byte[] { bytes[3], bytes[2] }, 0);
                offset = 4;
            }
            else if (msglen == 127)
            {
                Console.WriteLine("TODO: msglen == 127, needs qword to store msglen");
                // i don't really know the byte order, please edit this
                // msglen = BitConverter.ToUInt64(new byte[] { bytes[5], bytes[4], bytes[3], bytes[2], bytes[9], bytes[8], bytes[7], bytes[6] }, 0);
                // offset = 10;
            }

            if (msglen == 0)
                Console.WriteLine("msglen == 0");
            else if (mask)
            {
                byte[] decoded = new byte[msglen];
                byte[] masks = new byte[4] { bytes[offset], bytes[offset + 1], bytes[offset + 2], bytes[offset + 3] };
                offset += 4;

                for (int i = 0; i < msglen; ++i)
                    decoded[i] = (byte)(bytes[offset + i] ^ masks[i % 4]);

                ret = Encoding.UTF8.GetString(decoded);
               
            }
                return ret;           
        }
        public static bool GetBit(byte b, int bitNumber)
        {
            return (b & (1 << bitNumber - 1)) != 0;
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
