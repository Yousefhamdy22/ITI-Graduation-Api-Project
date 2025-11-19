// تم حذف using Domain.Common.Results;

namespace Application.Common.Behaviours.Interfaces
{
    public interface IUserService
    {
        // إذا كانت هذه الدالة غير معلقة، فليست بحاجة لتعديل لأنها لا تستخدم Result
        //Guid GetCurrentUserId(); 

        // إذا كانت هذه الدالة غير معلقة، فليست بحاجة لتعديل لأنها لا تستخدم Result
        //Task<bool> UserExistsAsync(Guid userId); 

        // تم تغيير نوع الإرجاع من Task<Result<UserDto>> إلى Task<UserDto>
        Task<UserDto> GetUserByIdAsync(Guid userId);
    }
}