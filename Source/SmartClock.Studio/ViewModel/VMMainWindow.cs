using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using SmartClock.Core;
using SmartClock.ScriptClock.ImageSharp;
using SmartClock.Studio.Models;
using SmartClock.Studio.Render;
using SmartClock.Studio.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            loadClockPack("Scripts\\Test1");
        }
        private string scriptCode;
        public string ScriptCode 
        {
            get => scriptCode;
            set=>SetProperty(ref scriptCode, value);
        } 

        public ImageSource ResultImage { get; }

        private ClockPack currentClockPack;
        public ClockPack CurrentClockPack
        {
            get => currentClockPack;
            set => SetProperty(ref currentClockPack, value);
        }

        public ObservableCollection<ImageResourceItem> ImageResources { get; } = new ObservableCollection<ImageResourceItem>();

        private void loadClockPack(string path)
        {
            CurrentClockPack = manager.LoadFromFolder(path);
            ScriptCode = CurrentClockPack.Code;
            ImageResources.Clear();
            foreach (var item in CurrentClockPack.Images)
            {
                ImageResources.Add(new ImageResourceItem()
                {
                    Name = Path.GetFileName(item),
                    Image = new BitmapImage(new Uri(item)),
                    Path=item
                }); ;
            }
        }

        private ScriptClockIS buildClock(ClockRefreshIntervalEnum interval)
        {
            CurrentClockPack.Code = ScriptCode;
            CurrentClockPack.Images.Clear();
            foreach (var item in ImageResources)
            {
                currentClockPack.Images.Add(item.Path);
            }
            return manager.BuildClock(CurrentClockPack, imgRender, interval);
        }


        public RelayCommand RunOnce => new RelayCommand(()=>
        {
            
            var clock = buildClock(ClockRefreshIntervalEnum.OneTime);
            clock.Start();
        });
        
        
        public RelayCommand Run  => new RelayCommand(()=>
        {
            currentClock?.Stop();
            currentClock = buildClock(ClockRefreshIntervalEnum.PerSecond);
            currentClock.Start();
        });

        public RelayCommand Stop => new RelayCommand(() =>
        {
            currentClock?.Stop();
        });
    }
}
