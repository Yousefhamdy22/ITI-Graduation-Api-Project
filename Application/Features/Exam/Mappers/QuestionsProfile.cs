using Application.Features.Exam.DTOs;
using AutoMapper;
using Core.Entities.Exams;

namespace Application.Features.Exam.Mappers;

public class QuestionsProfile : Profile
{
    public QuestionsProfile()
    {
        // Create mappings here if needed in the future


        CreateMap<QuestionDto, Question>().ReverseMap();
    }
}