using Application.Common.Exceptions;
using Application.Features.Exam.DTOs;
using Ardalis.Result;
using AutoMapper;
using Core.Entities.Exams;
using Core.Interfaces;
using MediatR;

namespace Application.Features.Exam.Commands.Questions.CreateQuestion;

public class CreateQuestionCommandHandler : IRequestHandler<CreateQuestionCommand, Result<QuestionDto>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public CreateQuestionCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<QuestionDto>> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
    {
        if (request.questionDto is null)
            return Result.Error("please add Question ");

        var question = _mapper.Map<Question>(request.questionDto);
        if (question == null) throw new BusinessException("mapper problem");
        await _unitOfWork.Questions.AddAsync(question);
        await _unitOfWork.CompleteAsync(cancellationToken);
        return Result.Success(_mapper.Map<QuestionDto>(question));
    }
}