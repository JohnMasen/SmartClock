using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Path = System.IO.Path;

namespace SmartClock.Studio.Models
{
    public class ClockPack: ObservableObject
    {
        
        public ClockPack()
        {
            Code = String.Empty;
            Files = new ObservableCollection<string>();
        }
        private string name;
        public string Name
        {
            get => name;
            set=>SetProperty(ref name, value);
        }

        private string code;
        public string Code
        {
            get => code;
            set=>SetProperty(ref code, value);
        }

        public ObservableCollection<string> Files { get; }
        public IEnumerable<string> Images
        {
            get
            {
                return from item in Files
                       join ext in Consts.ImageFiles
                       on Path.GetExtension(item) equals ext
                       select item;
            }
        }
    }
}
