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
            context.ValueConverter.RegisterConverter<Controls.LEDNumber>(
                (value, helper) =>
                {
                    return helper.CreateProxyObject<Controls.LEDNumber>(value,
                        (v) =>
                        {
                            v.SetMethod<string>("draw", value.Draw);
                        });
                }
            , null);
        }
    }
}
