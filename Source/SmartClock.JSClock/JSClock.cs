using SmartClock.Core;
using System;
using SixLabors.ImageSharp;
using System.Linq;
using System.Threading;
using ChakraCore.NET.Plugin.Drawing.ImageSharp;
using ChakraCore.NET.Hosting;

namespace SmartClock.JSClock
{
    public class JSClock : ClockBase
    {
        public string[] ScriptFolders { get; private set; }
        public string ResourceFolder { get; private set; }
        InfoManager manager;
        ClockApp app;
        ImageSharpDrawingInstaller engine;
        protected override Image<Rgba32> drawClock(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            app.Draw();
            var result = engine.LastDrawingSurface?.Image;
            if (result==null)
            {
                return null;
            }
            else
            {
                return result;
            }
        }
        public JSClock(IClockRenderer render,InfoManager infoManager,ClockRefreshIntervalEnum refreshInterval,string resourceFolder, params string[] scriptFolders):base(render,infoManager, refreshInterval)
        {
            manager = infoManager;
            ScriptFolders = scriptFolders;
            ResourceFolder = resourceFolder;
        }
        public override void Init()
        {
            base.Init();
            engine = new ImageSharpDrawingInstaller();
            JavaScriptHostingConfig config = new JavaScriptHostingConfig();
            config.AddPlugin(engine);
            foreach (var item in ScriptFolders)
            {
                config.AddModuleFolder(item);
            }
            engine.SetTextureRoot(ResourceFolder);
            engine.SetFontRoot(ResourceFolder);
            config.AddPlugin<ClockPluginInstaller>();
            app=JavaScriptHosting.Default.GetModuleClass<ClockApp>("app", "App", config);
            app.Init();
        }
    }
}
