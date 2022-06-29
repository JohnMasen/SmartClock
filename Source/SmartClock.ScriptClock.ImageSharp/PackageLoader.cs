using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
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
            using var f= FindEntry(path).Open();
            return Image.Load<Rgba32>(f);
        }

        public string LoadText(string path)
        {
            using var f = FindEntry(path).Open();
            using var reader = new StreamReader(f);
            return reader.ReadToEnd();
        }

        public Stream LoadStream(string path)
        {
            return FindEntry(path).Open();
        }

        private ZipArchiveEntry FindEntry(string name)
        {
            return package.Entries.FirstOrDefault(x => string.Compare(name, x.Name, true)==0);
        }
    }
}
