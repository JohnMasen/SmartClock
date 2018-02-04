using System;
using System.Collections.Generic;
using System.Text;
using ChakraCore.NET;
using SmartClock.Core;

namespace SmartClock.JSClock
{
    class ClockPluginInstaller : ChakraCore.NET.Hosting.IPluginInstaller
    {
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
            service.RegisterProxyConverter<InfoManager>(
                (jsvalue, obj, node) =>
                {
                    target.Binding.SetFunction<string, string, InfoPack>("getInfo", obj.GetInfo);
                }
                );
        }
    }
}
