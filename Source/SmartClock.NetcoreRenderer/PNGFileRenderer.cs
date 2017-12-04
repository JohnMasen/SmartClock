using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SmartClock.Core;
using System.Threading;

namespace SmartClock.NetcoreRenderer
{
    public class PNGFileRenderer : IClockRenderer
    {
        public RenderInfo Info => new RenderInfo() { Name = nameof(PNGFileRenderer), Version = "1.0.0" };
        public string FilePath { get; set; }
        public PNGFileRenderer(string path)
        {
            FilePath = path;
        }
        public async Task RenderAsync(Image<Rgba32> image, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            using (var f = System.IO.File.Open(FilePath, System.IO.FileMode.Create))
            {
                image.SaveAsPng(f);
                await f.FlushAsync();
            }
        }
    }
}
