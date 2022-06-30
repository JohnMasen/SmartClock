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
        
        
        public ClockPack LoadFromFolder(string path)
        {
            ClockPack result = new ClockPack();
            var mainFile = Directory.GetFiles(path, "main.csx", new EnumerationOptions()
            {
                RecurseSubdirectories = false,
                MatchCasing = MatchCasing.CaseInsensitive
            }).FirstOrDefault();
            if (mainFile == null)
            {
                throw new FileNotFoundException($"Could not find main.csx at folder {path}");
            }
            result.Code = File.ReadAllText(mainFile);

            foreach (var item in Directory.GetFiles(path))
            {
                if (string.Compare(Path.GetFileName(item),"main.csx",true)!=0)//exclude main code
                {
                    result.Files.Add(item);
                }
                
            }

            DirectoryInfo info = new DirectoryInfo(path);

            result.Name = info.Name;
            return result;
        }

        public ScriptClockIS BuildClock(ClockPack clock, IClockRenderer render, ClockRefreshIntervalEnum refreshInterval = ClockRefreshIntervalEnum.OneTime)
        {
            MemoryStream ms = new MemoryStream();
            buildClockArchive(clock, ms, true);
            ms.Seek(0, SeekOrigin.Begin);
            ZipArchive readonlyArchive = new ZipArchive(ms, ZipArchiveMode.Read);
            return new ScriptClockIS(new PackageLoader(readonlyArchive), render, new InfoManager(), refreshInterval);
        }
        public void SaveClock(ClockPack clock,Stream output)
        {
            buildClockArchive(clock, output);
        }
        public void SaveClock(ClockPack clock,string savePath)
        {
            using FileStream fs = File.OpenWrite(savePath);
            SaveClock(clock, fs);
        }
        private void buildClockArchive(ClockPack clock,Stream outputStream,bool leaveStreamOpen=false)
        {
            ZipArchive archive = new ZipArchive(outputStream, ZipArchiveMode.Create, leaveStreamOpen);
            using (var writer = new StreamWriter(archive.CreateEntry("main.csx").Open()))
            {
                writer.Write(clock.Code);
            }


            foreach (var f in clock.Files)
            {
                using var imgStream = archive.CreateEntry(Path.GetFileName(f)).Open();
                using var fs = File.Open(f, FileMode.Open);
                fs.CopyTo(imgStream);
            }
            archive.Dispose();
        }

    }
}
