using MediatR;

namespace Application.Features.Students.Commands.Students.RemoveStudent
{
    public record DeleteStudentCommand(Guid StudentId) : IRequest<bool>;
}
