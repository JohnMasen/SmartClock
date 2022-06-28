using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SmartClock.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartClock.ScriptClock.ImageSharp
{
    public class ImageClockRender :IClockRenderer 
    {
        public Image<Rgba32> Image { get; private set; }

        public RenderInfo Info { get; } = new RenderInfo() { Name = "ImageClockRender", Version = "1.0.0" };

        

        public Task RenderAsync(Image<Rgba32> image, CancellationToken token)
        {
            Image = image;
            return Task.CompletedTask;
        }
    }
}
