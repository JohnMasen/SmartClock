using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SmartClock.WaveShareEInk;
using System.Threading.Tasks;
using System.Text;
using Windows.UI.Core;
using Windows.Devices.SerialCommunication;
using Windows.Devices.Enumeration;
using SmartClock.DeviceTest.Modules;
using System.Diagnostics;
using SmartClock.WaveShareEInk.JS;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SmartClock.DeviceTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        EInkDevice device;
        StringBuilder sbInput, sbOutput;
        List<DeviceInfo> ports;
        EInkClockBase t;
        SerialDevice d;
        public MainPage()
        {
            this.InitializeComponent();
            sbInput = new StringBuilder();
            sbOutput = new StringBuilder();
        }

     

        private async Task loadDevices()
        {
            try
            {
                string s = SerialDevice.GetDeviceSelector();
                var tmp = await DeviceInformation.FindAllAsync(s);
                ports = (from item in tmp
                         select new DeviceInfo() { Name = item.Name, ID = item.Id }).ToList();
            }
            catch (Exception)
            {

                throw;
            }
             
        }

        private async void btnPing_Click(object sender, RoutedEventArgs e)
        {
            await device.Ping();
            textBox1.Text = DateTime.Now.ToString() + " Ping ok";

            //var result=await SendReceiveHelper.SendReceive(d, new byte[] { 0xA5, 0x00, 0x09, 0x00, 0xCC, 0x33, 0xC3, 0x3C, 0xAC });
            //System.Diagnostics.Debug.WriteLine(BitConverter.ToString(result));
        }

        private async void btnScanPorts_Click(object sender, RoutedEventArgs e)
        {
            await loadDevices();
            lstSerialPorts.ItemsSource = ports;
        }

        private async void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            d = await SerialDevice.FromIdAsync((lstSerialPorts.SelectedValue as DeviceInfo).ID);
            //d.BaudRate = 115200;
            //d.Parity = SerialParity.None;
            //d.StopBits = SerialStopBitCount.One;
            //d.ReadTimeout = TimeSpan.FromSeconds(2);
            //d.WriteTimeout = TimeSpan.FromSeconds(2);
            //d.Handshake = SerialHandshake.None;
            device = new EInkDevice(d);
            Debug.WriteLine("device connected");
        }

        

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            t?.Stop();
            t = new TestClock(device);
            t.Start();
        }

        private void btnStopClock_Click(object sender, RoutedEventArgs e)
        {
            t?.Stop();
        }

        private void txtJS_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnJSClockRun_Click(object sender, RoutedEventArgs e)
        {
            t?.Stop();
            t = new EinkJSClock(device, txtJS.Text);
            t.Start();
        }

      
    }
}
