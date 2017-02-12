using Chakra.NET;
using Chakra.NET.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace SmartClock.WaveShareEInk.JS
{
    public class EinkJSClock : EInkClockBase
    {
        string js;
        ChakraContext context;
        Action jsInit;
        Action<DateTime> jsDraw;
        ChakraRuntime runtime;
        //SpritBatchWrapper wrapper = new SpritBatchWrapper();
        public EinkJSClock(EInkDevice device, string script) : base(device)
        {
            js = script;
        }

        protected override void draw(EInkSpritBatch batch,DateTime clockTime)
        {
            //host.RunScript(js);
            jsDraw(clockTime);
            
            //System.Diagnostics.Debug.WriteLine($"JSResult={result}");
        }
        public override Task Init()
        {
            runtime = ChakraRuntime.Create();
            context = runtime.CreateContext(true);
            SpritBatchWrapper.Inject(context);
            LEDNumberWrapper.Inject(context);
            context.ValueConverter.RegisterMethodConverter<DateTime>();//for call to draw(time) in script

            context.WriteProperty<EInkSpritBatch>(context.GlobalObject, "batch", base.sb);
            context.WriteProperty<Controls.LEDNumber>(context.GlobalObject, "led",new Controls.LEDNumber(base.sb));
            var result = context.RunScript<string>(js);
            var tmp = context.GlobalObject.WithContext(context).GetField<JavaScriptValue>("Date");

            jsInit = context.ReadProperty<Action>(context.GlobalObject, "init");
            jsDraw = context.ReadProperty<Action<DateTime>>(context.GlobalObject, "draw");
            //jsDraw = jsDraw ?? context.ValueConverter.FromJSValue<Action>(context.GlobalObject);
            //jsDraw();
            jsInit?.Invoke();
            return Task.CompletedTask;
        }
    }
}
