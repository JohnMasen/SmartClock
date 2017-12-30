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
        private string serviceAPI_Current = "https://api.seniverse.com/v3/weather/now.json?key={0}&location={1}&language=zh-Hans&unit=c";
        public string Name => "XinzhiWeatherForcast";
        private DateTime lastrun = DateTime.MinValue;
        private string lastValue = string.Empty;
        private string city;
        private string key;
        private CancellationTokenSource cts;
        public ProviderStatusEnum Status { get; private set; }
        InfoPack forcast = InfoPack.NA;
        InfoPack current = InfoPack.NA;

        public XinzhiWeatherForcast(string key, string defaultCity)
        {
            city = defaultCity;
            this.key = key;
            Status = ProviderStatusEnum.Idle;

        }
        public InfoPack GetInfo(string arg)
        {
            if (arg=="now")
            {
                return current;
            }
            else
            {
                return forcast;
            }
            
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
                        var data=await loadData(url);//load forcast
                        if (data.Status!=ProviderStatusEnum.NA)
                        {
                            forcast = data;
                        }
                        
                        url= string.Format(serviceAPI_Current, key, city);
                        data = await loadData(url);//load current weather
                        if (data.Status!=ProviderStatusEnum.NA)
                        {
                            current = data;
                        }
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

        private async Task<InfoPack> loadData(string url)
        {
            HttpClient client = new HttpClient();
            try
            {
                string data = await client.GetStringAsync(url);
                InfoPack tmp = new InfoPack();
                tmp.LastUpdate = DateTime.Now;
                tmp.Status = ProviderStatusEnum.Ready;
                tmp.Value = data;
                
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"xinzhi weather ={data}");
#endif
                return tmp;
            }
            catch (Exception ex)//in case read data failed, do nothing
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"xinzhi weather load error. message={ex.ToString()}");
                return InfoPack.NA;
#endif
            }
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
