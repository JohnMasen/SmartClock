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
using Microsoft.AspNetCore.Http;

namespace SmartClock.Web.Controllers
{
    public class HomeController : Controller
    {
        
        public IActionResult Index()
        {
            IndexModel m = new IndexModel();
            m.InstalledScripts = ClockManager.manager.InstalledClockScripts;
            return View(m);
        }
        [HttpPost]
        public IActionResult UploadScript(List<IFormFile> files)
        {
            
            foreach (var file in files)
            {
                if (file.FileName.EndsWith(".zip"))
                {
                    var name= Path.GetFileNameWithoutExtension(file.FileName);
                    ClockManager.manager.InstallClockScript(file.OpenReadStream(), name);
                }
                
            }
            return RedirectToAction("Index");
        }

        public FileStreamResult Preview(string id)
        {
            ClockManager.manager.StartClock("preview", id);
            Stream r= ClockManager.previewRenderer.GetOutput();
            while (r==null)
            {
                Task.Delay(1000).Wait();
                r = ClockManager.previewRenderer.GetOutput();
            }
            r.Seek(0, 0);
            FileStreamResult result = new FileStreamResult(r, "image/x-png");
            return result;
        }

        public JsonResult RunClock(string id)
        {
            ClockManager.manager.StartClock("PerMinute", id);
            return new JsonResult("OK");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
