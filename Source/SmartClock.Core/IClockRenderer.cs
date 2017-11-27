using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartClock.Core
{
    public struct RenderInfo
    {
        public string Name;
        public string Version;
    }
    public interface IClockRenderer
    {
        void Render(Image<Rgba32> image);
        RenderInfo Info { get; }
    }
}
