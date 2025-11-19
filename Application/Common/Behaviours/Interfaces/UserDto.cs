// تم حذف using Domain.Common.Results;

namespace Application.Common.Behaviours.Interfaces
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string UserName { get; set; } = string.Empty;
    }
}