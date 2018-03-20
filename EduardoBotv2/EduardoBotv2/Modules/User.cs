using Discord;
using Discord.Commands;
using EduardoBot.Services;
using EduardoBot.Common.Data;
using System.Threading.Tasks;

namespace EduardoBot.Modules
{
    public class User : ModuleBase<EduardoContext>
    {
        private readonly UserService _service;

        public User(UserService service)
        {
            this._service = service;
        }

        [Command("info", RunMode = RunMode.Async), Alias("userinfo", "user")]
        [Summary("Fetch information about a user.")]
        [Remarks("Eduardo")]
        public async Task UserInfoCommand([Summary("User to get information on.")] IGuildUser user = null)
        {
            await _service.DisplayUserInfo(Context, user);
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
            await _service.GetAvatar(Context, user);
        }
    }
}