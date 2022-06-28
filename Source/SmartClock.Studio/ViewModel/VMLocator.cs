using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClock.Studio.ViewModel
{
    public static class VMLocator
    {
        private static VMMainWindow mainWindow = new VMMainWindow();
        public static  VMMainWindow VMMainWindow => mainWindow;
    }
}
