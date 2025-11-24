using Ardalis.Result;
using MediatR;

namespace Application.Features.Exam.Commands.Questions.RemoveQuestion;

public record RemoveQuestionByIdCommand(Guid quesitonId) : IRequest<Result<bool>>;