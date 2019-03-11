using Octokit;

namespace EduardoBotv2.Core.Services
{
    public class GithubService
    {
        private readonly Models.Credentials credentials;
        private readonly GitHubClient github;

        public GithubService(Models.Credentials credentials, GitHubClient github)
        {
            this.credentials = credentials;
            this.github = github;

        }
    }
}