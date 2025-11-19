using Application.Features.Students.Dtos;

namespace Application.Features.Enrollments.Dto
{
    public class EnrollmentDto
    {
        public Guid StudentId { get; set; }
        public Guid CourseId { get; set; }
        public DateTimeOffset EnrollmentDate { get; set; }
    }

    public class EnrollmentDetailsDto : EnrollmentDto
    {
     
        public string Status { get; set; } = string.Empty;
       
        public StudentDto? Student { get; set; }

        public CourseDto? Course { get; set; }


    }
}
