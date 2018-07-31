using System;
using Discord;
using System.Net.Http;
using System.Threading.Tasks;

namespace EduardoBotv2.Common.Utilities.Helpers
{
    public static class NetworkHelper
    {
        private static HttpClient httpClient = new HttpClient();

        public static async Task<HttpResponseMessage> MakeRequest(HttpRequestMessage request)
        {
            try
            {
                return await httpClient.SendAsync(request);
            }
            catch (Exception e)
            {
                await Logger.Log(new LogMessage(LogSeverity.Critical, "EduardoBot", $"Error sending request.\n{e}"));
                return null;
            }
        }
    }
}