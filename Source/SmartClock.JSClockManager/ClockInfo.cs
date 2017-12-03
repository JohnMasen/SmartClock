using SmartClock.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartClock.JSClockManager
{
    public class ClockInfo
    {
        public JSClock.JSClock Clock { get; set; }
        public IClockRenderer Renderer { get; private set; }
        public string ScriptFolder { get; set; }
        public ClockRefreshIntervalEnumn RefreshInterval { get; private set; }
        public ClockInfo(IClockRenderer renderer,ClockRefreshIntervalEnumn refreshInterval)
        {
            Renderer = renderer;
            RefreshInterval = refreshInterval;
        }
    }
}
