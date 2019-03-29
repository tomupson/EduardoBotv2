using Octokit;

namespace EduardoBotv2.Core.Modules.Github.Services
{
    public class GithubService
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