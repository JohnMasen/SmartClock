using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using SmartClock.UWPRenderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace WinIoTEInk32RenderServer
{
    class ResponseData
    {
        public int status;
        public string message;
        public static ResponseData SUCCESS = new ResponseData() { status = 0, message = string.Empty };
        public static ResponseData CreateError(string message)
        {
            return new ResponseData() { status = 0, message = message };
        }
    }
    [RestController(InstanceCreationType.Singleton)]
    class HostRouter
    {
        private Eink32Device device;
        private byte[] buffer = new byte[15000];
        public HostRouter()
        {
            device = new Eink32Device();
        }

        public async Task InitAsync()
        {
            await device.InitAsync();
            await device.ResetAsync();
            
        }
        [UriFormat("/SetBuffer?pos={startPos}")]
        public IPutResponse SetBuffer(int startPos,[FromContent] byte[] data)
        {
            try
            {
                data.CopyTo(buffer, startPos);
            }
            catch (Exception ex)
            {

                return new PutResponse(PutResponse.ResponseStatus.OK, ResponseData.CreateError(ex.ToString()));
            }
            
            return new PutResponse(PutResponse.ResponseStatus.OK,ResponseData.SUCCESS);
        }
        [UriFormat("/Refresh")]
        public IGetResponse Refresh()
        {
            try
            {
                Task.Factory.StartNew(async () =>
                {
                    await device.DisplayFrameAsync(buffer);
                }).Wait();
            }
            catch (Exception ex)
            {
                return new GetResponse(GetResponse.ResponseStatus.OK, ResponseData.CreateError(ex.ToString()));
            }
            return new GetResponse(GetResponse.ResponseStatus.OK,ResponseData.SUCCESS);
            
            
        }

        [UriFormat("/Ping")]
        public IGetResponse Ping()
        {
            return new GetResponse(GetResponse.ResponseStatus.OK, ResponseData.SUCCESS);
        }
    }
}
