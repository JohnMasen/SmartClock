using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SmartClock.Core;
using System.Threading;
using Newtonsoft.Json;
using SixLabors.ImageSharp.Dithering;

namespace SmartClock.NetcoreRenderer
{
    public class RemoteRenderer : IClockRenderer
    {
        private class ServerResponse
        {
            public int status { get; set; }
            public string message { get; set; }
        }

        class Capability
        {
            public string Host { get; set; }
            public int TransferBufferSize { get; set; }
        }
        public RenderInfo Info => new RenderInfo() { Name = nameof(RemoteRenderer), Version = "1.0.0" };
        private string serverAddress;
        System.Net.Http.HttpClient client;
        public int ServerBufferSize { get; private set; } = 0;
        public bool IsPreProcessEnabled { get; set; } = true;
        public float DitherThreshold { get; set; } = 0.5f;
        FloydSteinbergDiffuser dither = new FloydSteinbergDiffuser();
        public RemoteRenderer(string serverName)
        {
            this.serverAddress = $"http://{serverName}:8800/api";
        }

        public async Task Connect()
        {
            client = new System.Net.Http.HttpClient();
            var result = await serverGet<Capability>("/Capability");
            ServerBufferSize = result.TransferBufferSize;
        }

        private async Task<T> serverGet<T>(string apiAddress)
        {
            string address = serverAddress + apiAddress;
            var result = await client.GetAsync(address);
            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new InvalidOperationException($"GET from {address} failed with status code {result.StatusCode}");
            }
            var content = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }

        private async Task<T> serverPut<T>(string apiAddress, string content)
        {
            string address = serverAddress + apiAddress;
            System.Net.Http.StringContent c = new System.Net.Http.StringContent(content);
            c.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var result = await client.PutAsync(address, c);
            var resultContent = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(resultContent);
        }
        public async Task RenderAsync(Image<Rgba32> image, CancellationToken token)
        {
            StringBuilder sb = new StringBuilder();
            int pos = 0;
            Image<Rgba32> target;
            if (IsPreProcessEnabled)
            {
                target = image.Clone(ctx =>
                  {
                      ctx.Grayscale(SixLabors.ImageSharp.Processing.GrayscaleMode.Bt601);
                      ctx.Dither(dither, DitherThreshold);
                  });
            }
            else
            {
                target = image;
            }
            var buffer = convertToDeviceBuffer(target.SavePixelData());
            while (pos < buffer.Length)
            {
                int bytesLeft = buffer.Length - pos;
                int sendBufferSize = Math.Min(bytesLeft, ServerBufferSize);
                //byte[] sendBuffer = new byte[sendBufferSize];
                sb.Append("\"");
                sb.Append(Convert.ToBase64String(buffer, pos, sendBufferSize));
                sb.Append("\"");
                System.Diagnostics.Debug.WriteLine($"set buffer pos={pos} size= {sendBufferSize}");
                ServerResponse response = await serverPut<ServerResponse>($"/SetBuffer?pos={pos}", sb.ToString());
                if (response.status != 0)
                {
                    throw new InvalidOperationException($"Send buffer to server failed,message={response.message}");
                }
                pos += ServerBufferSize;
            }
            var result = await serverGet<ServerResponse>("/Refresh");
            if (result.status != 0)
            {
                throw new InvalidOperationException($"call server refresh failed with message {result.message}");
            }


        }

        private byte[] convertToDeviceBuffer(byte[] argb)
        {
            if (argb.Length % 32 != 0)
            {
                throw new ArgumentException("buffer length must be multiply of 32");
            }
            byte[] result = new byte[argb.Length / 32];
            int pos = 0;
            byte tmp = 0;
            byte[] masks =
            {
                0x80, //1000 0000
                0x40, //0100 0000
                0x20, //0010 0000
                0x10, //0001 0000
                0x08, //0000 1000
                0x04, //0000 0100
                0x02, //0000 0010
                0x01, //0000 0001
            };
            for (int i = 0; i < result.Length; i++)
            {
                tmp = 0;
                for (int j = 0; j < 8; j++)
                {
                    byte r = argb[pos++];
                    byte g = argb[pos++];
                    byte b = argb[pos++];
                    byte a = argb[pos++];//not used
                    if (!(r == 0 && b == 0 && g == 0))
                    {
                        tmp += masks[j];
                    }
                }
                result[i] = tmp;
            }
            return result;
        }
    }
}
