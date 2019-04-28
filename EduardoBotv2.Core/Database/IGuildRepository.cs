using System.Threading.Tasks;

namespace EduardoBotv2.Core.Database
{
    public interface IGuildRepository
    {
        Task AddGuildAsync(long guildId);
        Task RemoveGuildAsync(long guildId);
    }
}