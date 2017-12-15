using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinIoTEInk32RenderServer
{
    internal static class RendererHost
    {
        internal static SmartClock.UWPRenderer.WaveShareEink32Renderer Renderer { get; private set; } = new SmartClock.UWPRenderer.WaveShareEink32Renderer();
    }
}
