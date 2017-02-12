using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace SmartClock.WaveShareEInk
{
    public class EInkCommand
    {
        List<byte> data=new List<byte>();
        private static readonly byte[] COMMAND_HEADER = {0xA5};
        private static readonly byte[] COMMAND_END = { 0xCC, 0x33, 0xC3, 0x3C };
        public static readonly byte[] COMMAND_OK = new byte[2]{ 0x4F, 0x4B };
        
        public byte CommandType { get; private set; }

        public int ResultLength { get; set; } 

        public EInkCommand(byte commandType,int resultLength=2)
        {
            CommandType = commandType;
            ResultLength = resultLength;
        }

        public EInkCommand(byte commandType,  params byte[] para)
        {
            CommandType = commandType;
            ResultLength = 2;
            AddParameter(para);
        }

        public EInkCommand(byte commandType,  params int[] para)
        {
            CommandType = commandType;
            ResultLength = 2;
            foreach (var item in para)
            {
                AddParameter((short)item);
            }
        }

        public EInkCommand(byte commandType, params float[] para)
        {
            CommandType = commandType;
            ResultLength = 2;
            foreach (var item in para)
            {
                AddParameter((short)item);
            }
        }

        public EInkCommand(byte commandType, float x,float y, string text)
        {
            CommandType = commandType;
            ResultLength = 2;
            AddParameter((short)x);
            AddParameter((short)y);
            AddParameter(text);
        }


        public void ClearParameter()
        {
            data.Clear();
        }
        public void AddParameter(byte[] value)
        {
            data.AddRange(value);
        }

        public void AddParameter(string value)
        {
            
            var tmp = System.Text.Encoding.GetEncoding("GBK").GetBytes(value);//GBK encoding
            data.AddRange(tmp);
            data.Add(0x00);
        }

        public void AddParameter(Int16 value)
        {
            data.AddRange(int16ToBytes(value));
        }

        private byte[] int16ToBytes(Int16 value)
        {
            var result = new byte[2];
            result[0] = (byte)((value >> 8) & 0xff);
            result[1] = (byte)(value & 0xff);
            return result;
        }

        public byte[] GetCommandBytes()
        {
            List<byte> result = new List<byte>();
            result.AddRange(COMMAND_HEADER);//header

            Int16 length = (Int16)(data.Count + 9);
            result.AddRange(int16ToBytes(length));//length

            result.Add(CommandType);//command type

            result.AddRange(data);//parameter

            result.AddRange(COMMAND_END);//command end

            result.Add(bytesXOR(result));//XOR 

            return result.AsEnumerable().ToArray();
        }

        

        private byte bytesXOR(IEnumerable<byte> data)
        {
            byte buffer=0x00;
            bool isFirst = true;
            foreach (var item in data)
            {
                if (isFirst)
                {
                    buffer = item;
                    isFirst = false;
                }
                else
                {
                    buffer = (byte)(buffer ^ item);
                }
            }
            return buffer;
        }
    }
}
