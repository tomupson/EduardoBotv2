using EduardoBotv2.Core.Modules.Github.Services;
using EduardoBotv2.Core.Services;

namespace EduardoBotv2.Core.Modules.Github
{
    public class Github : EduardoModule<GithubService>
    {
        public Github(GithubService service)
            : base(service) { }
    }
}