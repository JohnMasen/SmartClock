using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Numerics;

namespace SmartClock.Core
{
    public abstract class ClockBase
    {
        bool isBusy = false;
        object syncRoot = new object();
        CancellationTokenSource cts;
        Task mainLoop;
        AutoResetEvent ase = new AutoResetEvent(false);
        public void Start()
        {
            Stop();
            cts = new CancellationTokenSource();
            mainLoop = Task.Factory.StartNew(async () =>
              {

                  System.Diagnostics.Debug.WriteLine("mainloop started");
                  CancellationToken token = cts.Token;
                  try
                  {
                      token.ThrowIfCancellationRequested();
                       Init();
                       drawFrame(null);
                      int nextSecond = 1000 - DateTime.Now.Millisecond;
                      await Task.Delay(nextSecond);
                      while (!token.IsCancellationRequested)
                      {
                           drawFrame(null);
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
        }

        protected virtual void drawFrame(object state)
        {
            //lock (syncRoot)
            //{
            //    if (isBusy)
            //    {
            //        return;
            //    }
            //    else
            //    {
            //        isBusy = true;
            //    }
            //}
            Main();
            //lock (syncRoot)
            //{
            //    isBusy = false;
            //}

        }


        public abstract void Main();
        public virtual void Init()
        {
        }


    }
}
