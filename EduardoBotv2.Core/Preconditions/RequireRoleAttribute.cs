using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace EduardoBotv2.Core.Preconditions
{
    public class RequireRoleAttribute : PreconditionAttribute
    {
        private readonly string _requiredRole;

        public RequireRoleAttribute(string requiredRole)
        {
            _requiredRole = requiredRole;
        }

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (context.User is SocketGuildUser guildUser)
            {
                if (guildUser.Roles.Any(role => role.Name == _requiredRole))
                {
                    return Task.FromResult(PreconditionResult.FromSuccess());
                }
            }

            return Task.FromResult(PreconditionResult.FromError($"User does not have required role {_requiredRole}"));
        }
    }
}