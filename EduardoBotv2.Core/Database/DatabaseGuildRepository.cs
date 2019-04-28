using System.Threading.Tasks;
using EduardoBotv2.Core.Models;

namespace EduardoBotv2.Core.Database
{
    public class DatabaseGuildRepository : IGuildRepository
    {
        private readonly Credentials _credentials;

        public DatabaseGuildRepository(Credentials credentials)
        {
            _credentials = credentials;
        }

        public async Task AddGuildAsync(long guildId)
        {
            AsyncDataReader dr = new AsyncDataReader("GUILD_AddGuild", _credentials.DbConnectionString);
            dr.AddParameter("@GuildId", guildId);

            await dr.ExecuteNonQueryAsync();
        }

        public async Task RemoveGuildAsync(long guildId)
        {
            AsyncDataReader dr = new AsyncDataReader("GUILD_RemoveGuild", _credentials.DbConnectionString);
            dr.AddParameter("@GuildId", guildId);

            await dr.ExecuteNonQueryAsync();
        }
    }
}