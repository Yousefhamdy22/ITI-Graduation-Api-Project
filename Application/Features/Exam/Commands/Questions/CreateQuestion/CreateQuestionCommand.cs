using Application.Features.Exam.DTOs;
using Ardalis.Result;
using MediatR;

namespace Application.Features.Exam.Commands.Questions.CreateQuestion;

public record CreateQuestionCommand(QuestionDto questionDto) : IRequest<Result<QuestionDto>>;