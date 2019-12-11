using System.Threading.Tasks;
using EduardoBotv2.Core;

namespace EduardoBotv2
{
    internal sealed class Program
    {
        private static async Task Main()
        {
            using EduardoBot bot = new EduardoBot();
            await bot.RunAsync();
        }
    }
}
