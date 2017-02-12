using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartClock.WaveShareEInk;
using SmartClock.WaveShareEInk.Layout;
using SmartClock.WaveShareEInk.Controls;

namespace SmartClock.DeviceTest
{
    class TestClock : SmartClock.WaveShareEInk.EInkClockBase
    {
        DateTime time;
        public TestClock(EInkDevice device) : base(device)
        {
        }

        

        protected override void draw(EInkSpritBatch batch,DateTime clockTime)
        {
            time = clockTime;
            batch.Clear();
            //drawChessboard(batch);

            //performanceTest1(batch);
            testLEDTime(batch);
            //test2(batch);
        }

        private void testLEDTime(EInkSpritBatch batch)
        {
            LEDNumber led= new LEDNumber(batch);
            string s = time.ToString("hhmm");
            batch.ApplyGrid("30*,3*,30*,3*,10*,3*,30*,3*,30*", "*",
                ()=>
                {
                    batch.DrawCell(0, 0, 1, 1, ()=>led.Draw(s.Substring(0,1)));
                    batch.DrawCell(0, 2, 1, 1, () => led.Draw(s.Substring(1, 1)));
                    batch.PushColor();
                    batch.ForegroundColor = EInkDevice.EInkColorEnum.LightGrey;
                    batch.DrawCell(0, 4, 1, 1, () => led.Draw(":"));
                    batch.PopColor();
                    batch.DrawCell(0, 6, 1, 1, () => led.Draw(s.Substring(2, 1)));
                    batch.DrawCell(0, 8, 1, 1, () => led.Draw(s.Substring(3, 1)));
                }
                );
        }
        
        private void testGrid(EInkSpritBatch batch)
        {
            SmartGrid grid = new SmartGrid(batch, "100,2*,*,*,*,*,*,*", "100,2*,*,*,*,*,*,*");
            grid.AddNamedRegion("content", 4, 2, 3, 3);
            grid.DrawGridLine(false);
            grid.DrawAt(0, 0,2,5, () =>
            {
                //sb.ForegroundColor = (row + col) % 2 == 0 ? EInkDevice.EInkColorEnum.Black : EInkDevice.EInkColorEnum.White;
                batch.DrawRect(0, 0, batch.DrawingSize.X, batch.DrawingSize.Y, true);
            });
            grid.DrawNamedRegion("content", () =>
             {
                 batch.Clear();
                 //sb.ForegroundColor = EInkDevice.EInkColorEnum.White;
                 //sb.DrawRect(0, 0, sb.DrawingSize.X, sb.DrawingSize.Y, true);
             }
            );
        }

        private void testLEDNumber(EInkSpritBatch batch)
        {
            string cellWidth = "8*";
            string cellHeight = "8*";
            string cellGap = "*";
            int cols = 6;
            int rows = 2;
            generateGridString(cellWidth, cellHeight, cellGap, cols, rows, out string sbc, out string sbr);

            SmartGrid grid = batch.RootGrid.CreateGrid(sbc, sbr);
            int num = 0;
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    grid.DrawAt(row * 2 + 1, col * 2 + 1,
                        () =>
                        {
                            if (num > 9)
                            {
                                new LEDNumber(batch).Draw(":");
                            }
                            else
                            {
                                new LEDNumber(batch).Draw(num.ToString());
                            }
                            num++;
                        }
                        );
                }
            }
            grid.DrawGridLine(false);
        }

        private void testLEDNumber1(EInkSpritBatch batch)
        {
            string cellWidth = "8*";
            string cellHeight = "8*";
            string cellGap = "*";
            int cols = 6;
            int rows = 2;
            generateGridString(cellWidth, cellHeight, cellGap, cols, rows, out string sbc, out string sbr);
            var led = new LEDNumber(batch);
            batch.ApplyGrid(sbc, sbr, ()=>
            {
                int num = 0;
                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < cols; col++)
                    {
                        batch.DrawCell(row * 2 + 1, col * 2 + 1,1,1,
                            () =>
                            {
                                if (num > 9)
                                {
                                    led.Draw(":");
                                }
                                else
                                {
                                    led.Draw(num.ToString());
                                }
                                num++;
                            }
                            );
                    }
                }
            });
            
            //grid.DrawGridLine(false);
        }

        private void generateGridString(string cellWidth, string cellHeight, string cellGap, int cols, int rows, out string colString, out string rowString)
        {
            var sbc = new StringBuilder();
            var sbr = new StringBuilder();
            sbc.Append(cellGap);

            for (int i = 0; i < cols; i++)
            {
                sbc.Append(",");
                sbc.Append(cellWidth);
                sbc.Append(",");
                sbc.Append(cellGap);
            }

            sbr.Append(cellGap);

            for (int i = 0; i < rows; i++)
            {
                sbr.Append(",");
                sbr.Append(cellHeight);
                sbr.Append(",");
                sbr.Append(cellGap);
            }
            colString = sbc.ToString();
            rowString = sbr.ToString();
        }

        //private void testMatrix1(EInkSpritBatch batch)
        //{
        //    MatrixDisplay m = new MatrixDisplay();
        //    m.Columns.AddRange(new float[] { 100, 100, 100, 100, 100,100 });
        //    m.Rows.AddRange(new float[] { 100, 100, 100 });
        //    m.DrawCell = (b, row, column,size) =>
        //      {
        //          bool value = (row+ column) %2==0;
        //          //b.ForegroundColor = value ? EInkDevice.EInkColorEnum.Black : EInkDevice.EInkColorEnum.White;
        //          b.DrawRect(0, 0, size.X, size.Y,value);
        //      };
        //    m.Draw(batch);
        //}

        private void testDrawLED(EInkSpritBatch batch)
        {
            LEDDisplay test = new LEDDisplay(13, 7, 20,5);
            bool v = false;
            for (int y = 0; y < test.Height; y++)
            {
                for (int x = 0; x < test.Width; x++)
                {
                    test.SetItem(x, y, v);
                    v = !v;
                }
                
            }

            //test.DrawCell = (sb,size,value) =>
            //  {
            //      size = size / 2;
            //      sb.ForegroundColor = value ? EInkDevice.EInkColorEnum.Black : EInkDevice.EInkColorEnum.White;
            //      sb.DrawCircle(size.X, size.Y, size.X, true);
            //  };
            batch.ApplyBlock(100, 0, 2f, () =>
            {
                //batch.Rotate(180 / Math.PI * 15);
                test.Draw(batch);
            });

        }
    
        //private void drawChessboard(EInkSpritBatch batch)
        //{
        //    batch.PushColor();
        //    batch.PushTransform();
        //    batch.BackgroundColor = EInkDevice.EInkColorEnum.White;
        //    batch.ForegroundColor = EInkDevice.EInkColorEnum.Black;
        //    int sizeX = 50;
        //    int sizeY = 50;

        //    batch.PushTransform();
        //    for (int x = 0; x < 800/ sizeX; x++)
        //    {
                
        //        for (int y = 0; y < 600/sizeY; y++)
        //        {
                    
        //            batch.DrawRect(0, 0, sizeX, sizeY, (x + y) % 2 == 0);
        //            batch.Translate(0, sizeY);
        //        }
        //        batch.PopTransform();
        //        batch.Translate(sizeX, 0);
        //        batch.PushTransform();
                
        //    }
        //    batch.PopTransform();
        //    batch.PopColor();
        //}

        private void test1(EInkSpritBatch batch)
        {
            batch.DrawText(80, 0, DateTime.Now.ToString(), EInkDevice.EInkFontSizeEnum.Size64);
            batch.DrawImage(400, 200, "PIC3.BMP");
            batch.DrawText(80, 100, "It works!", EInkDevice.EInkFontSizeEnum.Size64);
            batch.DrawText(80, 200, "On my laptop!!", EInkDevice.EInkFontSizeEnum.Size48);
            batch.DrawText(80, 260, "Last night!!!", EInkDevice.EInkFontSizeEnum.Size32);
        }
        private void performanceTest1(EInkSpritBatch batch)
        {
            for (int i = 0; i < 300; i++)
            {
                batch.DrawRect(0, 0, 10, 10);
            }
        }


        private void testTranslation(EInkSpritBatch batch)
        {

            basicDraw(batch,20);
            batch.ApplyBlock(100, 0, 1, () =>
            {
                basicDraw(batch, 20);
            });

            batch.ApplyBlock(100, 100, 1, () =>
            {
                batch.Rotate(Math.PI / 180 * 10);
                basicDraw(batch, 20);
            });

            batch.ApplyBlock(100, 0, 2, () =>
               {
                   basicDraw(batch, 20);
               });
            batch.ApplyBlock(0, 0, 2, () =>
            {
                batch.ApplyBlock(100, 0, 3, () =>
                {
                    basicDraw(batch, 20);
                });
            });
        }

        private void basicDraw(EInkSpritBatch batch,float size)
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    batch.DrawRect(i * size, j* size, size, size);
                }
                
            }
            

        }
    }
}
