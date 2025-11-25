using Application.Features.Exam.DTOs;
using AutoMapper;
using Core.Entities.Exams;

namespace Application.Features.Exam.Mappers;

public class QuestionsProfile : Profile
{
    public QuestionsProfile()
    {
        // من الـ Request إلى الـ Entity
        CreateMap<CreateQuestionRequestDto, Question>()
            .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
            .ForMember(dest => dest.AnswerOptions, opt => opt.MapFrom(src =>
                src.AnswerOptions.Select(a => new AnswerOption
                {
                    Text = a.Text,
                    IsCorrect = a.IsCorrect
                })
            ));

        // من الـ Entity إلى الـ Dto
        CreateMap<Question, QuestionDto>();
        CreateMap<AnswerOption, AnswerOptionDto>();
    }
}