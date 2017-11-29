using System;
using SmartClock.Core;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
namespace SmartClock.InfoProviders.XinZhiWeatherInfoProvider
{
    public class XinzhiWeatherForcast : Core.IInfoProvider
    {
        private string serviceAPI = "https://api.seniverse.com/v3/weather/daily.json?key={0}&location={1}&language=zh-Hans&unit=c&start=0&days=5";
        public string Name => "XinzhiWeatherForcast";
        private DateTime lastrun = DateTime.MinValue;
        private string lastValue = string.Empty;
        private string city;
        private string key;
        private CancellationTokenSource cts;
        public ProviderStatusEnum Status { get; private set; }
        InfoPack result = InfoPack.NA;

        public XinzhiWeatherForcast(string key, string defaultCity)
        {
            city = defaultCity;
            this.key = key;
            Status = ProviderStatusEnum.Idle;

        }
        public InfoPack GetInfo(string arg)
        {
            return result;
        }

        public void Start()
        {
            if (string.IsNullOrEmpty(city) || string.IsNullOrEmpty(key))
            {
                return;
            }
            Stop();
            cts = new CancellationTokenSource();
            Task.Factory.StartNew(async () =>
            {
                TimeSpan delay = TimeSpan.FromMinutes(10);

                var token = cts.Token;
                try
                {
                    while (true)
                    {
                        token.ThrowIfCancellationRequested();
                        string url = string.Format(serviceAPI, key, city);
                        HttpClient client = new HttpClient();

                        string data = await client.GetStringAsync(url);
                        InfoPack tmp = new InfoPack();
                        tmp.LastUpdate = DateTime.Now;
                        tmp.Status = ProviderStatusEnum.Ready;
                        tmp.Value = data;
                        result = tmp;
#if DEBUG
                        System.Diagnostics.Debug.WriteLine($"xinzhi weather ={data}");
#endif

                        await Task.Delay(delay, token);
                    }
                }
                catch (OperationCanceledException)
                {
                    System.Diagnostics.Debug.WriteLine("Xinzhi Weather operation canceled");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }

            }, cts.Token
            );
            Status = ProviderStatusEnum.Ready;

        }

        public void Stop()
        {
            if (cts == null)
            {
                return;
            }
            cts.Cancel();
            Status = ProviderStatusEnum.Idle;
            cts = null;

        }
    }
}
