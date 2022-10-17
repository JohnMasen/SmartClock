using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SmartClock.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WaveshareEInkDriver;

namespace QuickTest
{
    internal class EInkRenderer : IClockRenderer
    {
        WaveshareEInkDriver.IT8951SPIDevice device;
        public EInkRenderer()
        {
            device = new WaveshareEInkDriver.IT8951SPIDevice();
            device.Init();
            device.SetVCom(-1.91f);
        }
        public RenderInfo Info => new RenderInfo() { Name = nameof(EInkRenderer), Version = "1.0.0" };

        public Task RenderAsync(Image<Rgba32> image, CancellationToken token)
        {
            var tmp = image.CloneAs<L8>();
            device.DrawImage(tmp);
            return Task.CompletedTask;
        }
    }
}
