using Discord.Commands;
using EduardoBotv2.Core.Models;

namespace EduardoBotv2.Core.Services
{
    public abstract class EduardoModule<TService> : EduardoModule where TService : IEduardoService
    {
        protected readonly TService _service;

        protected EduardoModule(TService service)
        {
            _service = service;
        }
    }

    public abstract class EduardoModule : ModuleBase<EduardoContext>
    {

    }
}