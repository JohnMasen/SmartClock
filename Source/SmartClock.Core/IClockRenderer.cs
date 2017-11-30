using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartClock.Core
{
    public class RenderInfo
    {
        public string Name { get; set; }
        public string Version { get; set; }
    }
    public interface IClockRenderer
    {
        Task RenderAsync(Image<Rgba32> image);
        RenderInfo Info { get; }
    }
}
