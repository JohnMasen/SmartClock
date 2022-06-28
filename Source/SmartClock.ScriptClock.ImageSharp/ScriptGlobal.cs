using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Text;
using SixLabors.ImageSharp.Drawing.Processing;
using SmartClock.Core;
using System.IO.Compression;

namespace SmartClock.ScriptClock.ImageSharp
{
    public class ScriptGlobal
    {
        public PackageLoader Loader { get; set; }
        public InfoManager InfoManager { get; set; }
        public DateTime ClockTime { get; set; }
        public Image Image { get; set; }
        public void DrawImage(string path)
        {
            Image.Mutate(opt =>
            {
                opt.DrawImage(Loader.LoadImage(path), 1f);
            });
        }
    }
}
