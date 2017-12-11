using Microsoft.AspNetCore.Mvc.Rendering;
using SmartClock.JSClockManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartClock.Web.Models
{
    public class IndexModel
    {
        public IEnumerable<ClockScriptInfo> InstalledScripts { get; set; }
    }
}
