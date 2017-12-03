using SmartClock.JSClock;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
namespace SmartClock.JSClockManager
{
    public class Manager
    {
        public Manager(string tmpFolder="tmp",string clockFolder="clocks")
        {

        }
        public List<JSClock.JSClock> Clocks { get; private set; } = new List<JSClock.JSClock>();
        public void InstallClock(System.IO.Stream source,string name)
        {
            string tmpFile = Guid.NewGuid() + ".zip";
            using (var f = File.Create(tmpFile))
            {
                source.Seek(0, SeekOrigin.Begin);
                source.CopyTo(f);
                f.Flush(true);
            }
            InstallClock(tmpFile, name);
            File.Delete(tmpFile);
        }

        public void InstallClock(string zipFileName,string name)
        {
            if (Directory.Exists(name))
            {

            }
        }
    }
}
