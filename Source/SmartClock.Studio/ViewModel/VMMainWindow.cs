using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using SmartClock.ScriptClock.ImageSharp;
using SmartClock.Studio.Render;
using SmartClock.Studio.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SmartClock.Studio.ViewModel
{
    public class VMMainWindow : ObservableObject
    {
        ClockManager manager;
        SoftwareBitmapSourceRender imgRender;
        private ScriptClockIS currentClock;
        public VMMainWindow(ClockManager clockManager,SoftwareBitmapSourceRender render)
        {
            manager = clockManager;
            imgRender = render;
            ResultImage = imgRender.Image;
        }
        public string Text { get; set; } = "aaaaaaaa";

        public ImageSource ResultImage { get; } 
        public void RaisePropertyChanged([CallerMemberName] string? caller = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public RelayCommand RunOnce => new RelayCommand(RunOnceCommand);
        private void RunOnceCommand()
        {
            var pack=manager.LoadFromFolder("Scripts\\Test1");
            var clock=manager.BuildClock(pack, imgRender,Core.ClockRefreshIntervalEnum.OneTime);
            clock.Start();
        }

        
        private void RunCommand()
        {
            var pack = manager.LoadFromFolder("Scripts\\Test1");
            currentClock?.Stop();
            currentClock = manager.BuildClock(pack, imgRender, Core.ClockRefreshIntervalEnum.PerSecond);
            currentClock.Start();
        }
        public RelayCommand Run  => new RelayCommand(RunCommand);

        public RelayCommand Stop => new RelayCommand(() =>
        {
            currentClock?.Stop();
        });
    }
}
