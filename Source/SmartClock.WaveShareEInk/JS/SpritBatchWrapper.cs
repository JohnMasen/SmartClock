using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static SmartClock.WaveShareEInk.EInkDevice;
using System.Numerics;


using SmartClock.WaveShareEInk.Layout;
using Chakra.NET;
using Chakra.NET.API;

namespace SmartClock.WaveShareEInk.JS
{
    public class SpritBatchWrapper
    {
        public static void Inject(ChakraContext context)
        {
            context.ValueConverter
                .RegisterValueConverter<EInkColorEnum>(
                (value, helper) =>
                {
                    using (helper.With())
                    {
                        return helper.CreateValue<byte>((byte)value);
                    }

                },
                (value, helper) =>
                {
                    return (EInkColorEnum)helper.ReadValue<byte>(value);
                }
                );

            context.ValueConverter.RegisterValueConverter<EInkFontSizeEnum>(
            (value, helper) =>
            {
                return helper.CreateValue<byte>((byte)value);
            },
            (value, helper) =>
            {
                return (EInkFontSizeEnum)helper.ReadValue<byte>(value);
            }
            );

            context.ValueConverter.RegisterValueConverter<Vector2>(
            (value, helper) =>
            {
                JavaScriptValue result = helper.CreateObject();
                helper.WriteProperty<float>(result, "x", value.X);
                helper.WriteProperty<float>(result, "y", value.Y);
                return result;
            },
            (value, helper) =>
            {
                return new Vector2(
                    helper.ReadProperty<float>(value, "x"),
                    helper.ReadProperty<float>(value, "y")
                    );
            }
            )


        ;
            context.ValueConverter.RegisterProxyConverter<EInkSpritBatch>(
                (output, source) =>
                {

                    output.SetMethod("clear", source.Clear);
                    output.SetMethod("pushColor", source.PushColor);
                    output.SetMethod("popColor", source.PopColor);
                    output.SetMethod("pushTransform", source.PushTransform);
                    output.SetMethod("popTransform", source.PopTransform);

                    output.SetMethod<float, float>("translate", source.Translate);
                    output.SetMethod<float, float>("scale", source.Scale);
                    output.SetMethod<float>("rotate", source.Rotate);

                    output.SetMethod<float, float, float, Action>("applyBlock", source.ApplyBlock);
                    output.SetMethod<string, string, Action>("applyGrid", source.ApplyGrid);
                    output.SetMethod<int, int, int, int, Action>("drawCell", source.DrawCell);


                    output.SetMethod<float, float>("drawPixel", source.DrawPixel);
                    output.SetMethod<float, float, float, float>("drawLine", source.DrawLine);
                    output.SetMethod<float, float, float, float, float, float, bool>("drawTriangle", source.DrawTriangle);
                    output.SetMethod<float, float, float, float, bool>("drawRect", source.DrawRect);
                    output.SetMethod<float, float, float, bool>("drawCircle", source.DrawCircle);
                    output.SetMethod<float, float, string, EInkFontSizeEnum>("drawText", source.DrawText);
                    output.SetMethod<float, float, string>("drawImage", source.DrawImage);

                    output.SetProperty<Vector2>("DrawingSize",
                    (value) => source.DrawingSize = value,
                    () => source.DrawingSize);

                    output.SetProperty<EInkFontSizeEnum>("FontEN",
                    (value) => source.FontEN = value,
                    () => source.FontEN);

                    output.SetProperty<EInkFontSizeEnum>("FontCHN",
                    (value) => source.FontCHN = value,
                    () => source.FontCHN);

                    output.SetProperty<EInkColorEnum>("ForeGroud",
                    (value) => source.ForegroundColor = value,
                    () => source.ForegroundColor);

                    output.SetProperty<EInkColorEnum>("BackGround",
                    (value) => source.BackgroundColor = value,
                    () => source.BackgroundColor);

                    //v.SetProperty<SmartGrid>("RootGrid",
                    //null
                    //, () => batch.RootGrid);
                }
                );

        }
    }
}
