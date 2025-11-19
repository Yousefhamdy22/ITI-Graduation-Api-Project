using Application.Features.Students.Dtos;
using AutoMapper;
using Core.Interfaces;
// تم حذف using Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Application.Common.Behaviours.Interfaces;

namespace Application.Features.Students.Commands.Students.UpdateStudent
{
    // 1. تغيير قيمة الإرجاع من Result<StudentDto> إلى StudentDto
    public class UpdateStudentCommandHandler : IRequestHandler<UpdateStudentCommand, StudentDto>
    {
        private readonly IGenericRepository<Core.Entities.Students.Student> _studentRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateStudentCommandHandler> _logger;
        private readonly HybridCache _cache;

        public UpdateStudentCommandHandler(
            IGenericRepository<Core.Entities.Students.Student> studentRepository,
            IUserService userService,
            IMapper mapper,
            ILogger<UpdateStudentCommandHandler> logger,
            IUnitOfWork unitOfWork,
            HybridCache cache)
        {
            _studentRepository = studentRepository;
            _userService = userService;
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        // 2. تغيير قيمة الإرجاع إلى Task<StudentDto>
        public async Task<StudentDto> Handle(UpdateStudentCommand request, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Updating student {StudentId}", request.StudentId);

                // 1. Get the student
                var student = await _studentRepository.GetByIdAsync(request.StudentId, ct);
                if (student == null)
                {
                    _logger.LogWarning("Student {StudentId} not found", request.StudentId);
                    throw new KeyNotFoundException($"Student with ID {request.StudentId} not found.");
                }

                // 2. Get user data for response (الاعتماد على IUserService لرمي الاستثناء عند الفشل)
                // تم الآن استدعاء IUserService مباشرةً، وهي إما ترجع UserDto أو ترمي استثناء (KeyNotFoundException)
                var user = await _userService.GetUserByIdAsync(student.UserId);

                // 3. Apply domain update 
                // نفترض أن student.Update(request.Gender) ترمي InvalidOperationException إذا فشلت
                student.Update(request.Gender);

                // 4. Save changes
                await _studentRepository.UpdateAsync(student, ct);
                await _unitOfWork.CommitAsync(ct);

                // 5. Invalidate cache
                await _cache.RemoveAsync($"student_{request.StudentId}", ct);
                await _cache.RemoveByTagAsync("students", ct);

                _logger.LogInformation("Student {StudentId} updated successfully", request.StudentId);

                // 6. Return combined response (StudentDto)
                return new StudentDto
                {
                    Id = student.Id,
                    UserId = student.UserId,
                    // Student-specific data
                    Gender = student.Gender,

                    // User data from Auth Service
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber
                };
            }
            catch (KeyNotFoundException)
            {
                // إعادة رمي استثناءات Not Found
                throw;
            }
            catch (InvalidOperationException)
            {
                // إعادة رمي استثناءات منطق المجال (مثل فشل التحديث)
                throw;
            }
            catch (Exception ex)
            {
                // **Try/Catch:** تسجيل أي خطأ غير متوقع ورمي استثناء تطبيقي عام
                _logger.LogError(ex, "Error updating student {StudentId}", request.StudentId);
                throw new ApplicationException($"Student update failed for ID {request.StudentId}.", ex);
            }
        }
    }
}