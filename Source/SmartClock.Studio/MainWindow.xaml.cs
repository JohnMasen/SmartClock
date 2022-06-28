using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;
using SmartClock.Core;
using SmartClock.ScriptClock.ImageSharp;
using SmartClock.Studio.ViewModel;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using WinRT;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SmartClock.Studio
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private async void btnGo_Click(object sender, RoutedEventArgs e)
        {
            var tmp = createZipPackage("Scripts\\Test1");
            var render = new ImageClockRender();
            InfoManager info = new InfoManager();
            ScriptClockIS clock = ScriptClockIS.Load(tmp, render, info, Core.ClockRefreshIntervalEnum.OneTime);
            clock.Start();
            while (clock.IsRunning)
            {

            }
            WriteableBitmap writeableBitmap = new WriteableBitmap(render.Image.Width, render.Image.Height);
            
            using var buffer = MemoryPool<Bgra32>.Shared.Rent(render.Image.Width * render.Image.Height);
            render.Image.CloneAs<Bgra32>().CopyPixelDataTo(buffer.Memory.Span);
            MemoryMarshal.AsBytes(buffer.Memory.Span);
            
            
            
            int bufferSize = render.Image.Width * render.Image.Height * 4;
            byte[] swapBuffer = new byte[bufferSize];
            MemoryMarshal.AsBytes(buffer.Memory.Span).Slice(0,bufferSize).CopyTo(swapBuffer);

            SoftwareBitmap bitmap = new SoftwareBitmap(BitmapPixelFormat.Bgra8, render.Image.Width, render.Image.Height,BitmapAlphaMode.Premultiplied);
            //MemoryBuffer sbuffer = new MemoryBuffer((uint)(render.Image.Width * render.Image.Height * 4));
            bitmap.CopyFromBuffer(swapBuffer.AsBuffer());

            await VMLocator.VMMainWindow.ResultImage.SetBitmapAsync(bitmap);
            VMLocator.VMMainWindow.RaisePropertyChanged(nameof(VMMainWindow.ResultImage));
            //render.Image.SaveAsJpeg("result.jpg");
            //ProcessStartInfo startInfo = new ProcessStartInfo()
            //{
            //    UseShellExecute = true,
            //    FileName = "result.jpg"
            //};

            //Process.Start(startInfo);
            //Console.WriteLine("done");
        }

        private  ZipArchive createZipPackage(string path)
        {
            MemoryStream ms = new MemoryStream();
            ZipArchive result = new ZipArchive(ms, ZipArchiveMode.Update);
            foreach (var f in Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,path)))
            {
                result.CreateEntryFromFile(f, Path.GetFileName(f));
            }
            //tmp.Dispose();
            Console.WriteLine(ms.Length);
            return result;
        }
    }
}
