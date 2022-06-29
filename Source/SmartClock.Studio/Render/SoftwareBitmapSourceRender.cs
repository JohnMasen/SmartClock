using Microsoft.Toolkit.HighPerformance.Buffers;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Media.Imaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SmartClock.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Display.Core;
using Windows.Graphics.Imaging;
using Windows.UI.Core;

namespace SmartClock.Studio.Render
{
    public class SoftwareBitmapSourceRender:IClockRenderer
    {
        public SoftwareBitmapSourceRender()
        {
            //Image = new SoftwareBitmapSource();
            //bitmap = new SoftwareBitmap(BitmapPixelFormat.Bgra8, 800, 600, BitmapAlphaMode.Premultiplied);
        }
        public SoftwareBitmapSource Image { get; private set; }= new SoftwareBitmapSource();

        public RenderInfo Info { get; } = new RenderInfo() { Name = nameof(SoftwareBitmapSourceRender), Version = "1.0.0.0" };

        private SoftwareBitmap bitmap;


        public async Task RenderAsync(Image<Rgba32> image, CancellationToken token)
        {
            Debug.WriteLine("Begin render");
            if (bitmap == null || bitmap.PixelHeight != image.Height || bitmap.PixelWidth != image.Width)
            {
                //Image = new SoftwareBitmapSource();
                bitmap = new SoftwareBitmap(BitmapPixelFormat.Bgra8, image.Width, image.Height, BitmapAlphaMode.Premultiplied);
            }
            using var buffer = MemoryOwner<Bgra32>.Allocate(bitmap.PixelWidth * bitmap.PixelHeight);
            Configuration configuration = new Configuration() { PreferContiguousImageBuffers = true };
            image.CloneAs<Bgra32>(configuration).DangerousTryGetSinglePixelMemory(out var bgraBuffer);
            var tmpBuffer = MemoryMarshal.AsBytes(bgraBuffer.Span).ToArray();
            bitmap.CopyFromBuffer(tmpBuffer.AsBuffer());
            await Task.Factory.StartNew(() =>
            {
                Image.SetBitmapAsync(bitmap);
                Debug.WriteLine("End render");
            },Task.Factory.CancellationToken,TaskCreationOptions.None,App.Current.UIScheduler);
            
        }
        
    }
}
