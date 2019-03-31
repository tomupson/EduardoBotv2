using EduardoBotv2.Core.Services;
using Octokit;

namespace EduardoBotv2.Core.Modules.Github.Services
{
    public class GithubService : IEduardoService
    {
        private readonly Models.Credentials _credentials;
        private readonly GitHubClient _github;

        public GithubService(Models.Credentials credentials, GitHubClient github)
        {
            _credentials = credentials;
            _github = github;
        }
    }
}