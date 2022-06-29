using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClock.Studio.Models
{
    public class ClockPack: ObservableObject
    {
        public ClockPack()
        {
            Code = String.Empty;
            Images = new ObservableCollection<string>();
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

        public ObservableCollection<string> Images { get; }
    }
}
