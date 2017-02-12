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
                .RegisterConverter<EInkColorEnum>(
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

            context.ValueConverter.RegisterConverter<EInkFontSizeEnum>(
            (value, helper) =>
            {
                return helper.CreateValue<byte>((byte)value);
            },
            (value, helper) =>
            {
                return (EInkFontSizeEnum)helper.ReadValue<byte>(value);
            }
            );

                context.ValueConverter.RegisterConverter<Vector2>(
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
            context.ValueConverter.RegisterConverter<EInkSpritBatch>(
                (batch, helper) =>
                {
                    return context.CreateProxyObject(batch, (v) =>
                        {
                            v.SetMethod("clear", batch.Clear);
                            v.SetMethod("pushColor", batch.PushColor);
                            v.SetMethod("popColor", batch.PopColor);
                            v.SetMethod("pushTransform", batch.PushTransform);
                            v.SetMethod("popTransform", batch.PopTransform);

                            v.SetMethod<float, float>("translate", batch.Translate);
                            v.SetMethod<float, float>("scale", batch.Scale);
                            v.SetMethod<float>("rotate", batch.Rotate);

                            v.SetMethod<float, float, float, Action>("applyBlock", batch.ApplyBlock);
                            v.SetMethod<string, string, Action>("applyGrid", batch.ApplyGrid);
                            v.SetMethod<int, int, int, int, Action>("drawCell", batch.DrawCell);


                            v.SetMethod<float, float>("drawPixel", batch.DrawPixel);
                            v.SetMethod<float, float, float, float>("drawLine", batch.DrawLine);
                            v.SetMethod<float, float, float, float, float, float, bool>("drawTriangle", batch.DrawTriangle);
                            v.SetMethod<float, float, float, float, bool>("drawRect", batch.DrawRect);
                            v.SetMethod<float, float, float, bool>("drawCircle", batch.DrawCircle);
                            v.SetMethod<float, float, string, EInkFontSizeEnum>("drawText", batch.DrawText);
                            v.SetMethod<float, float, string>("drawImage", batch.DrawImage);

                            v.SetProperty<Vector2>("DrawingSize",
                            (value) => batch.DrawingSize = value,
                            () => batch.DrawingSize);

                            v.SetProperty<EInkFontSizeEnum>("FontEN",
                            (value) => batch.FontEN = value,
                            () => batch.FontEN);

                            v.SetProperty<EInkFontSizeEnum>("FontCHN",
                            (value) => batch.FontCHN = value,
                            () => batch.FontCHN);

                            v.SetProperty<EInkColorEnum>("ForeGroud",
                            (value) => batch.ForegroundColor = value,
                            () => batch.ForegroundColor);

                            v.SetProperty<EInkColorEnum>("BackGround",
                            (value) => batch.BackgroundColor = value,
                            () => batch.BackgroundColor);

                            v.SetProperty<SmartGrid>("RootGrid",
                            null
                            , () => batch.RootGrid);
                        });
                },
                null
                );
        }
    }
}
