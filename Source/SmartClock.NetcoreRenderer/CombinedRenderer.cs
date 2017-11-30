using System;
using SixLabors.ImageSharp;
using SmartClock.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartClock.NetcoreRenderer
{
    public class CombinedRendererItem
    {
        public bool IsEnabled { get; set; }
        public IClockRenderer Renderer { get; set; }
        public CombinedRendererItem(IClockRenderer renderer,bool enabled=true)
        {
            Renderer = renderer;
            IsEnabled = enabled;
        }
    }
    public class CombinedRenderer : IClockRenderer
    {
        public List<CombinedRendererItem> Renderers { get; private set; } = new List<CombinedRendererItem>();
        public RenderInfo Info => new RenderInfo() { Name = "CombinedRenderer", Version = "1.0.0" };

        public async Task RenderAsync(Image<Rgba32> image)
        {
            
            foreach (var item in Renderers)
            {
                if (item.IsEnabled)
                {
                    await item.Renderer.RenderAsync(image);
                }
            }
        }
    }
}
