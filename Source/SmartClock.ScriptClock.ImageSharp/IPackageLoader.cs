using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;

namespace SmartClock.ScriptClock.ImageSharp
{
    public interface IPackageLoader
    {
        Image<Rgba32> LoadImage(string path);
        Stream LoadStream(string path);
        string LoadText(string path);
    }
}