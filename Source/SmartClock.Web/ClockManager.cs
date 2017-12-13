using SmartClock.InfoProviders.XinZhiWeatherInfoProvider;
using SmartClock.JSClockManager;
using SmartClock.NetcoreRenderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartClock.Web
{
    public static class ClockManager
    {
        public readonly static Manager manager = new Manager();
        public readonly static PNGStreamRenderer previewRenderer = new PNGStreamRenderer();
        public static void Init()
        {
            var xinzhi = new XinzhiWeatherForcast("gxs3ezcux67dzvqa", "shanghai");//replace the key with your own, this is for my development only
            xinzhi.Start();
            manager.InfoManager.Providers.Add(xinzhi);
            manager.AddDefinition("preview", previewRenderer, Core.ClockRefreshIntervalEnum.OneTime);
            RemoteRenderer remote = new RemoteRenderer("192.168.0.220");
            manager.AddDefinition("PerMinute", remote, Core.ClockRefreshIntervalEnum.PerMinute);
        }
    }
}
