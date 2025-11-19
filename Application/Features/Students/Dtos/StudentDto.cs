namespace Application.Features.Students.Dtos
{
    public class StudentDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Gender { get; set; }


        // From User (Auth Service)
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
    }

    public class CreateStudentDto
    {
       
        public string FirstName { get; private set; } = default!;
        public string LastName { get; private set; } = default!;
        public string Email { get; private set; } = default!;
    }

    public class StudentWithEnrollmentsDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

     
        public string Gender { get; set; } = string.Empty;

        // User data
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string UserName { get; set; } = string.Empty;

        // Enrollments
        public List<CourseDto> Enrollments { get; set; } = new();
    }

    public class CourseDto
    {
    }

    public class StudentAnswerDto
    {
        public Guid QuestionId { get; set; }
        public Guid SelectedAnswerId { get; set; } 
    }
}
