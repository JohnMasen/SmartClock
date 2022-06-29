using SmartClock.Core;
using SmartClock.ScriptClock.ImageSharp;
using SmartClock.Studio.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClock.Studio.Services
{
    public class ClockManager
    {
        public static string[] ImageFiles = { ".jpg", ".bmp", ".png" };
        public ClockPack LoadFromFolder(string path)
        {
            ClockPack result = new ClockPack();
            string rootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            var mainFile = Directory.GetFiles(rootPath, "main.csx", new EnumerationOptions()
            {
                RecurseSubdirectories = false,
                MatchCasing = MatchCasing.CaseInsensitive
            }).FirstOrDefault();
            if (mainFile == null)
            {
                throw new FileNotFoundException($"Could not find main.csx at folder {rootPath}");
            }
            result.Code = File.ReadAllText(mainFile);

            var files = from f in Directory.GetFiles(rootPath)
                        join ext in ImageFiles on Path.GetExtension(f) equals ext
                        select f;
            foreach (var item in files)
            {
                result.Images.Add(item);
            }
            DirectoryInfo info = new DirectoryInfo(rootPath);

            result.Name = info.Name;
            return result;
        }

        public ScriptClockIS BuildClock(ClockPack clock, IClockRenderer render, ClockRefreshIntervalEnum refreshInterval = ClockRefreshIntervalEnum.OneTime)
        {
            MemoryStream ms = new MemoryStream();
            ZipArchive archive = new ZipArchive(ms, ZipArchiveMode.Create, true);
            using (var writer = new StreamWriter(archive.CreateEntry("main.csx").Open()))
            {
                writer.Write(clock.Code);
            }
            

            foreach (var f in clock.Images)
            {
                using var imgStream = archive.CreateEntry(Path.GetFileName(f)).Open();
                using var fs = File.Open(f, FileMode.Open);
                fs.CopyTo(imgStream);
            }
            archive.Dispose();
            ms.Seek(0, SeekOrigin.Begin);
            ZipArchive readonlyArchive = new ZipArchive(ms, ZipArchiveMode.Read);
            return new ScriptClockIS(new PackageLoader(readonlyArchive), render, new InfoManager(), refreshInterval);
        }

    }
}
