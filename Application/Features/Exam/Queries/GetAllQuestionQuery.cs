using Application.Features.Exam.DTOs;
using Ardalis.Result;
using MediatR;

namespace Application.Features.Exam.Queries;

public record GetAllQuestionQuery:IRequest<Result<List<QuestionDto>>>;  
