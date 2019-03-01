using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using EduardoBotv2.Core.Models;
using EduardoBotv2.Core.Services;

namespace EduardoBotv2.Core.Modules
{
    public class User : ModuleBase<EduardoContext>
    {
        private readonly UserService service;

        public User(UserService service)
        {
            this.service = service;
        }

        [Command("info", RunMode = RunMode.Async), Alias("userinfo", "user")]
        [Summary("Fetch information about a user.")]
        [Remarks("Eduardo")]
        public async Task UserInfoCommand([Summary("User to get information on.")] IGuildUser user = null)
        {
            await service.DisplayUserInfo(Context, user);
        }

        //[Command("checkinvis"), Alias("checkinvisible")]
        //[Summary("Find invisible users on the server")]
        //[Remarks("")]
        //public async Task CheckInvisibleCommand()
        //{
        //    await _service.CheckInvisible(Context);
        //}

        [Command("avatar", RunMode = RunMode.Async), Alias("av")]
        [Summary("Get the URL of a specific users avatar")]
        [Remarks("Eduardo")]
        public async Task AvatarCommand([Summary("The user who's avatar you want to fetch (defaults to yourself)")] IUser user = null)
        {
            await service.GetAvatar(Context, user);
        }
    }
}