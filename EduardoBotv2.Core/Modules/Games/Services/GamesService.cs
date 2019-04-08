using System;
using System.IO;
using System.Threading.Tasks;
using EduardoBotv2.Core.Models;
using EduardoBotv2.Core.Modules.Games.Models;
using EduardoBotv2.Core.Services;
using Newtonsoft.Json;

namespace EduardoBotv2.Core.Modules.Games.Services
{
    public class GamesService : IEduardoService
    {
        private static readonly Random _prng = new Random();

        private readonly EightBallData _eightBallData;

        public GamesService()
        {
            _eightBallData = JsonConvert.DeserializeObject<EightBallData>(File.ReadAllText("data/eightball.json"));
        }

        public async Task FlipCoin(EduardoContext context)
        {
            string result = new Random().Next(0, 2) == 0 ? "Heads" : "Tails";
            await context.Channel.SendMessageAsync(result);
        }

        public async Task DisplayEightBall(EduardoContext context, string question = null)
        {
            await context.Channel.TriggerTypingAsync();
            string answer = _eightBallData.Words[_prng.Next(0, _eightBallData.Words.Count)];
            await context.Channel.SendMessageAsync($"{question} -- {answer}");
        }
    }
}