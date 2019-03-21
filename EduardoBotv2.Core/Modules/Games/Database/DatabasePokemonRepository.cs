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
        private readonly string connectionString;

        public DatabasePokemonRepository(Credentials credentials)
        {
            connectionString = $@"Data Source=(LocalDb)\MSSQLLocalDB;Integrated Security=true;Initial Catalog=Eduardo;AttachDBFilename={credentials.AttachDbFilename}";
        }

        public async Task<Dictionary<PokemonSummary, int>> GetPokemonAsync(ulong discordUserId, ulong guildId)
        {
            DataReader dr = new DataReader("POKEMON_GetPokemonInventory", connectionString);
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
            DataReader dr = new DataReader("POKEMON_AddPokemonToInventory", connectionString);
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