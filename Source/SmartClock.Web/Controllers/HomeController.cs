using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SmartClock.Web.Models;
using SmartClock.JSClockManager;
using SmartClock.NetcoreRenderer;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using SmartClock.InfoProviders.XinZhiWeatherInfoProvider;

namespace SmartClock.Web.Controllers
{
    public class HomeController : Controller
    {
        Manager manager = new Manager();
        PNGStreamRenderer previewRenderer;
        public HomeController()
        {
            var xinzhi = new XinzhiWeatherForcast("gxs3ezcux67dzvqa", "shanghai");//replace the key with your own, this is for my development only
            xinzhi.Start();
            manager.InfoManager.Providers.Add(xinzhi);
            previewRenderer = new PNGStreamRenderer();
            manager.AddDefinition("preview", previewRenderer, Core.ClockRefreshIntervalEnum.OneTime);
            RemoteRenderer remote = new RemoteRenderer("192.168.0.220");
            manager.AddDefinition("PerMinute", remote, Core.ClockRefreshIntervalEnum.PerMinute);
        }
        public IActionResult Index()
        {
            IndexModel m = new IndexModel();
            m.InstalledScripts = manager.InstalledClockScripts;
            return View(m);
        }

        public FileStreamResult Preview(string id)
        {
            manager.StartClock("preview", id);
            Stream r=previewRenderer.GetOutput();
            while (r==null)
            {
                Task.Delay(1000).Wait();
                r = previewRenderer.GetOutput();
            }
            r.Seek(0, 0);
            FileStreamResult result = new FileStreamResult(r, "image/x-png");
            return result;
        }

        public JsonResult RunClock(string id)
        {
            manager.StartClock("PerMinute", id);
            return new JsonResult("OK");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
