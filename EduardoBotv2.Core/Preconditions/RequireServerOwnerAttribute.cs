using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace EduardoBotv2.Core.Preconditions
{
    public class RequireServerOwnerAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
            => context.Guild.OwnerId == context.User.Id ?
                Task.FromResult(PreconditionResult.FromSuccess()) :
                Task.FromResult(PreconditionResult.FromError("User is not owner of the current guild"));
    }
}