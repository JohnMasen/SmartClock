using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartClock.Devices.WaveShareEInk42
{
    public interface IEInk32DeviceIO
    {
        void SendCommand(byte[] command);
        void SendData(byte[] data);
        byte[] ReadData(int size);
        bool IsDeviceBusy { get; }
        Task ResetAsync();
        Task InitAsync();
    }
}
