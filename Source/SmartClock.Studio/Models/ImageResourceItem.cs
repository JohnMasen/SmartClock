using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClock.Studio.Models
{
    public class ImageResourceItem : ObservableObject
    {
        private string name;
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        private ImageSource image;
        public ImageSource Image
        {
            get => image;
            set => SetProperty(ref image, value);
        }

        private string path;
        public string Path
        {
            get => path;
            set => SetProperty(ref path, value);
        }
    }
}
