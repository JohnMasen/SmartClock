using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.Devices.Spi;

namespace SmartClock.UWPRenderer
{
    class Eink32Device
    {
        public static Eink32Device Default = new Eink32Device();
        public const int Width = 400;
        public const int Height = 300;
        private int frameBytes = Width * Height / 8;
        #region DeviceCommands
        public const byte PANEL_SETTING = 0x00;
        public const byte POWER_SETTING = 0x01;
        public const byte POWER_OFF = 0x02;
        public const byte POWER_OFF_SEQUENCE_SETTING = 0x03;
        public const byte POWER_ON = 0x04;
        public const byte POWER_ON_MEASURE = 0x05;
        public const byte BOOSTER_SOFT_START = 0x06;
        public const byte DEEP_SLEEP = 0x07;
        public const byte DATA_START_TRANSMISSION_1 = 0x10;
        public const byte DATA_STOP = 0x11;
        public const byte DISPLAY_REFRESH = 0x12;
        public const byte DATA_START_TRANSMISSION_2 = 0x13;
        public const byte LUT_FOR_VCOM = 0x20;
        public const byte LUT_WHITE_TO_WHITE = 0x21;
        public const byte LUT_BLACK_TO_WHITE = 0x22;
        public const byte LUT_WHITE_TO_BLACK = 0x23;
        public const byte LUT_BLACK_TO_BLACK = 0x24;
        public const byte PLL_CONTROL = 0x30;
        public const byte TEMPERATURE_SENSOR_COMMAND = 0x40;
        public const byte TEMPERATURE_SENSOR_SELECTION = 0x41;
        public const byte TEMPERATURE_SENSOR_WRITE = 0x42;
        public const byte TEMPERATURE_SENSOR_READ = 0x43;
        public const byte VCOM_AND_DATA_INTERVAL_SETTING = 0x50;
        public const byte LOW_POWER_DETECTION = 0x51;
        public const byte TCON_SETTING = 0x60;
        public const byte RESOLUTION_SETTING = 0x61;
        public const byte GSST_SETTING = 0x65;
        public const byte GET_STATUS = 0x71;
        public const byte AUTO_MEASUREMENT_VCOM = 0x80;
        public const byte READ_VCOM_VALUE = 0x81;
        public const byte VCM_DC_SETTING = 0x82;
        public const byte PARTIAL_WINDOW = 0x90;
        public const byte PARTIAL_IN = 0x91;
        public const byte PARTIAL_OUT = 0x92;
        public const byte PROGRAM_MODE = 0xA0;
        public const byte ACTIVE_PROGRAMMING = 0xA1;
        public const byte READ_OTP = 0xA2;
        public const byte POWER_SAVING = 0xE3;
        #endregion
        #region LUT
        private byte[] lut_vcom0 =
        {
        0x00, 0x17, 0x00, 0x00, 0x00, 0x02,
        0x00, 0x17, 0x17, 0x00, 0x00, 0x02,
        0x00, 0x0A, 0x01, 0x00, 0x00, 0x01,
        0x00, 0x0E, 0x0E, 0x00, 0x00, 0x02,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        };
        private byte[] lut_ww =
            {
            0x40, 0x17, 0x00, 0x00, 0x00, 0x02,
            0x90, 0x17, 0x17, 0x00, 0x00, 0x02,
            0x40, 0x0A, 0x01, 0x00, 0x00, 0x01,
            0xA0, 0x0E, 0x0E, 0x00, 0x00, 0x02,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            };
        private byte[] lut_bw =
            {
            0x40, 0x17, 0x00, 0x00, 0x00, 0x02,
            0x90, 0x17, 0x17, 0x00, 0x00, 0x02,
            0x40, 0x0A, 0x01, 0x00, 0x00, 0x01,
            0xA0, 0x0E, 0x0E, 0x00, 0x00, 0x02,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            };

        private byte[] lut_bb =
            {
            0x80, 0x17, 0x00, 0x00, 0x00, 0x02,
            0x90, 0x17, 0x17, 0x00, 0x00, 0x02,
            0x80, 0x0A, 0x01, 0x00, 0x00, 0x01,
            0x50, 0x0E, 0x0E, 0x00, 0x00, 0x02,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            };

        private byte[] lut_wb =
            {
            0x80, 0x17, 0x00, 0x00, 0x00, 0x02,
            0x90, 0x17, 0x17, 0x00, 0x00, 0x02,
            0x80, 0x0A, 0x01, 0x00, 0x00, 0x01,
            0x50, 0x0E, 0x0E, 0x00, 0x00, 0x02,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            };
        #endregion

        GpioPin rstPin;
        GpioPin busyPin;
        GpioPin dataPin;
        SpiDevice spi;
        private Eink32Device()
        {

        }
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

        public async Task ResetAsync()
        {
            await reset();
            sendCommand(POWER_SETTING);
            sendData(0x03);                  // VDS_EN, VDG_EN
            sendData(0x00);                  // VCOM_HV, VGHL_LV[1], VGHL_LV[0]
            sendData(0x2b);                  // VDH
            sendData(0x2b);                  // VDL
            sendData(0xff);                  // VDHR
            sendCommand(BOOSTER_SOFT_START);
            sendData(0x17);
            sendData(0x17);
            sendData(0x17);                  //07 0f 17 1f 27 2F 37 2f
            sendCommand(POWER_ON);
            waitDevice();
            sendCommand(PANEL_SETTING);
            sendData(0xbf);    // KW-BF   KWR-AF  BWROTP 0f
            sendData(0x0b);
            sendCommand(PLL_CONTROL);
            sendData(0x3c);
        }

        public async Task DisplayFrameAsync(byte[] frameBuffer)
        {
            setResolution();

            sendCommand(VCM_DC_SETTING);
            sendData(0x12);

            sendCommand(VCOM_AND_DATA_INTERVAL_SETTING);
            sendCommand(0x97);    //VBDF 17|D7 VBDW 97  VBDB 57  VBDF F7  VBDW 77  VBDB 37  VBDR B7

            sendEmptyFrame1();
            await Task.Delay(2);

            sendCommand(DATA_START_TRANSMISSION_2);
            sendData(frameBuffer);
            await Task.Delay(2);

            setLUT();

            sendCommand(DISPLAY_REFRESH);
            await Task.Delay(100);
            waitDevice();
        }

        private void setLUT()
        {
            sendCommand(LUT_FOR_VCOM);
            sendData(lut_vcom0);
            sendCommand(LUT_WHITE_TO_WHITE);
            sendData(lut_ww);
            sendCommand(LUT_BLACK_TO_WHITE);
            sendData(lut_bw);
            sendCommand(LUT_WHITE_TO_BLACK);
            sendData(lut_wb);
            sendCommand(LUT_BLACK_TO_BLACK);
            sendData(lut_bb);
        }

        public bool IsBusy => busyPin.Read() == GpioPinValue.Low;

        private void waitDevice(int millisecondsTimeout = 1000)
        {
            while (IsBusy)
            {
                Task.Delay(100).Wait(millisecondsTimeout);
            }
            if (IsBusy)
            {
                throw new InvalidOperationException("Wait for device busy timed out");
            }
        }

        private void setResolution()
        {
            sendCommand(RESOLUTION_SETTING);
            var w = BitConverter.GetBytes(Width);
            var h = BitConverter.GetBytes(Height);
            sendData(w[1], w[0], h[1], h[0]);
        }

        private void sendEmptyFrame1()
        {
            sendCommand(DATA_START_TRANSMISSION_1);
            for (int i = 0; i < frameBytes; i++)
            {
                sendData(0xff);
            }
        }

        private async Task reset()
        {
            rstPin.Write(GpioPinValue.Low);
            await Task.Delay(200);
            rstPin.Write(GpioPinValue.High);
            await Task.Delay(200);
        }

        private void sendCommand(params byte[] cmd)
        {
            dataPin.Write(GpioPinValue.Low);
            spi.Write(cmd);
        }

        private void sendData(params byte[] cmd)
        {
            dataPin.Write(GpioPinValue.High);
            spi.Write(cmd);
        }

        public async Task Sleep()
        {
            System.Diagnostics.Debug.WriteLine("begin sleep");
            sendCommand(VCOM_AND_DATA_INTERVAL_SETTING);
            sendData(0x17);                       //border floating    
            sendCommand(VCM_DC_SETTING);          //VCOM to 0V
            sendCommand(PANEL_SETTING);
            await Task.Delay(100);

            sendCommand(POWER_SETTING);           //VG&VS to 0V fast
            sendData(0x00);
            sendData(0x00);
            sendData(0x00);
            sendData(0x00);
            sendData(0x00);
            await Task.Delay(100);

            sendCommand(POWER_OFF);          //power off
            waitDevice();
            sendCommand(DEEP_SLEEP);         //deep sleep
            sendData(0xA5);
            System.Diagnostics.Debug.WriteLine("end sleep");
        }
    }

    static class EInk32DeviceHelper
    {
        internal static async Task DisplayARGB32ByteBufferAsync(this Eink32Device device, byte[] buffer)
        {
            if (buffer.Length % 32 !=0)
            {
                throw new ArgumentException("buffer length must be multiply of 32");
            }
            byte[] result = new byte[buffer.Length / 32];
            int pos = 0;
            byte tmp = 0;
            byte[] masks =
            {
                0x80, //1000 0000
                0x40, //0100 0000
                0x20, //0010 0000
                0x10, //0001 0000
                0x08, //0000 1000
                0x04, //0000 0100
                0x02, //0000 0010
                0x01, //0000 0001
            };
            for (int i = 0; i < result.Length; i++)
            {
                tmp = 0;
                for (int j = 0; j < 8; j++)
                {
                    byte r = buffer[pos++];
                    byte g = buffer[pos++];
                    byte b = buffer[pos++];
                    byte a = buffer[pos++];//not used
                    if (!(r == 0 && b == 0 && g == 0))
                    {
                        tmp += masks[j];
                    }
                }
                result[i] = tmp;
            }
            await device.DisplayFrameAsync(result);
        }
        
    }
}
