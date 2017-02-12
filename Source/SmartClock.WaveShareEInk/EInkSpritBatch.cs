using SmartClock.WaveShareEInk.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static SmartClock.WaveShareEInk.EInkDevice;

namespace SmartClock.WaveShareEInk
{
    public class EInkSpritBatch
    {
        EInkDevice device;
        bool isBegun = false;
        List<EInkCommand> commandBatch;
        private EInkDevice.EInkColorEnum _backgroundColor;
        //private Matrix3x2 world;
        private Stack<Tuple<Matrix3x2,float,Vector2>> transformStack;
        private Stack<EInkDevice.ColorSetting> colorStack;
        private Matrix3x2 transform;
        private float scale;
        public Vector2 DeviceSize { get; private set; }

        public Vector2 DrawingSize { get; set; }

        public EInkDevice.EInkColorEnum BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                if (_backgroundColor != value)
                {
                    addCommand(new EInkCommand(0x10, (byte)ForegroundColor, (byte)value));
                }
                _backgroundColor = value;
            }
        }

        private EInkDevice.EInkColorEnum _foregroundColor;

        public EInkDevice.EInkColorEnum ForegroundColor
        {
            get { return _foregroundColor; }
            set
            {
                if (_foregroundColor != value)
                {
                    addCommand(new EInkCommand(0x10, (byte)value, (byte)BackgroundColor));
                }
                _foregroundColor = value;
            }
        }

        private EInkFontSizeEnum _fontEN;

        public EInkFontSizeEnum FontEN
        {
            get { return _fontEN; }
            set
            {
                if (_fontEN != value)
                {
                    addCommand(new EInkCommand(0x1e, (byte)value));
                    _fontEN = value;
                }

            }
        }

        private EInkFontSizeEnum _fontCHN;

        public EInkFontSizeEnum FontCHN
        {
            get { return _fontCHN; }
            set
            {
                if (_fontCHN != value)
                {
                    addCommand(new EInkCommand(0x1F, (byte)value));
                    _fontCHN = value;
                }
            }
        }

        public Layout.SmartGrid RootGrid { get; private set; }

        private SmartGrid currentGrid;
        private struct DrawingPosition
        {
            public int X;
            public int Y;
            public DrawingPosition(int x, int y)
            {
                X = x;
                Y = y;
            }

            public DrawingPosition(Vector2 v)
            {
                X = (int)v.X;
                Y = (int)v.Y;
            }

        }
        public EInkSpritBatch(EInkDevice device)
        {
            this.device = device;
            commandBatch = new List<EInkCommand>();
            DeviceSize = device.DeviceSize;
            RootGrid = new Layout.SmartGrid(this, "*", "*");
        }
        public async Task BeginAsync()
        {
            if (isBegun)
            {
                throw new InvalidOperationException("Please call EndAsync() before you can start any operation");
            }
            isBegun = true;
            
            commandBatch.Clear();
            //load config
            var tmp = await device.GetDrawingColor();
            _foregroundColor = tmp.Foreground;
            _backgroundColor = tmp.Background;

            _fontEN = await device.GetFontSizeEnglish();
            _fontCHN = await device.GetFontSizeChinese();

            //reset all
            ResetWorld();
            ResetColor();
        }

        

        public async Task EndAsync()
        {
            if (!isBegun)
            {
                throw new InvalidOperationException("Please call BeginAsync() first");
            }
            if (transformStack.Count>0)
            {
                throw new InvalidOperationException("PushTransform and Poptransform not paired");
            }
            System.Diagnostics.Debug.WriteLine($"command queue size={commandBatch.Count}");
            await device.ExecuteBatchAsync(commandBatch);
            commandBatch.Clear();
            isBegun = false;
        }

        internal Task RefreshScreen()
        {
            return device.Refresh();
        }
        

        public void ResetWorld()
        {
            //world = Matrix3x2.Identity;
            transformStack = new Stack<Tuple<Matrix3x2, float,Vector2>>();
            transform = Matrix3x2.Identity;
            scale = 1;
            DrawingSize = DeviceSize;
        }

        public void ResetColor()
        {
            colorStack = new Stack<EInkDevice.ColorSetting>();
            BackgroundColor = EInkDevice.EInkColorEnum.White;
            ForegroundColor = EInkDevice.EInkColorEnum.Black;
        }

        public void PushColor()
        {
            colorStack.Push(new EInkDevice.ColorSetting() { Background = BackgroundColor, Foreground = ForegroundColor });
        }

        public void PopColor()
        {
            var tmp = colorStack.Pop();
            BackgroundColor = tmp.Background;
            ForegroundColor = tmp.Foreground;
        }

        public void Translate(float x, float y)
        {
            transform = Matrix3x2.CreateTranslation(x, y) * transform;
        }

        public void Scale(float scale)
        {
            transform = Matrix3x2.CreateScale(scale) * transform;
            this.scale *= scale;
        }
        public void Scale(float xScale,float yScale)
        {
            transform = Matrix3x2.CreateScale(xScale, yScale) * transform;
            this.scale *= Math.Min(xScale,yScale);
        }

        public void Rotate(float radians)
        {
            transform = Matrix3x2.CreateRotation(radians) * transform;
        }
        public void Rotate(double radians)
        {
            transform = Matrix3x2.CreateRotation((float)radians) * transform;
        }
        public void PushTransform()
        {
            transformStack.Push(new Tuple<Matrix3x2, float,Vector2>(transform, scale,DrawingSize));
        }


        public void ApplyBlock(float x,float y,float scale, Action a)
        {
            PushColor();
            PushTransform();
            Translate(x, y);
            Scale(scale);
            a();
            PopTransform();
            PopColor();
        }

        public void ApplyBlock(float x, float y, Action a)
        {
            ApplyBlock(x, y, 1, a);
        }

        public void PopTransform()
        {
            var tmp = transformStack.Pop();
            transform = tmp.Item1;
            scale = tmp.Item2;
            DrawingSize = tmp.Item3;
        }

        

        #region Drawing API
        public void DrawRect(float x1, float y1, float width, float height, bool fill = false)
        {
            if (width==0||height==0)
            {
                return;
            }
            else
            {
                width--;
                height--;
            }
            Vector2 topLeft = calculateXY(x1, y1);
            Vector2 topRight = calculateXY(x1+width, y1);
            Vector2 bottomLeft = calculateXY(x1, y1+height);
            Vector2 bottomRight = calculateXY(x1 + width, y1 + height);
            if (topLeft.Y==topRight.Y)
            {
                drawRectInternal(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y, fill);//no rotation, use fast API
                return;
            }
            if (fill)
            {
                drawTriangleInternal(topLeft.X, topLeft.Y, topRight.X, topRight.Y, bottomLeft.X, bottomLeft.Y, true);
                drawTriangleInternal(bottomRight.X, bottomRight.Y, topRight.X, topRight.Y, bottomLeft.X, bottomLeft.Y, true);
            }
            else
            {
                drawLineInternal(topLeft.X, topLeft.Y, topRight.X, topRight.Y);
                drawLineInternal(topLeft.X, topLeft.Y, bottomLeft.X, bottomLeft.Y);
                drawLineInternal(bottomRight.X, bottomRight.Y, topRight.X, topRight.Y);
                drawLineInternal(bottomRight.X, bottomRight.Y, bottomLeft.X, bottomLeft.Y);
            }

        }

        private void drawRectInternal(float x1, float y1, float x2, float y2, bool fill = false)
        {
            if (checkIfLessThanZero(x1,y1,x2,y2))
            {
                System.Diagnostics.Debug.WriteLine($"Invalid DrawRectFast call,position should not less than 0. [{x1},{y1}],[{x2},{y2}]");
                return;
            }
            if (fill)
            {
                addCommand(new EInkCommand(0x24, x1, y1, x2, y2));
            }
            else
            {
                addCommand(new EInkCommand(0x25, x1, y1, x2, y2));
            }



        }

        public void DrawLine(float x1, float y1, float x2, float y2)
        {
            var p1 = calculateXY(x1, y1);
            var p2 = calculateXY(x2, y2);
            if (p1.X < 0 || p1.Y < 0 || p2.X < 0 || p2.Y < 0)
            {
                
            }
            drawLineInternal(p1.X, p1.Y, p2.X, p2.Y);
            
        }

        private void drawLineInternal(float x1, float y1, float x2, float y2)
        {
            if (checkIfLessThanZero(x1,y1,x2,y2))
            {
                System.Diagnostics.Debug.WriteLine($"Invalid DrawLine call,from {x1},{y1} to {x2},{y2}");
                return;
            }
            addCommand(new EInkCommand(0x22, x1, y1, x2, y2));
        }

        private bool checkIfLessThanZero(params float[] values)
        {
            foreach (var item in values)
            {
                if (item<0)
                {
                    return true;
                }
            }
            return false;
        }


        public void DrawText(float x, float y, string content, EInkFontSizeEnum fontSize)
        {
            FontCHN = fontSize;
            FontEN = fontSize;
            var p = calculateXY(x, y);
            addCommand(new EInkCommand(0x30, p.X, p.Y, content));
        }


        public void DrawImage(float x, float y, string filename)
        {
            var p = calculateXY(x, y);
            addCommand(new EInkCommand(0x70, p.X, p.Y, filename));
        }

        internal void Fill(byte color)
        {
            Fill((EInkColorEnum)color);
        }
        public void Fill(EInkColorEnum color)
        {
            PushColor();
            ForegroundColor = color;
            DrawRect(0, 0, DrawingSize.X, DrawingSize.Y, true);
            PopColor();
        }

        public void Clear()
        {
            if (DrawingSize!=DeviceSize)//drawing size changed, clear drawing region
            {
                Fill(BackgroundColor);
            }
            else //hardware clear
            {
                addCommand(new EInkCommand(0x2E));
            }
            
        }


        public void DrawCircle(float x, float y, float r, bool isFill = false)
        {
            var p = calculateXY(x, y);
            if (checkIfLessThanZero(p.X,p.Y))
            {
                System.Diagnostics.Debug.WriteLine($"Invalid DrawCircle call,position should not less than 0. [{p.X},{p.Y}]");
                return;
            }
            if (isFill)
            {
                addCommand(new EInkCommand(0x27, p.X, p.Y, r*scale));
            }
            else
            {
                addCommand(new EInkCommand(0x26, p.X, p.Y, r * scale));
            }

        }

        public void DrawTriangle(float x1, float y1, float x2, float y2, float x3, float y3, bool isFill = false)
        {
            var p1 = calculateXY(x1, y1);
            var p2 = calculateXY(x2, y2);
            var p3 = calculateXY(x3, y3);
            if (checkIfLessThanZero(x1,y1,x2,y2,x3,y3))
            {
                System.Diagnostics.Debug.WriteLine($"Invalid DrawTriangle call,position should not less than 0. [{p1.X},{p1.Y}],[{p2.X},{p2.Y}],[{p3.X},{p3.Y}]");
                return;
            }
            drawTriangleInternal(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y, isFill);
        }

        private void drawTriangleInternal(float x1, float y1, float x2, float y2, float x3, float y3, bool isFill )
        {
            if (isFill)
            {
                addCommand(new EInkCommand(0x29, x1, y1, x2, y2, x3, y3));
            }
            else
            {
                addCommand(new EInkCommand(0x28, x1, y1, x2, y2, x3, y3));
            }
        }

        public void DrawPixel(float x, float y)
        {
            var p = calculateXY(x, y);
            if (checkIfLessThanZero(p.X,p.Y))
            {
                System.Diagnostics.Debug.WriteLine($"Invalid DrawPixel call,position should not less than 0. [{p.X},{p.Y}]");
                return;
            }
            addCommand(new EInkCommand(0x20, p.X, p.Y));
        }
        #endregion


        private Vector2 calculateXY(Vector2 source)
        {
            return Vector2.Transform(source, transform);
        }

        private Vector2 calculateXY(float x, float y)
        {
            return calculateXY(new Vector2(x, y));
        }
        private void addCommand(EInkCommand command)
        {
            commandBatch.Add(command);
        }

        
        public void ApplyGrid(string columns,string rows,Action callback)
        {
            var previousGrid = currentGrid;
            currentGrid = new SmartGrid(this, columns, rows);
            callback();
            currentGrid = previousGrid;
        }

        public void DrawCell(int rowIndex,int colIndex,int rowSpan,int colSpan,Action callback)
        {
            if (currentGrid==null)
            {
                throw new InvalidOperationException("DrawCell can only be called inside ApplyGrid() callback");
            }
            currentGrid.DrawAt(rowIndex, colIndex, rowSpan, colSpan, callback);
        }

    }
}
