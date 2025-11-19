using Application.Features.Students.Dtos;
using AutoMapper;
using Core.Interfaces;
using Infrastructure.Common.GenRepo;


// تم حذف using Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using Application.Common.Behaviours.Interfaces;

namespace Application.Features.Students.Queries.Students.GetStudentById
{
    // 1. تغيير قيمة الإرجاع من Result<StudentDto> إلى StudentDto
    public class GetStudentByIdQueryHandler
       : IRequestHandler<GetStudentByIdQuery, StudentDto>
    {
        private readonly IGenericRepository<Core.Entities.Students.Student> _studentRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ILogger<GetStudentByIdQueryHandler> _logger;

        public GetStudentByIdQueryHandler(
            IGenericRepository<Core.Entities.Students.Student> studentRepository,
            IUserService userService,
            IMapper mapper,
            ILogger<GetStudentByIdQueryHandler> logger)
        {
            _studentRepository = studentRepository;
            _userService = userService;
            _mapper = mapper;
            _logger = logger;
        }

        // 2. تغيير قيمة الإرجاع إلى Task<StudentDto>
        public async Task<StudentDto> Handle(GetStudentByIdQuery request, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Getting student by ID: {StudentId}", request.StudentId);

                // 1. Get student from repository
                var student = await _studentRepository.GetByIdAsync(request.StudentId, ct);
                if (student == null)
                {
                    _logger.LogWarning("Student {StudentId} not found", request.StudentId);
                    // رمي KeyNotFoundException بدلاً من Result.FromError(Error.NotFound)
                    throw new KeyNotFoundException($"Student with ID {request.StudentId} not found.");
                }

                // 2. Get user data from Auth service
                // تم تغيير الاستدعاء للاعتماد على IUserService لرمي استثناء عند الفشل
                var user = await _userService.GetUserByIdAsync(student.UserId);
                // تم حذف:
                // if (!userResult.IsSuccess)
                // { ... رمي استثناء ... }
                // var user = userResult.Value;


                // 3. Combine student and user data
                var studentDto = new StudentDto
                {
                    // Student-specific data
                    Id = student.Id,
                    UserId = student.UserId,
                    // نفترض وجود خاصية Gender في الكيان
                    // Gender = student.Gender, 

                    // User data from AspNetUsers
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    UserName = user.UserName
                };

                _logger.LogInformation("Successfully retrieved student {StudentId}", request.StudentId);

                // 4. إرجاع الـ DTO مباشرةً
                return studentDto;
            }
            catch (KeyNotFoundException)
            {
                // إعادة رمي استثناءات Not Found
                throw;
            }
            catch (ApplicationException)
            {
                // إعادة رمي استثناءات منطق التطبيق
                throw;
            }
            catch (Exception ex)
            {
                // **Try/Catch:** تسجيل أي خطأ غير متوقع ورمي استثناء تطبيقي عام
                _logger.LogError(ex, "Error getting student by ID: {StudentId}", request.StudentId);
                // رمي ApplicationException بدلاً من Result.FromError(Error.Failure)
                throw new ApplicationException($"Student retrieval failed for ID {request.StudentId}.", ex);
            }
        }
    }
}