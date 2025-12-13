using Core.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Modules.Commands.RemoveModule
{
    public record DeleteModuleCommand(Guid ModuleId) : IRequest<Result<bool>>;

}
