using Application.Features.Exam.DTOs;
using Ardalis.Result;
using AutoMapper;
using Core.Entities.Exams;
using Core.Interfaces;
using Core.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Exam.Commands.Questions.CreateQuestion;

public class CreateQuestionCommandHandler : IRequestHandler<CreateQuestionCommand, Result<QuestionDto>>
{
    private readonly IFileStorageService _fileStorage;
    private readonly ILogger<CreateQuestionCommandHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public CreateQuestionCommandHandler(IUnitOfWork unitOfWork, IMapper mapper,
        ILogger<CreateQuestionCommandHandler> logger, IFileStorageService fileStorage)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _fileStorage = fileStorage;
    }

    public async Task<Result<QuestionDto>> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
    {
        var dto = request.dto;
        if (dto == null) return Result.Error("please add Question");

        if (request.dto.Image != null)
        {
            // upload new file
            var newImageUrl = await _fileStorage.UploadFileAsync(
                request.dto.Image.FileName,
                request.dto.Image.OpenReadStream(),
                "Questions"
            );

            dto.ImageUrl = newImageUrl;
        }

        var question = _mapper.Map<Question>(dto);
     


        // foreach (var ans in question.AnswerOptions)
        // {
        //     ans.Question = question;
        // }

        await _unitOfWork.Questions.AddAsync(question);

        var affected = await _unitOfWork.CompleteAsync(cancellationToken);
        _logger.LogInformation("SaveChanges affected rows: {Affected}", question);

        // Verify saved answer options via repository query
        // var savedOptions = (await _unitOfWork.AnswerOptions.FindAllAsync(a => a.QuestionId == question.Id))?.ToList();
        // _logger.LogInformation("AnswerOptions saved for question {QuestionId}: {Count}", question.Id, savedOptions?.Count ?? 0);
        //
        // // Re-fetch the saved entity including AnswerOptions to ensure we return the persisted state
        // var savedQuestion = await _unitOfWork.Questions.FindAsync(q => q.Id == question.Id, new[] { "AnswerOptions" });
        //
        // var resultDto = _mapper.Map<QuestionDto>(savedQuestion ?? question);
        // return Result.Success(resultDto);
        var resultDto = _mapper.Map<QuestionDto>(question);
        return Result.Success(resultDto);
    }
}