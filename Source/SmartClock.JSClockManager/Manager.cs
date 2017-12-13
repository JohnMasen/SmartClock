using SmartClock.Core;
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
        public string ClockScriptFolder { get; private set; } = "Clocks";
        public InfoManager InfoManager { get; private set; } = new InfoManager();
        public IEnumerable<(string Name, ClockInfo Container)> DefinedClocks => from item in items select (item.Key,item.Value);
        private Dictionary<string, ClockInfo> items = new Dictionary<string, ClockInfo>();
        public void InstallClockScript(Stream source,string name, bool updateIfRunning = true)
        {
            string tmpFile =  Guid.NewGuid() + ".zip";
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

            //check if the folder is used by running clock
            var runningClock = DefinedClocks.FirstOrDefault(x => x.Container.ScriptFolder == scriptFolder && x.Container.Clock?.IsRunning==true).ToTuple();
            if (runningClock.Item1 != null)
            {
                if (updateIfRunning)
                {
                    StopClock(runningClock.Item1);
                }
                else
                {
                    throw new InvalidOperationException("Script is used by a running clock, stop the clock first");
                }
            }

            //make sure the target folder exists and empty
            if (Directory.Exists(scriptFolder))
            {
                Directory.Delete(scriptFolder, true);
            }
            Directory.CreateDirectory(scriptFolder);
            

            //extract to target folder
            ZipFile.ExtractToDirectory(zipFileName, scriptFolder);

            //restart the clock which was using the script
            if (runningClock.Item1!=null)
            {
                StartClock(runningClock.Item1, scriptFolder);
            }
        }


        public IEnumerable<ClockScriptInfo> InstalledClockScripts
        {
            get
            {
                DirectoryInfo info = new DirectoryInfo(ClockScriptFolder);
                return from item in info.GetDirectories()
                select new ClockScriptInfo() { Name = item.Name, FolderName = item.Name };
            }
        }

        public void AddDefinition(string name,IClockRenderer renderer,ClockRefreshIntervalEnum refreshInterval)
        {
            items.Add(name, new ClockInfo(renderer, refreshInterval));
        }

        public void StartClock(string name,string scriptFolder)
        {
            scriptFolder = Path.Combine(ClockScriptFolder, scriptFolder);
            var info = items[name];
            info.Clock?.Stop();
            info.Clock = new JSClock.JSClock(info.Renderer, InfoManager, scriptFolder,info.RefreshInterval);
            info.Clock.Init();
            info.Clock.Start();
            info.ScriptFolder = scriptFolder;
        }

        public void StopClock(string name)
        {
            items[name].Clock?.Stop();
        }
    }
}
