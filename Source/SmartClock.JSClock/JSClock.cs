using SmartClock.Core;
using System;
using SixLabors.ImageSharp;
using JSDraw.NET;
using System.Linq;

namespace SmartClock.JSClock
{
    public class JSClock : ClockBase
    {
        public string ScriptPath { get; private set; }
        public string DrawFunctionName { get; private set; }
        public string SetupFunctionName { get; private set; }
        private string appFile;
        JSDraw.NET.JSDraw engine;
        InfoManager manager;
        protected override Image<Rgba32> drawClock()
        {
            engine.ClearObjectList();
            engine.Run(DrawFunctionName);
            return engine.GetOutput().First().Item;
        }
        public JSClock(IClockRenderer render,InfoManager infoManager,String scriptFolder,string appFile="app.js",string drawFunction="draw",string setupFunction="setup"):base(render,infoManager)
        {
            ScriptPath = scriptFolder;
            DrawFunctionName = drawFunction;
            SetupFunctionName = setupFunction;
            manager = infoManager;
            this.appFile = appFile;
        }
        public override void Init()
        {
            base.Init();
            engine = new JSDraw.NET.JSDraw();
            string scriptPath = System.IO.Path.Combine(ScriptPath, appFile);
            string script = System.IO.File.ReadAllText(scriptPath);
            engine.WorkPath = ScriptPath;
            engine.Load(script);
            engine.Context.EnableInfoManager(manager);
            engine.Run(SetupFunctionName);
        }
    }
}
