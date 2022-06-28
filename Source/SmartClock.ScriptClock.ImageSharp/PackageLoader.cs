using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace SmartClock.ScriptClock.ImageSharp
{
    public class PackageLoader
    {
        ZipArchive package;
        public PackageLoader(ZipArchive content)
        {
            package = content;
        }
        public Image<Rgba32> LoadImage(string path)
        {
            using var f= package.GetEntry(path).Open();
            return Image.Load<Rgba32>(f);
        }

        public string LoadText(string path)
        {
            using var f = package.GetEntry(path).Open();
            using var reader = new StreamReader(f);
            return reader.ReadToEnd();
        }
    }
}
