﻿using System.Threading.Tasks;
using Discord.Commands;
using EduardoBotv2.Core.Models;
using EduardoBotv2.Core.Services;

namespace EduardoBotv2.Core.Modules
{
    public class DankMemes : ModuleBase<EduardoContext>
    {
        private readonly DankMemesService service;

        public DankMemes(DankMemesService service)
        {
            this.service = service;
        }

        [Command("dank", RunMode = RunMode.Async)]
        [Summary("Posts a random dank meme from hot posts on r/dankmemes")]
        public async Task DankCommand()
        {
            await service.PostDankMeme();
        }
    }
}