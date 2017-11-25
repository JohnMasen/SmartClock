using SixLabors.ImageSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace EInk23Test
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        EInkDevice device ;
        string workPath = "ScriptPack\\Test";
        JSDraw.NET.JSDraw draw;
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (device == null)
            {
                device = new EInkDevice();
                await device.InitAsync();
            }
            draw = new JSDraw.NET.JSDraw();
            draw.WorkPath = workPath;
            this.txtScript.Text = loadScript("test.js");
        }

        private async void btnRun_Click(object sender, RoutedEventArgs e)
        {
            draw.Load(this.txtScript.Text);
            draw.Run();
            await renderResult(draw.GetOutput().First().Item);
            draw.ClearObjectList(true);
        }
        private string loadScript(string name)
        {
            string filepath = workPath+"\\" + name;
            return File.ReadAllText(filepath);

        }
        private async Task renderResult(SixLabors.ImageSharp.Image<Rgba32> img)
        {
            var stream = new MemoryStream();
            img.SaveAsBmp(stream);
            stream.Position = 0;
            var bii = await BitmapFactory.FromStream(stream);
            imgResult.Source = bii;
        }

        private byte[] createFromImage(Image<Rgba32> img)
        {
            var source = img.SavePixelData();
            byte[] result = new byte[source.Length / 4/8];
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
                    byte r= source[pos++];
                    byte g = source[pos++];
                    byte b = source[pos++];
                    byte a = source[pos++];//not used
                    if (!(r==0 && b==0 && g==0))
                    {
                        tmp += masks[j];
                    }
                }
                result[i] = tmp;
            }
            return result;
        }

        private async void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            draw.Load(this.txtScript.Text);
            draw.Run();
            byte[] buffer = createFromImage(draw.GetOutput().First().Item);
            draw.ClearObjectList(true);
            await device.Reset();
            await device.DisplayFrameAsync(buffer);
        }
    }
}
