using Application.Features.Lectures.Dtos;
using Application.Features.Modules.Dtos;
using AutoMapper;
using Core.Entities.Courses;


namespace Application.Features.Modules.Mappers
{
    public class ModuleProfile : Profile
    {
        public ModuleProfile()
        {

            CreateMap<Core.Entities.Courses.Module, ModuleDto>()
                .ForMember(dest => dest.ModuleId, opt => opt.MapFrom(src => src.Id));

            CreateMap<Module, UpdatModuleDto>().ReverseMap();
            CreateMap<Lecture, LectureDto>();
            
        }
    }
}
