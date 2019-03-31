using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using EduardoBotv2.Core.Modules.User.Services;
using EduardoBotv2.Core.Services;

namespace EduardoBotv2.Core.Modules.User
{
    public class User : EduardoModule<UserService>
    {
        public User(UserService service)
            : base(service) { }

        [Command("user")]
        [Summary("Fetch information about a user")]
        [Remarks("Eduardo")]
        public async Task UserInfoCommand([Summary("User to get information on")] IGuildUser user = null)
        {
            await _service.DisplayUserInfo(Context, user);
        }

        [Command("avatar")]
        [Summary("Get the URL of a specific users avatar")]
        [Remarks("Eduardo")]
        public async Task AvatarCommand([Summary("The user who's avatar you want to fetch (defaults to yourself)")] IUser user = null)
        {
            await _service.GetAvatar(Context, user);
        }
    }
}