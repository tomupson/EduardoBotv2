using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EduardoBotv2.Core.Helpers;
using EduardoBotv2.Core.Models;
using EduardoBotv2.Core.Services;
using RedditSharp;
using RedditSharp.Things;

namespace EduardoBotv2.Core.Modules.Memes.Services
{
    public class MemesService : IEduardoService
    {
        private readonly Reddit _reddit;
        private readonly Random _rng;

        public MemesService(Reddit reddit)
        {
            _reddit = reddit;
            _rng = new Random();
        }

        public async Task PostDankMeme(EduardoContext context)
        {
            Subreddit subreddit = await _reddit.GetSubredditAsync("dankmemes");
            Listing<Post> postListing = subreddit.GetPosts(Subreddit.Sort.Hot, 100);
            List<Post> posts = new List<Post>(100);

            await postListing.ForEachAsync(post =>
            {
                posts.Add(post);
            });

            Post randomPost = posts[_rng.Next(0, posts.Count)];

            using Stream stream = await NetworkHelper.GetStreamAsync(randomPost.Url.AbsoluteUri);
            await context.Channel.SendFileAsync(stream, $"{randomPost.Title}.png");
        }
    }
}
