using Chakra.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClock.WaveShareEInk.JS
{
    class LEDNumberWrapper
    {
        public static void Inject(ChakraContext context)
        {
            context.ValueConverter.RegisterProxyConverter<Controls.LEDNumber>(
                (output, source) =>
                {
                    output.SetMethod<string>("draw", source.Draw);
                }
            );
        }
    }
}
