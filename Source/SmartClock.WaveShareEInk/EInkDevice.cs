using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SmartClock.WaveShareEInk
{
    public class EInkDevice
    {
        public enum EInkColorEnum : byte
        {
            Black = 0x00,
            DarkGrey = 0x01,
            LightGrey = 0x02,
            White = 0x03
        };

        public enum EInkFontSizeEnum : byte
        {
            Size32 = 0x01,
            Size48 = 0x02,
            Size64 = 0x03
        }

        public enum StorageAreaEnum : byte
        {
            NAND = 0x00,
            Flash = 0x01
        }

        public enum ScreenOrientationEnum : byte
        {
            Normal = 0x00,
            Rotate180 = 0x01
        }

        public struct ColorSetting
        {
            public EInkColorEnum Foreground;
            public EInkColorEnum Background;
        }

        private SerialPort serial;

        public Vector2 DeviceSize { get; } = new Vector2(800, 600);

        //TODO: should avoid direct reference hardware device, will cause conflict. use logical IO instead
        public EInkDevice(SerialPort serial)
        {
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            this.serial = serial;
            var magic = serial.BaudRate; //HACK:the LoadAsync will hang until I found this magic word(for my Prolific USB2Serial device)

            serial.ReadTimeout = 1;
            serial.WriteTimeout = 1;
            serial.Handshake = Handshake.None;
            serial.BaudRate = 115200;
            serial.DataBits = 8;
            serial.StopBits = StopBits.One;
            serial.Parity = Parity.None;
        }

        public  byte[] Execute(EInkCommand command, bool checkIfOKReturned)
        {
            byte[] cmdResult = new byte[command.ResultLength];

            var data = command.GetCommandBytes();
            serial.Write(data, 0, data.Length);
            if (serial.BytesToRead!=command.ResultLength)
            {
                throw new InvalidOperationException($"Command result length mismatch, command={command.CommandType}");
            }
            serial.Read(cmdResult, 0, command.ResultLength);
            if (checkIfOKReturned && !cmdResult.SequenceEqual(EInkCommand.COMMAND_OK))
            {
                throw new InvalidOperationException($"Invalid command result, command={command.CommandType}");
            }
            return cmdResult;
            //using (DataReader reader = new DataReader(serial.InputStream))
            //{
            //    using (DataWriter writer = new DataWriter(serial.OutputStream))
            //    {
            //            writer.WriteBytes(command.GetCommandBytes());
            //            await writer.StoreAsync();
            //            await reader.LoadAsync(1024);
            //            if (reader.UnconsumedBufferLength != command.ResultLength)
            //            {
            //                throw new InvalidOperationException($"Command result length mismatch, command={command.CommandType}");
            //            }
            //            reader.ReadBytes(cmdResult);
            //            if (checkIfOKReturned && !cmdResult.SequenceEqual(EInkCommand.COMMAND_OK))
            //            {
            //                throw new InvalidOperationException($"Invalid command result, command={command.CommandType}");
            //            }

            //        writer.DetachStream();
            //        reader.DetachStream();
            //        return cmdResult;
            //    }
            //}

        }

        public void ExecuteBatchAsync(IEnumerable<EInkCommand> commands)
        {
            byte[] cmdResult = new byte[2];
#if DEBUG
            List<Tuple<EInkCommand, int>> perfcounter = new List<Tuple<EInkCommand, int>>();
            Stopwatch sw = new Stopwatch();
#endif


            foreach (var item in commands)
            {
#if DEBUG
            
                sw.Restart();
#endif
                Execute(item, true);
#if DEBUG
                sw.Stop();
                perfcounter.Add(new Tuple<EInkCommand, int>(item, (int)sw.ElapsedMilliseconds));
#endif
            }
#if DEBUG
            int line = 1;
            Debug.WriteLine("sequence,command,duration,raw");
            foreach (var item in perfcounter)
            {
                Debug.WriteLine($"{line++},{BitConverter.ToString(new byte[] { item.Item1.CommandType })},{item.Item2},{BitConverter.ToString(item.Item1.GetCommandBytes()).Replace("-", " ")}");
            }
            Debug.WriteLine($"total {perfcounter.Count} commands");
#endif

//            using (DataReader reader=new DataReader(serial.InputStream))
//            {
//                using (DataWriter writer=new DataWriter(serial.OutputStream))
//                {
//                    foreach (var item in commands)
//                    {

//                        writer.WriteBytes(item.GetCommandBytes());
//#if DEBUG
//                        sw.Restart();
//#endif
//                        await writer.StoreAsync();
//                        await reader.LoadAsync(1024);
//#if DEBUG
//                        sw.Stop();
//                        perfcounter.Add(new Tuple<EInkCommand, int>(item, (int)sw.ElapsedMilliseconds));
//#endif
//                        if (reader.UnconsumedBufferLength!=2)
//                        {
//                            throw new InvalidOperationException($"Command execution error, command={item.CommandType}");
//                        }
//                        reader.ReadBytes(cmdResult);
//                        if (!cmdResult.SequenceEqual(EInkCommand.COMMAND_OK))
//                        {
//                            throw new InvalidOperationException($"Invalid command result, command={item.CommandType}");
//                        }
                        
//                    }
//                    writer.DetachStream();
//                    reader.DetachStream();
//#if DEBUG
//                    int line = 1;
//                    Debug.WriteLine("sequence,command,duration,raw");
//                    foreach (var item in perfcounter)
//                    {
//                        Debug.WriteLine($"{line++},{BitConverter.ToString(new byte[] { item.Item1.CommandType })},{item.Item2},{BitConverter.ToString(item.Item1.GetCommandBytes()).Replace("-"," ")}");
//                    }
//                    Debug.WriteLine($"total {perfcounter.Count} commands");
//#endif
//                }
//            }
        }

        #region Control API
        public void Ping()
        {
            runCommandInternal(0x00);
        }

        public void SetStorageArea(StorageAreaEnum value)
        {
            runCommandInternal(0x07, (byte)value);
        }

        public StorageAreaEnum GetStorageArea()
        {
            byte[] tmp =  runCommandInternal(0x06, true, 1);
            return (StorageAreaEnum)tmp[0];
        }

        public void Sleep()
        {
             runCommandInternal(0x08);
        }

        public ScreenOrientationEnum GetScreenOrientation()
        {
            byte[] result =  runCommandInternal(0x0C, true, 1);
            return (ScreenOrientationEnum)result[0];
        }

        public void SetScreenOrientation(ScreenOrientationEnum value)
        {
            runCommandInternal(0x0D, (byte)value);
        }
        public void LoadFont()
        {
            EInkCommand cmd = new EInkCommand(0x0E);
            Execute(cmd, true);
        }

        public void LoadImage()
        {
            EInkCommand cmd = new EInkCommand(0x0F);
            Execute(cmd, true);
        }


        public void Refresh()
        {
            runCommandInternal(0x0A);
        }
        #endregion

        #region Drawing Config API
        public void SetDrawingColor(EInkColorEnum foreground, EInkColorEnum background)
        {
            runCommandInternal(0x10, (byte)foreground, (byte)background);
        }

        public ColorSetting GetDrawingColor()
        {
            byte[] result = runCommandInternal(0x11, true, 2);
            return new ColorSetting()
            {
                Foreground = (EInkColorEnum)result[0] - 0x30,
                Background = (EInkColorEnum)result[1] - 0x30
            };
        }

        public void SetFontSizeEnglish(EInkFontSizeEnum size)
        {
            runCommandInternal(0x1E, (byte)size);
        }

        public void SetFontSizeChinese(EInkFontSizeEnum size)
        {
            runCommandInternal(0x1F, (byte)size);
        }


        public EInkFontSizeEnum GetFontSizeEnglish()
        {
            byte[] tmp = runCommandInternal(0x1C, true, 1);
            return (EInkFontSizeEnum)tmp[0];
        }

        public EInkFontSizeEnum GetFontSizeChinese()
        {
            byte[] tmp = runCommandInternal(0x1D, true, 1);
            return (EInkFontSizeEnum)tmp[0];
        }
        #endregion

        #region internal call wrapper
        private byte[] runCommandInternal(byte command, bool hasResult = false, int resultLength = 2)
        {
            EInkCommand cmd = new EInkCommand(command, resultLength);
            return  Execute(cmd, !hasResult);
        }

        private void runCommandInternal(byte command, params byte[] para)
        {
            EInkCommand cmd = new EInkCommand(command, para);
             Execute(cmd, true);
        }

        private void runCommandInternal(byte command, params int[] para)
        {
            EInkCommand cmd = new EInkCommand(command, para);
             Execute(cmd, true);
        }

        private void runCommandInternal(byte command, int x, int y, string content)
        {
            EInkCommand cmd = new EInkCommand(command, x, y, content);
             Execute(cmd, true);
        }
        #endregion

        private short resultToShort(byte[] data)
        {
            //if (BitConverter.IsLittleEndian)
            //{
            //    Array.Reverse(data);
            //}
            return BitConverter.ToInt16(data, 0);
        }

    }
}
