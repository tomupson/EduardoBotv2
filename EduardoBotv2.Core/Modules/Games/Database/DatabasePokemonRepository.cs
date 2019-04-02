using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using EduardoBotv2.Core.Database;
using EduardoBotv2.Core.Models;
using EduardoBotv2.Core.Modules.Games.Models.Pokemon;

namespace EduardoBotv2.Core.Modules.Games.Database
{
    public class DatabasePokemonRepository : IPokemonRepository
    {
        private readonly string _connectionString;

        public DatabasePokemonRepository(Credentials credentials)
        {
            _connectionString = credentials.DbConnectionString;
        }

        public async Task<Dictionary<PokemonSummary, int>> GetPokemonAsync(ulong discordUserId, ulong guildId)
        {
            AsyncDataReader dr = new AsyncDataReader("POKEMON_GetPokemonInventory", _connectionString);
            dr.AddParameter("@DiscordGuildId", (long)guildId);
            dr.AddParameter("@DiscordUserId", (long)discordUserId);

            Dictionary<PokemonSummary, int> pokemon = new Dictionary<PokemonSummary, int>();
            await dr.ExecuteReaderAsync(reader =>
            {
                int amount = reader.GetInt32(reader.GetOrdinal("AMOUNT"));
                pokemon.TryAdd(GetPokemonSummaryFromReader(reader), amount);

                return Task.CompletedTask;
            });

            return pokemon;
        }

        public async Task AddPokemonAsync(ulong discordUserId, ulong guildId, PokemonSummary pokemon)
        { 
            AsyncDataReader dr = new AsyncDataReader("POKEMON_AddPokemonToInventory", _connectionString);
            dr.AddParameter("@DiscordGuildId", (long)guildId);
            dr.AddParameter("@DiscordUserId", (long)discordUserId);
            dr.AddParameter("@PokemonNumber", pokemon.Id);
            dr.AddParameter("@PokemonName", pokemon.Name);

            await dr.ExecuteNonQueryAsync();
        }

        private static PokemonSummary GetPokemonSummaryFromReader(IDataReader reader) => new PokemonSummary(
            reader.GetInt32(reader.GetOrdinal("POKEMON_NUMBER")),
            reader.GetString(reader.GetOrdinal("POKEMON_NAME")));
    }
}