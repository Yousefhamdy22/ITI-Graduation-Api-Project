using Application.Features.Exam.DTOs;
using Ardalis.Result;
using AutoMapper;
using Core.Entities.Exams;
using Core.Interfaces;
using MediatR;

namespace Application.Features.Exam.Commands.Answers.CreateAnswer;

public class CreateAnswerCommandHandler : IRequestHandler<CreateAnswerCommand, Result<AnswerOptionDto>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAnswerCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<AnswerOptionDto>> Handle(CreateAnswerCommand request, CancellationToken cancellationToken)
    {
        var answerOption = _mapper.Map<AnswerOption>(request.Dto);
        if (answerOption == null)
            return Result<AnswerOptionDto>.Error("Mapping error occurred.");
        await _unitOfWork.AnswerOptions.AddAsync(answerOption);
        await _unitOfWork.CompleteAsync(cancellationToken);
        var answerOptionDto = _mapper.Map<AnswerOptionDto>(answerOption);
        return Result<AnswerOptionDto>.Success(answerOptionDto);
    }
}