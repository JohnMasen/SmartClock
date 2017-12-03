using SmartClock.JSClock;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
namespace SmartClock.JSClockManager
{
    public class Manager
    {
        public string TempFolder { get; private set; } = "tmp";
        public string ClockScriptFolder { get; private set; } = "clocks";

        public Dictionary<string, JSClock.JSClock> Clocks { get; private set; } = new Dictionary<string, JSClock.JSClock>();
        public void InstallClockScript(Stream source,string name, bool updateIfRunning = true)
        {
            string tmpFile = Path.Combine(TempFolder, Guid.NewGuid() + ".zip");
            using (var f = File.Create(tmpFile))
            {
                source.Seek(0, SeekOrigin.Begin);
                source.CopyTo(f);
                f.Flush(true);
            }
            try
            {
                InstallClockScript(tmpFile, name,updateIfRunning);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                File.Delete(tmpFile);
            }
        }

        public void InstallClockScript(string zipFileName,string name,bool updateIfRunning=true)
        {
            string scriptFolder = Path.Combine(ClockScriptFolder, name);
            var runningClock = Clocks.Values.FirstOrDefault(x => x.ScriptFolder == scriptFolder && x.IsRunning);
            if (runningClock != null)
            {
                if (updateIfRunning)
                {
                    runningClock.Stop();
                }
                else
                {
                    throw new InvalidOperationException("Script is used by a running clock, stop the clock first");
                }
                
            }
            if (Directory.Exists(name))
            {
                Directory.Delete(name, true);
            }
            ZipFile.ExtractToDirectory(zipFileName, scriptFolder);
            if (runningClock!=null)
            {
                runningClock.Init();
                runningClock.Start();
            }
        }


        public IEnumerable<ClockScriptInfo> InstalledClockScripts()
        {
            return from item in Directory.GetDirectories(ClockScriptFolder)
                   select new ClockScriptInfo() { Name = item };
        }
    }
}
