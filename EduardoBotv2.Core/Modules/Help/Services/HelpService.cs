using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using EduardoBotv2.Core.Models;
using EduardoBotv2.Core.Services;

namespace EduardoBotv2.Core.Modules.Help.Services
{
    public class HelpService : IEduardoService
    {
        public async Task DisplayHelpAsync(EduardoContext context, CommandService commandService, string commandOrModule = null)
        {
            EmbedBuilder output = new EmbedBuilder()
                .WithColor(Color.DarkerGrey);
            if (string.IsNullOrWhiteSpace(commandOrModule))
            {
                output.Title = "EduardoBotv2 - Modules";

                foreach (ModuleInfo mod in commandService.Modules.Where(m => m.Parent == null).OrderBy(m => m.Name))
                {
                    AddHelp(mod, ref output);
                }

                output.Footer = new EmbedFooterBuilder
                {
                    Text = "Use 'help <module>' to get help with a module."
                };
            }
            else
            {
                ModuleInfo matchingModule = commandService.Modules.FirstOrDefault(m => string.Equals(m.Name, commandOrModule, StringComparison.CurrentCultureIgnoreCase));
                if (matchingModule == null)
                {
                    await context.Channel.SendMessageAsync("No module could be found with that name.");
                    return;
                }

                output.Title = $"EduardoBotv2 - {matchingModule.Name}";
                output.Description = $"{matchingModule.Summary}\n" +
                                     (!string.IsNullOrWhiteSpace(matchingModule.Remarks) ? $"({matchingModule.Remarks})\n" : "") +
                                     (matchingModule.Aliases.Any(alias => !string.IsNullOrWhiteSpace(alias)) ? $"Prefix{(matchingModule.Aliases.Count > 1 ? "es" : "")}: {string.Join(", ", matchingModule.Aliases)}\n" : "") +
                                     (matchingModule.Submodules.Any() ? $"Submodules: {string.Join(", ", matchingModule.Submodules.Select(m => m.Name))}\n" : "");

                AddCommands(context, matchingModule, ref output);
            }

            IDMChannel userDm = await context.User.GetOrCreateDMChannelAsync();

            await userDm.SendMessageAsync(embed: output.Build());
        }

        public void AddHelp(ModuleInfo module, ref EmbedBuilder builder)
        {
            builder.AddField(f =>
            {
                f.Name = $"**{module.Name}**";

                string value = "";

                if (module.Submodules.Any())
                {
                    value += $"Submodules: {string.Join(", ", module.Submodules.Select(m => m.Name))}\n";
                }

                value += $"Commands: {string.Join(", ", module.Commands.Select(cmd => $"`{module.Group} {cmd.Name}`"))}";

                f.Value = value;
            });

            foreach (ModuleInfo sub in module.Submodules)
            {
                AddHelp(sub, ref builder);
            }
        }

        public void AddCommands(EduardoContext context, ModuleInfo module, ref EmbedBuilder builder)
        {
            foreach (CommandInfo command in module.Commands)
            {
                command.CheckPreconditionsAsync(context, context.Provider).GetAwaiter().GetResult();
                AddCommand(command, ref builder);
            }
        }

        public void AddCommand(CommandInfo command, ref EmbedBuilder builder)
        {
            builder.AddField(f =>
            {
                f.Name = $"**{command.Name}**";
                f.Value = $"{command.Summary}\n" +
                    (command.Aliases.Any() ? $"**Aliases:** {string.Join(", ", command.Aliases.Select(x => $"`{x}`"))}\n" : "") +
                    $"**Usage:** `{GetPrefix(command)} {GetAliases(command)}`\n" +
                    (!string.IsNullOrEmpty(command.Remarks) ? $"**Example:** `{command.Aliases.FirstOrDefault()} {command.Name} {command.Remarks}`" : "");
            });
        }

        public string GetAliases(CommandInfo command)
        {
            StringBuilder output = new StringBuilder();

            if (!command.Parameters.Any())
            {
                return output.ToString();
            }

            foreach (ParameterInfo param in command.Parameters)
            {
                output.Append(param switch
                {
                    { IsOptional: true } => $"[{param.Name} = {param.DefaultValue}] ",
                    { IsMultiple: true } => $"|{param.Name}| ",
                    { IsRemainder: true } => $"...{param.Name} ",
                    _ => $"<{param.Name}> "
                });
            }

            return output.ToString();
        }

        public string GetPrefix(CommandInfo command)
        {
            string output = GetPrefix(command.Module);
            output += $"{command.Aliases.FirstOrDefault()} ";

            return output;
        }

        public string GetPrefix(ModuleInfo module)
        {
            string output = "";

            if (module.Parent != null)
            {
                output = $"{GetPrefix(module.Parent)}{output}";
            }

            if (module.Aliases.Any(x => !string.IsNullOrWhiteSpace(x)))
            {
                output += string.Concat(module.Aliases.FirstOrDefault(), " ");
            }

            return output;
        }
    }
}
