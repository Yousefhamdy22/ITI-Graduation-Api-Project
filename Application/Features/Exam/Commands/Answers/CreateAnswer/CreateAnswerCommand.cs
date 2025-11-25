using Application.Features.Exam.DTOs;
using Ardalis.Result;
using MediatR;

namespace Application.Features.Exam.Commands.Answers.CreateAnswer;

public record CreateAnswerCommand(AnswerOptionDto Dto) : IRequest<Result<AnswerOptionDto>>;