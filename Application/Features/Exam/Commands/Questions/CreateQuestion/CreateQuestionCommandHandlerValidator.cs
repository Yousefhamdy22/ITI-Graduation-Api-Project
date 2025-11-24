using FluentValidation;

namespace Application.Features.Exam.Commands.Questions.CreateQuestion;

public class CreateQuestionCommandHandlerValidator : AbstractValidator<CreateQuestionCommand>
{
    public CreateQuestionCommandHandlerValidator()
    {
        RuleFor(x => x.questionDto.Text).NotEmpty().WithMessage("Title is required.");
        RuleFor(x => x.questionDto.Points).NotEmpty().WithMessage("Point is required.");
    }
}