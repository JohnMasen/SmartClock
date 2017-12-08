using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SmartClock.Core;
using SixLabors.ImageSharp.Processing;
using System.Threading;
using SmartClock.UWPRenderer;

namespace SmartClock.UWPRenderer
{
    public class WaveShareEink32Renderer : IClockRenderer
    {
        public float DitherThreshold { get; set; } = 0.5f;
        public RenderInfo Info => new RenderInfo() { Name = "WaveShareEInk32", Version = "1.0.0" };
        Eink32Device device;
        public bool IsPreProcessEnabled { get; set; } = true;
        public async Task RenderAsync(Image<Rgba32> image, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            if (image==null)
            {
                return;
            }
            if (device == null)
            {
                device = Eink32Device.Default;
                await device.InitAsync();
                await device.ResetAsync();
            }
            Image<Rgba32> tmp;
            if (IsPreProcessEnabled)
            {
                tmp = image.Clone(ctx => ctx
                  .Grayscale(GrayscaleMode.Bt601)
                  .Dither(new SixLabors.ImageSharp.Dithering.FloydSteinbergDiffuser(), DitherThreshold)
                );
            }
            else
            {
                tmp = image;
            }
            var buffer = tmp.SavePixelData();
            await device.DisplayARGB32ByteBufferAsync(buffer);
        }
        
    }
}
