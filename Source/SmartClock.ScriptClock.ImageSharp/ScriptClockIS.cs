﻿using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
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
        public ScriptClockIS(PackageLoader loader, IClockRenderer render, InfoManager infoManager, ClockRefreshIntervalEnum refreshInterval = ClockRefreshIntervalEnum.PerSecond) : base(render, infoManager, refreshInterval)
        {
            global = new ScriptGlobal();
            global.Image = new Image<Rgba32>(800, 600);
            global.InfoManager = infoManager;
            global.Loader = loader;
            mainCode = CSharpScript.Create(loader.LoadText("main.csx"), null, typeof(ScriptGlobal));
        }

        public string Name { get; private set; }
        public string Version { get; private set; }
        public static ScriptClockIS Load(Stream contentStream, IClockRenderer render, InfoManager infoManager, ClockRefreshIntervalEnum refreshInterval = ClockRefreshIntervalEnum.PerSecond)
        {
            
            return new ScriptClockIS(new PackageLoader(contentStream), render, infoManager, refreshInterval);
        }

        protected override async Task<Image<Rgba32>> drawClockAsync(CancellationToken token)
        {
            global.ClockTime = DateTime.Now;
            await mainCode.RunAsync(global);
            return global.Image as Image<Rgba32>;
        }
    }
}
