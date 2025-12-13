

using Application.Features.Lectures.Dtos;


namespace Application.Features.Modules.Dtos
{

    public class ModuleDto
    {
        public Guid ModuleId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid CourseId { get; set; }

        public List<ResponseLecture> Lectures { get; set; }
    }

    public class CreateModuleDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class UpdatModuleDto
    {
        public Guid ModuleId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }

}
