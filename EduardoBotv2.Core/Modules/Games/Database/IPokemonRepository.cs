using System.Collections.Generic;
using System.Threading.Tasks;
using EduardoBotv2.Core.Modules.Games.Models.Pokemon;

namespace EduardoBotv2.Core.Modules.Games.Database
{
    public interface IPokemonRepository
    {
        Task<Dictionary<PokemonSummary, int>> GetPokemonAsync(long discordUserId, long guildId);

        Task AddPokemonAsync(long discordUserId, long guildId, PokemonSummary pokemon);
    }
}