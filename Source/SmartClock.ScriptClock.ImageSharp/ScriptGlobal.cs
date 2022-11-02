using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Text;
using SixLabors.ImageSharp.Drawing.Processing;
using SmartClock.Core;
using System.IO.Compression;
using System.Net.Http.Headers;
using SixLabors.ImageSharp.Memory;
using SixLabors.Fonts;
using System.IO;
using SixLabors.ImageSharp.PixelFormats;

namespace SmartClock.ScriptClock.ImageSharp
{
    public class ScriptGlobal
    {
        public IPackageLoader Loader { get; private set; }
        public InfoManager InfoManager { get; private set; }
        public DateTime ClockTime { get; set; }
        public Image<Rgba32> Image { get; private set; }
        public ScriptGlobal(IPackageLoader loader, InfoManager infoManager,  Image<Rgba32> image)
        {
            Loader = loader;
            InfoManager = infoManager;
            Image = image;
            IsFirstRun = true;
        }

        public bool IsFirstRun { get; internal set; } 
        private FontCollection fontCollection = new FontCollection();
        
        public void DrawImage(string path)
        {
            DrawImage(Loader.LoadImage(path));
        }
        public void DrawImage(Image img, int posX = 0, int posY = 0, int? sizeX = null, int? sizeY = null)
        {
            if (sizeX.HasValue && sizeY.HasValue && (sizeX.Value != img.Width || sizeY.Value != img.Height))
            {
                img = img.Clone(opt =>
                {
                    ResizeOptions resizeOptions = new ResizeOptions();
                    resizeOptions.Size = new Size(sizeX.Value, sizeY.Value);
                    resizeOptions.Mode = ResizeMode.Stretch;
                    opt.Resize(resizeOptions);
                });

            }

            Image.Mutate(opt =>
            {
                opt.DrawImage(img, 1f);
            });
        }

        public void Rotate(Image img, float degrees)
        {
            img.Mutate(opt =>
            {
                opt.Rotate(degrees);
            });
        }
        public void LoadFont(string path)
        {
            using var fontStream = Loader.LoadStream(path);
            using MemoryStream ms = new MemoryStream();
            fontStream.CopyTo(ms);
            ms.Position = 0;
            var ffamily = fontCollection.Add(ms);
        }

        public void DrawText(string text,string fontName,float fontSize, int posX, int posY,string colorHex)
        {
            var font = fontCollection.Get(fontName).CreateFont(fontSize);
            Image.Mutate(opt =>
            {
                opt.DrawText(text, font, Color.ParseHex(colorHex), new PointF(posX, posY));
            });
        }
    }
}
