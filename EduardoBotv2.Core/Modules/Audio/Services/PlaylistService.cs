using System.Threading.Tasks;
using EduardoBotv2.Core.Models;
using EduardoBotv2.Core.Modules.Audio.Database.Playlist;
using EduardoBotv2.Core.Services;

namespace EduardoBotv2.Core.Modules.Audio.Services
{
    public class PlaylistService : IEduardoService
    {
        private readonly IPlaylistRepository _playlistRepository;

        public PlaylistService(IPlaylistRepository playlistRepository)
        {
            _playlistRepository = playlistRepository;
        }

        public async Task CreatePlaylistAsync(EduardoContext context, string playlistName)
        {
            if (string.IsNullOrWhiteSpace(playlistName)) return;

            await _playlistRepository.CreatePlaylistAsync(context.Message.Author.Id, playlistName);
        }
    }
}