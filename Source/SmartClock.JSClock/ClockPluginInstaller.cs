using System;
using System.Collections.Generic;
using System.Text;
using ChakraCore.NET;
using SmartClock.Core;

namespace SmartClock.JSClock
{
    class ClockPluginInstaller : ChakraCore.NET.Hosting.IPluginInstaller
    {
        private InfoManager manager;
        public ClockPluginInstaller(InfoManager manager)
        {
            this.manager = manager;
        }
        public string Name => "JSClock";

        public string GetSDK()
        {
            return Properties.Resources.sdk;
        }

        public void Install(JSValue target)
        {
            var service = target.ServiceNode.GetService<IJSValueConverterService>();
            service.RegisterStructConverter<InfoPack>(
                (jsvalue, value) =>
                {
                    jsvalue.WriteProperty<string>("value", value.Value);
                    jsvalue.WriteProperty<string>("lastUpdate", value.LastUpdate.ToString());
                    jsvalue.WriteProperty<int>("status", (int)value.Status);
                },
                (jsvalue) =>
                {
                    throw new NotSupportedException();
                }

                );
                target.Binding.SetFunction<string, string, InfoPack>("getInfo", manager.GetInfo);
            target.Binding.SetMethod<string>("Echo", (s)=>System.Diagnostics.Debug.WriteLine(s));
        }
    }
}
