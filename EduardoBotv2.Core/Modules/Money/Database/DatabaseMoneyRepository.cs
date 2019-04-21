using System.Threading.Tasks;
using EduardoBotv2.Core.Database;
using EduardoBotv2.Core.Models;
using EduardoBotv2.Core.Modules.Money.Database.Results;

namespace EduardoBotv2.Core.Modules.Money.Database
{
    public class DatabaseMoneyRepository : IMoneyRepository
    {
        private readonly Credentials _credentials;

        public DatabaseMoneyRepository(Credentials credentials)
        {
            _credentials = credentials;
        }

        public async Task<int> GetMoneyAsync(long discordUserId, long guildId)
        {
            AsyncDataReader dr = new AsyncDataReader("MONEY_GetMoney", _credentials.DbConnectionString);
            dr.AddParameter("@DiscordUserId", discordUserId);
            dr.AddParameter("@DiscordGuildId", guildId);

            int money = 0;
            await dr.ExecuteReaderAsync(reader =>
            {
                money = reader.GetInt32(reader.GetOrdinal("MONEY"));
                return Task.CompletedTask;
            });

            return money;
        }

        public async Task SetMoneyAsync(long discordUserId, long guildId, int money)
        {
            AsyncDataReader dr = new AsyncDataReader("MONEY_SetMoney", _credentials.DbConnectionString);
            dr.AddParameter("@DiscordUserId", discordUserId);
            dr.AddParameter("@DiscordGuildId", guildId);
            dr.AddParameter("@Money", money);

            await dr.ExecuteNonQueryAsync();
        }

        public async Task<DonateMoneyResult> DonateMoneyAsync(long donorDiscordUserId, long doneeDiscordUserId, long guildId, int amount)
        {
            AsyncDataReader dr = new AsyncDataReader("MONEY_DonateMoney", _credentials.DbConnectionString);
            dr.AddParameter("@DonorDiscordUserId", donorDiscordUserId);
            dr.AddParameter("@DoneeDiscordUserId", doneeDiscordUserId);
            dr.AddParameter("@DiscordGuildId", guildId);
            dr.AddParameter("@Amount", amount);

            try
            {
                return (DonateMoneyResult)await dr.ExecuteScalarAsync();
            } catch
            {
                return DonateMoneyResult.TransactionFailed;
            }
        }
    }
}