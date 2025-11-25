using Application.Features.Exam.Commands.Answers.CreateAnswer;
using Application.Features.Exam.DTOs;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Exam;

[Route("api/[controller]")]
[ApiController]
public class AnswerController : ControllerBase
{
    private readonly ILogger<AnswerController> _logger;
    private readonly ISender _sender;


    public AnswerController(ILogger<AnswerController> logger, ISender sender)
    {
        _logger = logger;
        _sender = sender;
    }

    [HttpPost("CreateAnswer")]
    [TranslateResultToActionResult]
    public async Task<Result<AnswerOptionDto>> CreateAnswer([FromBody] CreateAnswerCommand command)
    {
        return await _sender.Send(command);
    }
}