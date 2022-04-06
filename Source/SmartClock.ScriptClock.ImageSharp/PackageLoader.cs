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
        public PackageLoader(Stream packageStream)
        {
            MemoryStream ms = new MemoryStream();
            packageStream.CopyTo(ms);//copy and release the source stream 
            ms.Position = 0;
            package = new ZipArchive(ms);
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
