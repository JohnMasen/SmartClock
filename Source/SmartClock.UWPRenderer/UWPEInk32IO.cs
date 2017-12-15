using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartClock.Devices.WaveShareEInk42;
using Windows.Devices.Gpio;
using Windows.Devices.Spi;

namespace SmartClock.UWPRenderer
{
    public class UWPEInk32IO : IEInk32DeviceIO
    {
        GpioPin rstPin;
        GpioPin busyPin;
        GpioPin dataPin;
        SpiDevice spi;

        public bool IsDeviceBusy => busyPin.Read() == GpioPinValue.Low;

        public async Task InitAsync()
        {
            var gpioController = GpioController.GetDefault();
            rstPin = gpioController.OpenPin(17);
            rstPin.SetDriveMode(GpioPinDriveMode.Output);
            busyPin = gpioController.OpenPin(24);
            busyPin.SetDriveMode(GpioPinDriveMode.Input);
            dataPin = gpioController.OpenPin(25);
            dataPin.SetDriveMode(GpioPinDriveMode.Output);
            //tstPin = gpioController.OpenPin(18);
            var spiController = await SpiController.GetDefaultAsync();
            var spiSetting = new SpiConnectionSettings(0);
            spiSetting.ClockFrequency = 2000000;
            spiSetting.Mode = SpiMode.Mode0;
            //spiSetting.DataBitLength = 8;
            spi = spiController.GetDevice(spiSetting);
        }

        public byte[] ReadData(int size)
        {
            byte[] buffer = new byte[size];
            spi.Read(buffer);
            return buffer;
        }

        public async Task ResetAsync()
        {
            rstPin.Write(GpioPinValue.Low);
            await Task.Delay(200);
            rstPin.Write(GpioPinValue.High);
            await Task.Delay(200);
        }

        public void SendCommand(byte[] command)
        {
            dataPin.Write(GpioPinValue.Low);
            spi.Write(command);
        }

        public void SendData(byte[] data)
        {
            dataPin.Write(GpioPinValue.High);
            spi.Write(data);
        }
    }
}
