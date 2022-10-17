// See https://aka.ms/new-console-template for more information
using Iot.Device.OneWire;
using QuickTest;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SmartClock.ScriptClock.ImageSharp;
using System.IO.Compression;

Console.WriteLine("Hello, World!");
using MemoryStream ms = new MemoryStream();
using (ZipArchive archive = new ZipArchive(ms, ZipArchiveMode.Create, true))
{
    foreach (var item in Directory.GetFiles("Test1"))
    {
        archive.CreateEntryFromFile(item, Path.GetFileName(item));
    }
}
ms.Position = 0;
ZipArchive content = new ZipArchive(ms, ZipArchiveMode.Read);
var render = new EInkRenderer();
var clock=ScriptClockIS.Load(content, render,null,SmartClock.Core.ClockRefreshIntervalEnum.PerMinute);
clock.Init();
clock.Start();
//string p = Path.Combine("Test1", "Pic1.jpg");
//var img=Image.Load<Rgba32>(p);
//await render.RenderAsync(img, Task.Factory.CancellationToken);

Console.WriteLine("Press ENTER to exit");
Console.ReadLine();
clock.Stop();
render.Clear();
