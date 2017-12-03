using SmartClock.Core;
using System;
using SixLabors.ImageSharp;
using JSDraw.NET;
using System.Linq;

namespace SmartClock.JSClock
{
    public class JSClock : ClockBase
    {
        public string ScriptFolder { get; private set; }
        public string DrawFunctionName { get; private set; }
        public string SetupFunctionName { get; private set; }
        public string AppFile { get; private set; }
        JSDraw.NET.JSDraw engine;
        InfoManager manager;
        protected override Image<Rgba32> drawClock()
        {
            engine.ClearObjectList();
            engine.Run(DrawFunctionName);
            var result = engine.GetOutput().FirstOrDefault();
            if (result==null)
            {
                return null;
            }
            else
            {
                return result.Item;
            }
        }
        public JSClock(IClockRenderer render,InfoManager infoManager,String scriptFolder,ClockRefreshIntervalEnumn refreshInterval, string appFile="app.js",string drawFunction="draw",string setupFunction="setup"):base(render,infoManager, refreshInterval)
        {
            ScriptFolder = scriptFolder;
            DrawFunctionName = drawFunction;
            SetupFunctionName = setupFunction;
            manager = infoManager;
            this.AppFile = appFile;
        }
        public override void Init()
        {
            base.Init();
            engine = new JSDraw.NET.JSDraw();
            string scriptPath = System.IO.Path.Combine(ScriptFolder, AppFile);
            string script = System.IO.File.ReadAllText(scriptPath);
            engine.WorkPath = ScriptFolder;
            engine.Load(script);
            engine.Context.EnableInfoManager(manager);
            engine.Run(SetupFunctionName);
        }
    }
}
