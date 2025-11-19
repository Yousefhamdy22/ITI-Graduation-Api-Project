using Application.Features.Students.Dtos;
using MediatR;
using System;

namespace Application.Features.Students.Commands.Students.CreateStudent
{
    public record CreateStudentCommand(
         // لاحظ أنك تحتاج لإضافة جميع خصائص التسجيل هنا (الاسم، الإيميل، كلمة المرور... إلخ)
         string gender
     // string FirstName, 
     // string LastName,
     // string Email,
     // string Password
     ) : IRequest<StudentDto>;
}
    

