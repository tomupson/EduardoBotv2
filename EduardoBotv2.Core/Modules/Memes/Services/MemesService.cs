using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EduardoBotv2.Core.Helpers;
using EduardoBotv2.Core.Models;
using RedditSharp;
using RedditSharp.Things;

namespace EduardoBotv2.Core.Modules.Memes.Services
{
    public class MemesService
    {
        private readonly Reddit reddit;
        private readonly Random rng;

        public MemesService(Reddit reddit)
        {
            this.reddit = reddit;
            rng = new Random();
        }

        public async Task PostDankMeme(EduardoContext context)
        {
            Subreddit subreddit = await reddit.GetSubredditAsync("dankmemes");
            Listing<Post> postListing = subreddit.GetPosts(Subreddit.Sort.Hot, 100);
            List<Post> posts = new List<Post>(100);

            await postListing.ForEachAsync(post =>
            {
                posts.Add(post);
            });

            Post randomPost = posts[rng.Next(0, posts.Count)];

            using (Stream stream = await NetworkHelper.GetStreamAsync(randomPost.Url.AbsoluteUri))
            {
                await context.Channel.SendFileAsync(stream, $"{randomPost.Title}.png");
            }
        }
    }
}