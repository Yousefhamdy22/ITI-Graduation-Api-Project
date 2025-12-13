using Application.Features.Modules.Dtos;
using Core.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Modules.Queries.GetModulesQuery
{
    public record GetModulesByCourseIdQuery(Guid CourseId)
                                               : IRequest<Result<List<ModuleDto>>>;
}
