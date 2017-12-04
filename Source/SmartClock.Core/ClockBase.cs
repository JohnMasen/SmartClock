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
    public enum ClockRefreshIntervalEnumn : int
    {
        PerSecond = 0,
        PerMinute = 1
    }

    public abstract class ClockBase
    {
        object syncRoot = new object();
        CancellationTokenSource cts;
        Task mainLoop;
        AutoResetEvent ase = new AutoResetEvent(false);
        public IClockRenderer Render { get; private set; }
        protected InfoManager info;

        public ClockRefreshIntervalEnumn RefreshInterval { get; private set; }
        public ClockBase(IClockRenderer render, InfoManager infoManager, ClockRefreshIntervalEnumn refreshInterval = ClockRefreshIntervalEnumn.PerSecond)
        {
            this.Render = render ?? throw new ArgumentNullException(nameof(render));
            this.info = infoManager;
            RefreshInterval = refreshInterval;
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
                    await DrawAsync();
                    //int nextSecond = 1000 - DateTime.Now.Millisecond;
                    //await Task.Delay(nextSecond);
                    int nextRefresh;
                    switch (RefreshInterval)
                    {
                        case ClockRefreshIntervalEnumn.PerSecond:
                            nextRefresh = 1000 - DateTime.Now.Millisecond;
                            break;
                        case ClockRefreshIntervalEnumn.PerMinute:
                            nextRefresh = (60 - DateTime.Now.Second) * 1000 + (1000 - DateTime.Now.Millisecond);
                            break;
                        default:
                            throw new InvalidOperationException("RefreshInterval is not in valid value");
                    }
                    while (!token.IsCancellationRequested)
                    {
                        await Task.Delay(nextRefresh, token);
                        await DrawAsync();
                    }
                    System.Diagnostics.Debug.WriteLine("mainloop stopped");
                }
                catch (OperationCanceledException)
                {
                    System.Diagnostics.Debug.WriteLine("mainloop stopped");
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine($"clock failed, internal = {e.ToString()}");
                    throw new InvalidOperationException("clock failed,check inner exception for details", e);
                }
                finally
                {
                    ase.Set();//release blocking
                    cts = null;
                    IsRunning = false;
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
        }

        protected abstract Image<Rgba32> drawClock();


        public virtual async Task DrawAsync()
        {
            await Render.RenderAsync(drawClock());
        }
        public virtual void Init()
        {
        }

        public bool IsRunning { get; private set; } = false;

    }
}
