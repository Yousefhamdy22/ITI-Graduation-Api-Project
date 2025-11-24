using Application.Features.Exam.DTOs;
using Ardalis.Result;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Exam.Commands.Questions.UpdateQuestion;

public record UpdateQuestionCommand(QuestionDto Dto, IFormFile? Image)
    : IRequest<Result<QuestionDto>>;