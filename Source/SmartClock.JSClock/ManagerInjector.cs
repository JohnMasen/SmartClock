using SmartClock.Core;
using System;
using System.Collections.Generic;
using System.Text;
using ChakraCore.NET;
namespace SmartClock.JSClock
{
    public static class ManagerInjector
    {
        public static void EnableInfoManager (this ChakraCore.NET.ChakraContext  context,InfoManager manager,string jsValueName="_info")
        {
            var service=context.ServiceNode.GetService<ChakraCore.NET.IJSValueConverterService>();
            service.RegisterStructConverter<InfoPack>(
                (jsvalue,value)=>
                {
                    jsvalue.WriteProperty<string>("value", value.Value);
                    jsvalue.WriteProperty<string>("lastUpdate", value.LastUpdate.ToString());
                    jsvalue.WriteProperty<int>("status", (int)value.Status);
                },
                (jsvalue)=>
                {
                    throw new NotSupportedException();
                }

                );
            service.RegisterProxyConverter<InfoManager>(
                (jsvalue, obj, node) => 
                {
                    jsvalue.SetFunction<string,string, InfoPack>("getInfo", obj.GetInfo);
                }
                );
            context.GlobalObject.WriteProperty<InfoManager>(jsValueName, manager);
            context.RunScript(Properties.Resources.InfoManager);
        }
    }
}
