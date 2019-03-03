using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace EduardoBotv2.Core.Helpers
{
    public static class NetworkHelper
    {
        private static readonly HttpClient _client = new HttpClient(new HttpClientHandler
        {
            AutomaticDecompression = (DecompressionMethods) 0xFF
        });

        public static async Task<Stream> GetStream(string url, Dictionary<string, string> headers = null)
        {
            HttpResponseMessage response = await _client.SendAsync(BuildRequest(url, HttpMethod.Get, headers));
            return await response.Content.ReadAsStreamAsync();
        }

        public static async Task<string> GetString(string url, Dictionary<string, string> headers = null)
        {
            HttpResponseMessage response = await _client.SendAsync(BuildRequest(url, HttpMethod.Get, headers));
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<byte[]> GetBytes(string url, Dictionary<string, string> headers = null)
        {
            HttpResponseMessage response = await _client.SendAsync(BuildRequest(url, HttpMethod.Get, headers));
            return await response.Content.ReadAsByteArrayAsync();
        }

        private static HttpRequestMessage BuildRequest(string url, HttpMethod method, Dictionary<string, string> headers = null)
        {
            HttpRequestMessage request = new HttpRequestMessage(method, url);
            if (headers != null)
            {
                foreach ((string name, string value) in headers)
                {
                    request.Headers.Add(name, value);
                }
            }

            return request;
        }
    }
}