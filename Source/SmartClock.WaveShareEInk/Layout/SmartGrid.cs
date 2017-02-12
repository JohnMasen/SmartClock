using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SmartClock.WaveShareEInk.Layout
{
    public class SmartGrid
    {
        public Vector2 DrawingSize { get; private set; }
        public List<LineDefinition> Columns { get; private set; } 
        public List<LineDefinition> Rows { get; private set; }

        private string rowDefinition;
        private string colDefinition;
        public Dictionary<string, Tuple<int, int, int, int>> NamedRegions { get; set; } = new Dictionary<string, Tuple<int, int, int, int>>();
        EInkSpritBatch batch;
        public class LineDefinition
        {
            public LineDefinition(float start, float length)
            {
                //Index = index;
                Start = start;
                Length = length;
            }
            public float Start { get; set; }
            public float Length { get; set; }
            //public int Index { get; set; }
        }

        public void AddNamedRegion(string name,int rowIndex,int colIndex, int rowSpan,int colSpan)
        {
            NamedRegions.Add(name, new Tuple<int, int, int, int>(rowIndex, colIndex, rowSpan, colSpan));
        }

        public void DrawNamedRegion(string name,Action action)
        {
            var tmp = NamedRegions[name];
            DrawAt(tmp.Item1, tmp.Item2, tmp.Item3, tmp.Item4, action);
        }
        public void DrawGridLine(bool fillCell)
        {
            for (int row = 0; row < Rows.Count; row++)
            {
                for (int col = 0; col < Columns.Count; col++)
                {
                    DrawAt(row, col, () =>
                    {
                        if (fillCell)
                        {
                            batch.ForegroundColor = (row + col) % 2 == 0 ? EInkDevice.EInkColorEnum.Black : EInkDevice.EInkColorEnum.White;
                            batch.DrawRect(0, 0, batch.DrawingSize.X, batch.DrawingSize.Y, true);
                        }
                        else
                        {
                            batch.DrawRect(0, 0, batch.DrawingSize.X, batch.DrawingSize.Y, false);
                        }
                        
                    });
                }
            }
        }


        public SmartGrid(EInkSpritBatch batch, string columns, string rows)
        {
            this.batch = batch;
            rowDefinition = rows;
            colDefinition = columns;
            ResetGrid();
        }

        public void ResetGrid()
        {
            if (DrawingSize!=batch.DrawingSize)
            {
                DrawingSize = batch.DrawingSize;
                Columns = new List<LineDefinition>(calculateSize(colDefinition, DrawingSize.X));
                Rows = new List<LineDefinition>(calculateSize(rowDefinition, DrawingSize.Y));
            }
        }

        public SmartGrid CreateGrid(string columns,string rows)
        {
            return new SmartGrid(batch, columns, rows);
        }

        public void Project(int rowIndex,int colIndex,int rowSpan, int colSpan)
        {
            ResetGrid();
            batch.PushColor();
            batch.PushTransform();
            var region = getRegion(rowIndex, colIndex, rowSpan, colSpan);
            Debug.WriteLine($"project to {region.Item1.X},{region.Item1.Y} size:{region.Item2.X},{region.Item2.Y}");
            batch.Translate(region.Item1.X, region.Item1.Y);
            batch.DrawingSize = region.Item2;
        }

        public void DrawAt(int rowIndex, int colIndex, int rowSpan, int colSpan,Action action)
        {
            Project(rowIndex, colIndex, rowSpan, colSpan);
            action();
            UnProject();
        }

        public void DrawAt(int rowIndex, int colIndex, Action action)
        {
            DrawAt(rowIndex, colIndex, 1, 1, action);
        }
        public void UnProject()
        {
            batch.PopTransform();
            batch.PopColor();
        }


        private Tuple<Vector2, Vector2> getRegion(int rowIndex, int colIndex, int rowSpan, int colSpan)
        {
            var start = getCell(rowIndex, colIndex);
            var last = getCell(rowIndex + rowSpan - 1, colIndex + colSpan - 1);
            return new Tuple<Vector2, Vector2>(start.Item1, last.Item1+last.Item2-start.Item1);//1st item=position, 2nd item =size
        }

        private Tuple<Vector2,Vector2> getCell(int rowIndex,int colIndex)
        {
            float posX, posY, sizeX, sizeY;
            var col = Columns[colIndex];
            var row = Rows[rowIndex];
            posX = col.Start;
            sizeX = col.Length;
            posY = row.Start;
            sizeY = row.Length;
            return new Tuple<Vector2, Vector2>(new Vector2(posX, posY), new Vector2(sizeX, sizeY));
        }

        private IEnumerable<LineDefinition> calculateSize(string str, float size)
        {
            //split and trim
            var tmp1 = from item in str.Split(',')
                      select item.Trim();
            var tmp = from item in tmp1
                      select item == "*" ? "1*" : item;
            //mark the items if they're dynamic
            var items = from item in tmp
                        select new
                        {
                            isStar = item.EndsWith("*"),
                            value = float.Parse(item.Replace("*", string.Empty))
                        };
            //calculate the actual size
            float totalPercent = items.Where((x) => x.isStar).Sum((x) => x.value);
            float dynamicLength = size - items.Where((x) => !x.isStar).Sum((x) => x.value);
            float unitSize = dynamicLength / totalPercent;

            var tmp2 = from item in items
                         select 
                         item.isStar ? item.value * unitSize : item.value;
            
            //calculate position and size
            float pos = 0;
            //int index = 0;
            foreach (var item in tmp2)
            {
                yield return new LineDefinition(pos, item);
                pos += item;
                //index++;
            }
        }
    }
}
