using SmartClock.Core;
using System;
using System.Collections.Generic;
using System.Text;
using SixLabors.ImageSharp;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace SmartClock.NetcoreRenderer
{
    public class PNGStreamRenderer : IClockRenderer
    {
        public RenderInfo Info => new RenderInfo() { Name = nameof(PNGStreamRenderer), Version = "1.0.0" };

        private MemoryStream output;
        public Stream GetOutput()
        {
            var result = output;
            output = null;
            return result;
            
        }

        public Task RenderAsync(Image<Rgba32> image, CancellationToken token)
        {
            var tmp = new MemoryStream();
            image.SaveAsPng(tmp);
            output = tmp;
            return Task.CompletedTask;
        }
    }
}
