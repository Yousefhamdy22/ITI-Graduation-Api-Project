using Application.Features.Exam.DTOs;
using Ardalis.Result;
using AutoMapper;
using Core.Interfaces;
using MediatR;

namespace Application.Features.Exam.Commands.Questions.UpdateQuestion;

public class UpdateQuestionCommandHandler : IRequestHandler<UpdateQuestionCommand, Result<QuestionDto>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateQuestionCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<QuestionDto>> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
    {
        var question = await _unitOfWork.Questions.GetByIdAsync(request.Dto.Id);
        if (question == null)
            return Result.NotFound("Question not found");
        _mapper.Map(request.Dto, question);
        var QuestionUpdated = _unitOfWork.Questions.Update(question);
        await _unitOfWork.CompleteAsync(cancellationToken);
        var questionDto = _mapper.Map<QuestionDto>(QuestionUpdated);
        return Result.Success(questionDto);
    }
}