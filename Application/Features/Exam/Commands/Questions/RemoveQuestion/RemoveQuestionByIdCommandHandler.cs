using Ardalis.Result;
using Core.Interfaces;
using MediatR;

namespace Application.Features.Exam.Commands.Questions.RemoveQuestion;

public class RemoveQuestionByIdCommandHandler : IRequestHandler<RemoveQuestionByIdCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public RemoveQuestionByIdCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(RemoveQuestionByIdCommand request, CancellationToken cancellationToken)
    {
        var question = _unitOfWork.Questions.GetByIdAsync(request.quesitonId);
        if (question == null) return Result<bool>.NotFound("Question not found");
        _unitOfWork.Questions.Delete(question.Result);
        await _unitOfWork.CompleteAsync(cancellationToken);
        return Result.Success(true, "Question removed successfully");
    }
}