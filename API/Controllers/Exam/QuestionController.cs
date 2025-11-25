using System.Text.Json;
using Application.Features.Exam.Commands.Questions.CreateQuestion;
using Application.Features.Exam.Commands.Questions.RemoveQuestion;
using Application.Features.Exam.Commands.Questions.UpdateQuestion;
using Application.Features.Exam.DTOs;
using Application.Features.Exam.Queries;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Core.Interfaces;
using Core.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Exam;

[Route("api/[controller]")]
[ApiController]
public class QuestionController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<QuestionController> _logger;
    private readonly ISender _sender;
    private readonly IUnitOfWork _unitOfWork;

    public QuestionController(ILogger<QuestionController> logger, ISender sender, IWebHostEnvironment env,
        IFileStorageService fileStorageService, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _sender = sender;
        _fileStorageService = fileStorageService;
        _env = env;
        _unitOfWork = unitOfWork;
    }


    [HttpPost("CreateQuestion")]
    [TranslateResultToActionResult]
    public async Task<Result<QuestionDto>> CreateQuestion([FromForm] CreateQuestionRequestDto request)
    {
        if (request == null)
            return Result.NotFound("Question Data cannot be null");

        return await _sender.Send(new CreateQuestionCommand(request));
    }


    [HttpDelete("RemoveQuestion/{questionId}")]
    [TranslateResultToActionResult]
    public async Task<Result<bool>> RemoveQuestion(Guid questionId)
    {
        return await _sender.Send(new RemoveQuestionByIdCommand(questionId));
    }

    [HttpPut("UpdateQuestion")]
    [TranslateResultToActionResult]
    public async Task<Result<QuestionDto>> UpdateQuestion([FromForm] QuestionDto? dto, IFormFile? image)
    {
        // fallback to JSON form field
        if (dto == null && Request.HasFormContentType && Request.Form.TryGetValue("questionDto", out var json))
            try
            {
                dto = JsonSerializer.Deserialize<QuestionDto>(json[0], new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Failed to deserialize questionDto from form on update");
            }

        if (dto == null)
            return Result.NotFound("Question Data cannot be null");

        if (image != null)
            dto.ImageUrl =
                await _fileStorageService.UploadFileAsync(image.FileName, image.OpenReadStream(), "Questions");

        return await _sender.Send(new UpdateQuestionCommand(dto, image));
    }


    [HttpGet("GetAllQuestions")]
    [TranslateResultToActionResult]
    public async Task<Result<List<QuestionDto>>> GetAllQuestions()
    {
        return await _sender.Send(new GetAllQuestionQuery());
    }
}