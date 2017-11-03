using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartClock.WaveShareEInk;
using SmartClock.Core;

namespace SmartClock.WaveShareEInk
{
    public abstract class EInkClockBase : ClockBase
    {
        protected EInkDevice device;
        protected EInkSpritBatch sb;
        protected abstract void draw(EInkSpritBatch batch,DateTime clockTime);

        //private bool isBusy=false;
        private DateTime lastRun=DateTime.MinValue;
        private System.Diagnostics.Stopwatch sw;
        public EInkClockBase(EInkDevice device)
        {
            this.device = device;
            sb = new EInkSpritBatch(device);
            sw = new System.Diagnostics.Stopwatch();
        }

        public override void Main()
        {
            //if (isBusy)
            //{
            //    return;
            //}
            //isBusy = true;
            DateTime now = DateTime.Now;
            if (lastRun==DateTime.MinValue)//first run
            {
                System.Diagnostics.Debug.WriteLine($"first run");
                createBuffer(now);
                sendToDevice();

                sb.RefreshScreen();

                createBuffer(now.AddMinutes(1));//pre draw next frame
                sendToDevice();

                lastRun = now;
            }
            else if (lastRun.Minute!=now.Minute)
            {
                sb.RefreshScreen();//refresh the screen from last "pre draw"

                createBuffer(now.AddMinutes(1));
                sendToDevice();
                lastRun = now;
            }
            //isBusy = false;
            
        }
        private void createBuffer(DateTime clockTime)
        {
            sb.Begin();
            sw.Restart();
            draw(sb, clockTime);
            sw.Stop();
            System.Diagnostics.Debug.WriteLine($"draw timing={sw.ElapsedMilliseconds}ms");
        }

        private void sendToDevice()
        {
            sw.Restart();
            sb.End();
            sw.Stop();
            System.Diagnostics.Debug.WriteLine($"update timing={sw.ElapsedMilliseconds}ms");
        }

       
    }
}
