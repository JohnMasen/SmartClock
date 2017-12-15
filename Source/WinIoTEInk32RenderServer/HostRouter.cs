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
using static WinIoTEInk32RenderServer.RendererHost;
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

    class Capability
    {
        public string Host = nameof(WinIoTEInk32RenderServer);
        public int TransferBufferSize = 15000;
    }

    [RestController(InstanceCreationType.Singleton)]
    class HostRouter
    {
        private byte[] buffer = new byte[15000];
        private object syncRoot = new object();

        
        [UriFormat("/SetBuffer?pos={startPos}")]
        public IPutResponse SetBuffer(int startPos,[FromContent] string data)
        {
            try
            {
                //using (System.IO.MemoryStream ms=new System.IO.MemoryStream(data))
                //{
                //    using (System.IO.StreamReader reader=new System.IO.StreamReader(ms))
                //    {
                //        string tmp = reader.ReadToEnd();
                        var bytes=Convert.FromBase64String(data);
                System.Diagnostics.Debug.WriteLine(bytes.Length);
                        bytes.CopyTo(buffer, startPos);
                //    }
                //}
            }
            catch (Exception ex)
            {
                return new PutResponse(PutResponse.ResponseStatus.OK,ResponseData.CreateError(ex.ToString()));
            }

            return new PutResponse(PutResponse.ResponseStatus.OK,ResponseData.SUCCESS);
        }
        [UriFormat("/Refresh")]
        public IGetResponse Refresh()
        {
            try
            {
                lock (syncRoot)
                {
                    Renderer.RenderRawBuffer(buffer).Wait();
                }
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

        [UriFormat("/Capability")]
        public IGetResponse GetCapability()
        {
            return new GetResponse(GetResponse.ResponseStatus.OK, new Capability());
        }
    }
}
