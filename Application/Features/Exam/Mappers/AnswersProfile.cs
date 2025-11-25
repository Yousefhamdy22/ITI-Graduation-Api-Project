using Application.Features.Exam.DTOs;
using AutoMapper;
using Core.Entities.Exams;

namespace Application.Features.Exam.Mappers;

public class AnswersProfile : Profile
{
    public AnswersProfile()
    {
        CreateMap<AnswerOption, AnswerOptionDto>().ReverseMap();
    }
}