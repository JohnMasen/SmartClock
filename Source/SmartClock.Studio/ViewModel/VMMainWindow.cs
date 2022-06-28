using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SmartClock.Studio.ViewModel
{
    public class VMMainWindow:INotifyPropertyChanged
    {

        public string Text { get; set; } = "aaaaaaaa";

        public SoftwareBitmapSource ResultImage { get; set; } = new SoftwareBitmapSource();
        public void RaisePropertyChanged([CallerMemberName] string? caller=null)
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(caller));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
