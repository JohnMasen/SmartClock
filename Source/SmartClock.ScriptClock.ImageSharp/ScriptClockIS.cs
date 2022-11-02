using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SmartClock.Core;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace SmartClock.ScriptClock.ImageSharp
{
    public class ScriptClockIS:ClockBase
    {
        Script mainCode;
        ScriptGlobal global;
        public ScriptClockIS(IPackageLoader loader, IClockRenderer render, InfoManager infoManager, ClockRefreshIntervalEnum refreshInterval = ClockRefreshIntervalEnum.PerSecond) : base(render, infoManager, refreshInterval)
        {
            global = new ScriptGlobal(loader, infoManager, new Image<Rgba32>(800, 600));
            mainCode = CSharpScript.Create(loader.LoadText("Main.csx"), null, typeof(ScriptGlobal));
        }

        public string Name { get; private set; }
        public string Version { get; private set; }
        public static ScriptClockIS Load(ZipArchive packageContent, IClockRenderer render, InfoManager infoManager, ClockRefreshIntervalEnum refreshInterval = ClockRefreshIntervalEnum.PerSecond)
        {
            
            return new ScriptClockIS(new PackageLoader(packageContent), render, infoManager, refreshInterval);
        }

        protected override async Task<Image<Rgba32>> drawClockAsync(CancellationToken token)
        {
            global.Image.Mutate(opt =>
            {
                opt.Clear(Color.White);
            });

            global.ClockTime = DateTime.Now;
            await mainCode.RunAsync(global);
            global.IsFirstRun = false;
            return global.Image as Image<Rgba32>;
        }
    }
}
