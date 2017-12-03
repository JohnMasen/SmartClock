using System;
using System.Threading.Tasks;
using Unosquare.RaspberryIO.Gpio;
namespace EInkDevice.NetCore
{
    public class EInkDeviceNetCore : EInkDeviceBase
    {
        GpioPin rstPin;
        GpioPin busyPin;
        GpioPin dataPin;
        SpiChannel spi;
        public override Task InitAsync()
        {
            var gpioController = GpioController.Instance;
            rstPin = gpioController.Pin17;
            rstPin.PinMode = GpioPinDriveMode.Output;
            busyPin = gpioController.Pin24;
            busyPin.PinMode= GpioPinDriveMode.Input;
            dataPin = gpioController.Pin25;
            dataPin.PinMode = GpioPinDriveMode.Output;
            //tstPin = gpioController.OpenPin(18);
            spi = SpiBus.Instance.Channel0;
            SpiBus.Instance.Channel0Frequency = 2000000;
            //var spiSetting = new SpiConnectionSettings(0);
            //spiSetting.ClockFrequency = 2000000;
            //spiSetting.Mode = SpiMode.Mode0;
            //spiSetting.DataBitLength = 8;
            //spi = spiController.GetDevice(spiSetting);
            return Task.CompletedTask;
        }

        protected override bool getBusyStatus()
        {
            return !busyPin.Read();
        }

        protected override async Task resetAsync()
        {
            rstPin.Write(GpioPinValue.Low);
            await Task.Delay(200);
            rstPin.Write(GpioPinValue.High);
            await Task.Delay(200);
        }

        protected override void sendCommand(params byte[] data)
        {
            dataPin.Write(GpioPinValue.Low);
            spi.Write(data);
        }

        protected override void sendData(params byte[] data)
        {
            dataPin.Write(GpioPinValue.High);
            spi.Write(data);
        }
    }
}
