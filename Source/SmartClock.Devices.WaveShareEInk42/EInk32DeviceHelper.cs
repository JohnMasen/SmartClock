using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartClock.Devices.WaveShareEInk42
{
    public static class EInk32DeviceHelper
    {
        public static async Task RenderRGBAFrameAsync(this Eink32Device device, byte[] bgraBuffer)
        {
            await device.DisplayFrameAsync(convertRGBABuffer(bgraBuffer));
        }
        private static byte[] convertRGBABuffer(byte[] buffer)
        {
            if (buffer.Length % 32 != 0)
            {
                throw new ArgumentException("buffer length must be multiply of 32");
            }
            byte[] result = new byte[buffer.Length / 32];
            int pos = 0;
            byte tmp = 0;
            byte[] masks =
            {
                0x80, //1000 0000
                0x40, //0100 0000
                0x20, //0010 0000
                0x10, //0001 0000
                0x08, //0000 1000
                0x04, //0000 0100
                0x02, //0000 0010
                0x01, //0000 0001
            };
            for (int i = 0; i < result.Length; i++)
            {
                tmp = 0;
                for (int j = 0; j < 8; j++)
                {
                    byte r = buffer[pos++];
                    byte g = buffer[pos++];
                    byte b = buffer[pos++];
                    pos++; //skip alpha channel
                    if (!(r == 0 && b == 0 && g == 0))
                    {
                        tmp += masks[j];
                    }
                }
                result[i] = tmp;
            }
            return result;
        }
    }
}
