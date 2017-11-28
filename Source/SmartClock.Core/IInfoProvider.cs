using System;
using System.Collections.Generic;
using System.Text;

namespace SmartClock.Core
{
    public enum ProviderStatusEnum:int
    {
        NA=0,
        Ready=1,
        Idle=2

    }
    public struct InfoPack
    {
        public DateTime LastUpdate;
        public string Value;
        public ProviderStatusEnum Status;
        public static InfoPack NA = new InfoPack() { LastUpdate = DateTime.Now, Value = string.Empty, Status = ProviderStatusEnum.NA };
    }
    public interface IInfoProvider
    {
        InfoPack GetInfo(string arg);
        string Name { get; }
    }
}
