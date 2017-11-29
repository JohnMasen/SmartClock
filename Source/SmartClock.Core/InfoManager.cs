using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace SmartClock.Core
{
    public class InfoManager
    {
        public List<IInfoProvider> Providers { get; private set; } = new List<IInfoProvider>();
        public InfoPack GetInfo(string providerName,string arg)
        {
            var provider=Providers.FirstOrDefault(x => x.Name == providerName);
            if (provider==null)
            {
                return InfoPack.NA;
            }
            else
            {
                return provider.GetInfo(arg);
            }
        }

    }
}
