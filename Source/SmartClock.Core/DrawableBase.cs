using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SmartClock.Core
{
    public abstract class DrawableBase
    {
        public bool Visiable { get; set; } = true;
        protected Matrix3x2 word ;
        protected Matrix3x2 translate ;
        private Stack<Matrix3x2> translateStack = new Stack<Matrix3x2>();
        public List<DrawableBase> Children { get; private set; } = new List<DrawableBase>();

        protected struct PixelPosition
        {
            public int X;
            public int Y;
            public PixelPosition(int x, int y)
            {
                X = x;
                Y = y;
            }
        }
        internal void drawInternal(Matrix3x2 parentWorld)
        {
            if (parentWorld!=null)
            {
                word = parentWorld;
            }
            else
            {
                word = Matrix3x2.Identity;
            }
            translate = Matrix3x2.Identity;
            
            translateStack.Clear();
            if (Visiable)
            {
                Draw();
                afterDraw();
            }
        }

        internal void afterDraw()
        {
            if (translateStack.Count > 0)
            {
                throw new InvalidOperationException("Translate stack not empty, check PushTranslate and PopTranslate are paired in the Draw()");
            }
        }

        public virtual void Draw()
        {
            Matrix3x2 translated = word * translate;
            foreach (var item in Children)
            {
                item.drawInternal(translated);
            }
        }
        
        protected virtual void Translate(float x, float y)
        {
            translate = translate * Matrix3x2.CreateTranslation(x, y);
        }

        protected virtual void Scale(float x, float y)
        {
            translate = translate * Matrix3x2.CreateScale(x, y);
        }

        protected virtual void Rotate(float radians)
        {
            translate = translate * Matrix3x2.CreateRotation(radians);
        }

        protected virtual void PushTranslate()
        {
            translateStack.Push(translate);
        }

        protected virtual void PopTranslate()
        {
            translate = translateStack.Pop();
        }

        protected PixelPosition toPixel(Vector2 source) 
        {
            var tmp = Vector2.Transform(source, word * translate);
            return new PixelPosition((int)tmp.X, (int)tmp.Y);
        }

        

    }
}
