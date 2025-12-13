using Application.Features.Modules.Dtos;
using Core.Common.Results;
using MediatR;


namespace Application.Features.Modules.Commands.UpdateModules
{
    public class UpdateModuleCommand : IRequest<Result<UpdatModuleDto>>
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid CourseId { get; set; }
    }



}
