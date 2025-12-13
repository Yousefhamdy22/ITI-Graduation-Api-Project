using Application.Features.Modules.Dtos;
using Core.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Modules.Commands.CreateModules
{
    public record CreateModuleCommand(string Title, string Description, Guid CourseId) : IRequest<Result<ModuleDto>>;

}
