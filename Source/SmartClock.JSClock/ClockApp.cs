using System;
using System.Collections.Generic;
using System.Text;
using ChakraCore.NET.Hosting;

namespace SmartClock.JSClock
{
    class ClockApp:JSValueWrapperBase
    {
        public void Init()
        {
            Reference.CallMethod("Init");
        }

        public void Draw()
        {
            Reference.CallMethod("Draw");
        }
    }
}
