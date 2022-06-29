using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace SmartClock.Core
{
    public enum ClockRefreshIntervalEnum : int
    {
        PerSecond = 0,
        PerMinute = 1,
        OneTime = 99
    }

    public abstract class ClockBase
    {
        object syncRoot = new object();
        CancellationTokenSource cts;
        Task mainLoop;
        AutoResetEvent ase = new AutoResetEvent(false);
        public IClockRenderer Render { get; private set; }
        protected InfoManager info;

        public ClockRefreshIntervalEnum RefreshInterval { get; private set; }
        public ClockBase(IClockRenderer render, InfoManager infoManager, ClockRefreshIntervalEnum refreshInterval = ClockRefreshIntervalEnum.PerSecond)
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
                    await DrawAsync(token);
                    if (RefreshInterval!=ClockRefreshIntervalEnum.OneTime)
                    {
                        
                        while (!token.IsCancellationRequested)
                        {
                            int nextRefresh;
                            switch (RefreshInterval)
                            {
                                case ClockRefreshIntervalEnum.PerSecond:
                                    nextRefresh = 1000 - DateTime.Now.Millisecond;
                                    break;
                                case ClockRefreshIntervalEnum.PerMinute:
                                    nextRefresh = (60 - DateTime.Now.Second) * 1000 + (1000 - DateTime.Now.Millisecond);
                                    break;
                                default:
                                    throw new InvalidOperationException("RefreshInterval is not in valid value");
                            }
                            await Task.Delay(nextRefresh, token);
                            await DrawAsync(token);
                        }
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

        protected abstract Task<Image<Rgba32>> drawClockAsync(CancellationToken token);


        public virtual async Task DrawAsync(CancellationToken token)
        {
            await Render.RenderAsync(await drawClockAsync(token), token);
        }
        public virtual void Init()
        {
        }

        public bool IsRunning { get; private set; } = false;

    }
}
