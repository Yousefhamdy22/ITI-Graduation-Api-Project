using Application.Features.Exam.DTOs;
using Ardalis.Result;
using MediatR;

namespace Application.Features.Exam.Commands.Questions.UpdateQuestion;

public record UpdateQuestionCommand(QuestionDto Dto) : IRequest<Result<QuestionDto>>;