using System;
using SmartClock.Core;
using System.Net.Http;

namespace SmartClock.InfoProviders.XinZhiWeatherInfoProvider
{
    public class XinzhiWeatherForcast : Core.IInfoProvider
    {
        private string serviceAPI = "https://api.seniverse.com/v3/weather/daily.json?key={0}&location={1}&language=zh-Hans&unit=c&start=0&days=5";
        public string Name => "XinzhiWeatherForcast";
        string api;
        DateTime lastrun = DateTime.MinValue;
        string lastValue = string.Empty;
        string city;
        string key;
        public XinzhiWeatherForcast(string key,string defaultCity)
        {
            city = defaultCity;
            this.key = key;
            
        }
        public InfoPack GetInfo(string arg)
        {
            string result;
            if ((DateTime.Now-lastrun).Minutes<=10)
            {
                result= lastValue;
            }
            else
            {
                if (!string.IsNullOrEmpty(arg) && arg!=city)
                {
                    city = arg;
                }
                HttpClient client = new HttpClient();
                string api = string.Format(serviceAPI, key,city);
                var t = client.GetStringAsync(api);
                t.Wait();
                result = t.Result;
            }
            return new InfoPack() { LastUpdate = DateTime.Now, Status = ProviderStatusEnum.Ready, Value = result };
        }

        
    }
}
