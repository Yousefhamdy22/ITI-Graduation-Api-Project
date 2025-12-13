using Application.Features.Modules.Dtos;
using Core.Common.Results;
using MediatR;

namespace Application.Features.Modules.Queries.GetModulesBiIdQuery
{
    public record GetModuleByIdQuery : IRequest<Result<ModuleDto>>
    {
        public Guid ModuleId { get; set; }
    }
}
