using Application.Common.Behaviours.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly AppDBContext _context;

           public UserService(
          AppDBContext context )
      
           {
               _context = context;
              
           }

        public async Task<UserDto> GetUserByIdAsync(Guid userId)
        {
            // لا حاجة لـ try/catch هنا، لأن طبقة Application (التي تستدعي هذه الخدمة) هي من تتولى التقاط الاستثناءات

            var user = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    PhoneNumber = u.PhoneNumber,
                    UserName = u.UserName
                })
                .FirstOrDefaultAsync();

            // عند عدم العثور على المستخدم، يتم رمي استثناء بدلاً من إرجاع Result.FromError
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found.");
            }

            // عند النجاح، يتم إرجاع الـ DTO مباشرة
            return user;
        }

        Task<UserDto> IUserService.GetUserByIdAsync(Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}
