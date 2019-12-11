using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Models;
using YoutubeExplode.Models.MediaStreams;

namespace EduardoBotv2.Core.Helpers
{
    public static class VideoHelper
    {
        public static async Task<List<Video>> GetOrSearchVideoAsync(string query)
        {
            using HttpClient httpClient = new HttpClient();
            YoutubeClient client = new YoutubeClient(httpClient);

            List<Video> videos;

            if (!YoutubeClient.TryParseVideoId(query, out string videoId))
            {
                videos = (await client.SearchVideosAsync(query, 1)).ToList();
            }
            else
            {
                videos = new List<Video>(1)
                {
                    await client.GetVideoAsync(videoId)
                };
            }

            return videos;
        }

        public static async Task<MediaStreamInfoSet> GetMediaStreamInfoAsync(Video video)
        {
            if (video == null)
            {
                return null;
            }

            using HttpClient httpClient = new HttpClient();
            YoutubeClient client = new YoutubeClient(httpClient);

            return await client.GetVideoMediaStreamInfosAsync(video.Id);
        }
    }
}
