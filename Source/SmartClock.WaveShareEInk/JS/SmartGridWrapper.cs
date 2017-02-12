
using Chakra.NET;
using Chakra.NET.API;
using SmartClock.WaveShareEInk.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClock.WaveShareEInk.JS
{
    class SmartGridWrapper
    {
        public static void Inject(ChakraContext context)
        {
            context.ValueConverter.RegisterConverter<SmartGrid.LineDefinition>(
                (value, helper) =>
                {
                    var result = JavaScriptValue.CreateObject();  //marshal by value, no need to create external object
                    result.WithContext(context)
                    .SetField<float>("start", value.Start)
                    .SetField<float>("length", value.Length)
                    ;
                    return result;
                },
                null
                );
                context.ValueConverter.ReigsterArrayConverter<SmartGrid.LineDefinition>()
                ;

            context.ValueConverter.RegisterConverter<SmartGrid>(
                (grid, helper) =>
                {
                  
                        return helper.CreateProxyObject<SmartGrid>(grid,
                        (v) =>
                        {
                            v
                            .SetMethod<bool>("drawGridLine", grid.DrawGridLine)
                            .SetMethod<int, int, int, int, Action>("drawAt", grid.DrawAt)
                            .SetFunction<string, string, SmartGrid>("createGrid", grid.CreateGrid)
                            .SetMethod<string, int, int, int, int>("addRegion", grid.AddNamedRegion)
                            .SetMethod<string, Action>("drawRegion", grid.DrawNamedRegion)
                            .SetProperty<IEnumerable<SmartGrid.LineDefinition>>("Columns",
                            null,
                            () => grid.Columns
                            )
                            .SetProperty<IEnumerable<SmartGrid.LineDefinition>>("Rows",
                            null,
                            () => grid.Rows
                            )
                            ;
                        }
                                                );


                }
            , null);

        }
    }
}
