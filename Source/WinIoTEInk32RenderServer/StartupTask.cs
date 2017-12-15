using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using Restup.Webserver.Rest;
using Restup.Webserver.Http;
using Restup.Webserver.File;
using SmartClock.UWPRenderer;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.Fonts;
using SixLabors.Primitives;
using Windows.Networking.Connectivity;
using Windows.Networking;
using System.Threading.Tasks;
using static WinIoTEInk32RenderServer.RendererHost;
// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace WinIoTEInk32RenderServer
{
    public sealed class StartupTask : IBackgroundTask
    {
        BackgroundTaskDeferral deferral;
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // 
            // TODO: Insert code to perform background work
            //
            // If you start any asynchronous methods here, prevent the task
            // from closing prematurely by using BackgroundTaskDeferral as
            // described in http://aka.ms/backgroundtaskdeferral
            //
            deferral = taskInstance.GetDeferral();
            var restRouteHandler = new RestRouteHandler();
            restRouteHandler.RegisterController<HostRouter>();
            var configuration = new HttpServerConfiguration()
              .ListenOnPort(8800)
              .RegisterRoute("api", restRouteHandler)
              .RegisterRoute(new StaticFileRouteHandler("WebRoot"))
              .EnableCors();
            var httpServer = new HttpServer(configuration);
            await httpServer.StartServerAsync();
            await DrawWelcomeScreen();
        }

        private async Task DrawWelcomeScreen()
        {
            Image<Rgba32> image = new Image<Rgba32>(400, 300);
            FontCollection fc = new FontCollection();
            fc.Install("DigitalDream.ttf");
            var f = fc.CreateFont("Digital Dream", 18);
            GraphicsOptions options = new GraphicsOptions(false);
            image.Mutate((ctx) =>
                {
                    ctx.Fill(Rgba32.White);
                    ctx.DrawText($"{GetCurrentIpv4Address()}:8800", f, Rgba32.Black, new PointF(0, 0),options);
                    ctx.DrawText($"{DateTime.Now.ToString()}", f, Rgba32.Black, new PointF(0, 100),options);
                    ctx.Dither(new SixLabors.ImageSharp.Dithering.FloydSteinbergDiffuser(),0.5f);
                });
            await Renderer.RenderAsync(image,Task.Factory.CancellationToken);
        }

        private static string GetCurrentIpv4Address()

        {
            try
            {
                var icp = NetworkInformation.GetInternetConnectionProfile();
                if (icp != null && icp.NetworkAdapter != null && icp.NetworkAdapter.NetworkAdapterId != null)
                {
                    var name = icp.ProfileName;
                    var hostnames = NetworkInformation.GetHostNames();
                    foreach (var hn in hostnames)
                    {
                        if (hn.IPInformation != null &&
                            hn.IPInformation.NetworkAdapter != null &&
                            hn.IPInformation.NetworkAdapter.NetworkAdapterId != null &&
                            hn.IPInformation.NetworkAdapter.NetworkAdapterId == icp.NetworkAdapter.NetworkAdapterId &&
                            hn.Type == HostNameType.Ipv4)
                        {
                            return hn.CanonicalName;
                        }
                    }
                }
            }
            catch (Exception)
            {
                // do nothing
                // in some (strange) cases NetworkInformation.GetHostNames() fails... maybe a bug in the API...
            }
            return "NoInternetConnection";

        }
    }
}
