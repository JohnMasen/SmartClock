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

        public override async Task Main()
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
                await createBuffer(now);
                await sendToDevice();

                await sb.RefreshScreen();

                await createBuffer(now.AddMinutes(1));//pre draw next frame
                await sendToDevice();

                lastRun = now;
            }
            else if (lastRun.Minute!=now.Minute)
            {
                await sb.RefreshScreen();//refresh the screen from last "pre draw"

                await createBuffer(now.AddMinutes(1));
                await sendToDevice();
                lastRun = now;
            }
            //isBusy = false;
            
        }
        private async Task createBuffer(DateTime clockTime)
        {
            await sb.BeginAsync();
            sw.Restart();
            draw(sb, clockTime);
            sw.Stop();
            System.Diagnostics.Debug.WriteLine($"draw timing={sw.ElapsedMilliseconds}ms");
        }

        private async Task sendToDevice()
        {
            sw.Restart();
            await sb.EndAsync();
            sw.Stop();
            System.Diagnostics.Debug.WriteLine($"update timing={sw.ElapsedMilliseconds}ms");
        }

       
    }
}
