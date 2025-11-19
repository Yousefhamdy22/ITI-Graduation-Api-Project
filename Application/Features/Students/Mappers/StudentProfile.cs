using Application.Features.Students.Dtos;
using AutoMapper;
using Core.Entities.Students;


namespace Application.Features.Students.Mappers
{
    public class StudentProfile : Profile
    {

        public StudentProfile()
        {
             //CreateMap<Domain.Entities.Students.Student, StudentDto>().ReverseMap();

             CreateMap<Core.Entities.Students.Student, CreateStudentDto>().ReverseMap();

            CreateMap<Student, StudentDto>()
                 .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                 .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
                 .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
                 .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
                 .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));
               
            CreateMap<Student, StudentWithEnrollmentsDto>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));




        }
    }
}
