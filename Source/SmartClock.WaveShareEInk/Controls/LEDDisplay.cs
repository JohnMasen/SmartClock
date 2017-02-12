using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static SmartClock.WaveShareEInk.EInkDevice;

namespace SmartClock.WaveShareEInk.Controls
{
    public class LEDDisplay
    {
        public int Height { get; private set; }
        public int Width { get; private set; }

        public class LEDItem
        {
            public Vector2 Position;
            public Vector2 Size;
            public bool Value;
        }
        public int UnitSize { get; private set; }
        public int ItemGap { get; private set; }
        public LEDItem[,] Items { get; private set; }
        private Vector2 drawingSize;

        private EInkColorEnum background;

        public Action<EInkSpritBatch, EInkColorEnum, Vector2> DrawBackground = (x, c, s) =>
                {
                    x.ForegroundColor = c;
                    x.DrawRect(0, 0, s.X, s.Y, true);
                };
        public Action<EInkSpritBatch, Vector2, bool> DrawCell = (x, v, b) =>
            {
                if (b)
                {
                    x.ForegroundColor = EInkColorEnum.Black;
                }
                else
                {
                    x.ForegroundColor = EInkColorEnum.White;
                }
                x.DrawRect(0, 0, v.X, v.Y, true);
            };
        public LEDDisplay(int width, int height, int unitSize, int gap, EInkColorEnum backgroundColor = EInkColorEnum.LightGrey)
        {
            Height = height;
            Width = width;
            UnitSize = unitSize;
            ItemGap = gap;
            drawingSize = new Vector2(width * (unitSize + gap) + gap, height * (unitSize + gap) + gap);
            initItems();
            background = backgroundColor;
        }
        public void SetItem(int x, int y, bool value)
        {
            Items[x, y].Value = value;
        }

        public LEDItem GetItem(int x, int y)
        {
            return Items[x, y];
        }

        private void initItems()
        {
            Items = new LEDItem[Width, Height];
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Items[x, y] = new LEDItem() { Position = new Vector2(x * (UnitSize + ItemGap), y * (UnitSize + ItemGap)), Size = new Vector2(UnitSize), Value = false };
                }
            }
        }
        public void Draw(EInkSpritBatch batch)
        {

            batch.ApplyBlock(0, 0, ()=>DrawBackground(batch, background, drawingSize));


            int blockSize = UnitSize + ItemGap;
            batch.ApplyBlock(ItemGap, ItemGap, () =>
              {
                  for (int y = 0; y < Height; y++)
                  {
                      for (int x = 0; x < Width; x++)
                      {
                          var item = GetItem(x, y);
                          batch.ApplyBlock(x * blockSize, y * blockSize,()=>
                          {
                              DrawCell(batch, item.Size, item.Value);
                          });
                      }
                  }
              }
            );

        }

    }
}
