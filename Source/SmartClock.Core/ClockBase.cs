using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Numerics;
using SixLabors.ImageSharp;

namespace SmartClock.Core
{
    public abstract class ClockBase
    {
        object syncRoot = new object();
        CancellationTokenSource cts;
        Task mainLoop;
        AutoResetEvent ase = new AutoResetEvent(false);
        IClockRenderer render;
        protected InfoManager info;
        public ClockBase(IClockRenderer render,InfoManager infoManager)
        {
            this.render = render ?? throw new ArgumentNullException(nameof(render));
            this.info = infoManager;
        }
        public void Start()
        {
            Stop();
            cts = new CancellationTokenSource();
            IsRunning = true;
            mainLoop = Task.Factory.StartNew(async () =>
              {

                  System.Diagnostics.Debug.WriteLine("mainloop started");
                  CancellationToken token = cts.Token;
                  try
                  {
                      token.ThrowIfCancellationRequested();
                       Init();
                       Draw();
                      int nextSecond = 1000 - DateTime.Now.Millisecond;
                      await Task.Delay(nextSecond);
                      while (!token.IsCancellationRequested)
                      {
                          Draw();
                          await Task.Delay(1000);
                      }
                      System.Diagnostics.Debug.WriteLine("mainloop stopped");
                  }
                  catch (OperationCanceledException)
                  {
                      System.Diagnostics.Debug.WriteLine("mainloop stopped at booting");
                  }
                  catch (Exception e)
                  {
                      System.Diagnostics.Debug.WriteLine($"clock failed, internal = {e.ToString()}");
                      throw new InvalidOperationException("clock failed,check inner exception for details", e);
                  }
                  finally
                  {
                      ase.Set();//release blocking
                  }
              }, cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default
            );
        }

        public void Stop()
        {
            if (cts == null)
            {
                return;
            }
            cts.Cancel();
            ase.WaitOne();//wait until mail loop stop
            cts = null;
            IsRunning = false;
        }

        protected abstract Image<Rgba32> drawClock();
        

        public virtual void Draw()
        {
            render.Render(drawClock());
        }
        public virtual void Init()
        {
        }

        public bool IsRunning { get;private set; } = false;

    }
}
