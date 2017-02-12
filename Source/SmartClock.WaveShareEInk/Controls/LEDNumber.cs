using SmartClock.WaveShareEInk.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClock.WaveShareEInk.Controls
{
    public class LEDNumber
    {
        EInkSpritBatch batch;
        SmartGrid grid;
        private Dictionary<string, string> font = new Dictionary<string, string>()
        {
            {"1","3,6" },
            {"2","1,3,4,5,7" },
            {"3","1,3,4,6,7" },
            {"4","2,4,3,6" },
            {"5","1,2,4,6,7" },
            {"6","1,2,4,5,6,7" },
            {"7","1,3,6" },
            {"8","1,2,3,4,5,6,7" },
            {"9","1,2,3,4,6,7" },
            {"0","1,2,3,5,6,7" },
            {":","d1,d2" }
        };
        public LEDNumber(EInkSpritBatch batch)
        {
            this.batch = batch;
            grid = new SmartGrid(batch,"*,8*,*", "*,6*,*,6*,*");
            grid.AddNamedRegion("1", 0, 0, 1, 3);
            grid.AddNamedRegion("2", 0, 0, 3, 1);
            grid.AddNamedRegion("3", 0, 2, 3, 1);
            grid.AddNamedRegion("4", 2, 0, 1, 3);
            grid.AddNamedRegion("5", 2, 0, 3, 1);
            grid.AddNamedRegion("6", 2, 2, 3, 1);
            grid.AddNamedRegion("7", 4, 0, 1, 3);
            grid.AddNamedRegion("d1", 1, 1, 1, 1);
            grid.AddNamedRegion("d2", 3, 1, 1, 1);
        }
        public void Draw(string number)
        {
            var size = batch.DrawingSize;
            
            batch.Clear();
            if (font.ContainsKey(number))
            {
                foreach (var item in font[number].Split(','))
                {
                    grid.DrawNamedRegion(item, () => batch.Fill(EInkDevice.EInkColorEnum.Black));
                }
            }

        }
    }
}
