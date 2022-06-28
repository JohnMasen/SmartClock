using SixLabors.ImageSharp;
using SmartClock.Core;
using SmartClock.ScriptClock.ImageSharp;
using System.IO.Compression;

namespace SmartClock.Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
            var tmp=createZipPackage("Scripts\\Test1");
            var render = new ImageClockRender();
            InfoManager info = new InfoManager();
            ScriptClockIS clock = ScriptClockIS.Load(tmp, render, info, Core.ClockRefreshIntervalEnum.OneTime);
            clock.Start();
            while (clock.IsRunning)
            {

            }
            render.Image.SaveAsJpeg("result.jpg");
            Console.WriteLine("done");
            //File.Delete(tmp);
        }


        static ZipArchive createZipPackage(string path)
        {
            MemoryStream ms = new MemoryStream();
            ZipArchive result = new ZipArchive(ms,ZipArchiveMode.Update);
            foreach (var f in Directory.GetFiles(path))
            {
                result.CreateEntryFromFile(f, Path.GetFileName(f));
            }
            //tmp.Dispose();
            Console.WriteLine(ms.Length);
            return result;
        }
    }
}