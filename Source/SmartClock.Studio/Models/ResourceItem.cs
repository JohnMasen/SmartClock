using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClock.Studio.Models
{
    public enum ResourceItemTypeEnum
    {
        Image,
        Font,
        Data
    }
    public class ResourceItem : ObservableObject
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

        private ResourceItemTypeEnum resourceType;
        public ResourceItemTypeEnum ResourceType
        {
            get=> resourceType;
            set=>SetProperty(ref resourceType, value);
        }

        public static ResourceItemTypeEnum ParseItemType(string path)
        {
            string ext = System.IO.Path.GetExtension(path);
            if (Consts.ImageFiles.Contains(ext))
            {
                return ResourceItemTypeEnum.Image;
            }
            else if (Consts.FontFiles.Contains(ext))
            {
                return ResourceItemTypeEnum.Font;
            }
            else
            {
                return ResourceItemTypeEnum.Data;
            }
        }
        
    }
}
