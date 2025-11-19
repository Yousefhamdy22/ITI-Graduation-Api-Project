using Application.Features.Students.Dtos;
using AutoMapper;
using Infrastructure.Interface;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Behaviours.Interfaces;

namespace Application.Features.Students.Queries.Students.GetStudentWithEnrollmentsQuery
{
    // 1. تم تغيير قيمة الإرجاع إلى DTO مباشرةً
    public class GetStudentWithEnrollmentsQueryHandler
        : IRequestHandler<GetStudentWithEnrollmentsQuery, StudentWithEnrollmentsDto>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ILogger<GetStudentWithEnrollmentsQueryHandler> _logger;

        public GetStudentWithEnrollmentsQueryHandler(
            IStudentRepository studentRepository,
            IUserService userService,
            IMapper mapper,
            ILogger<GetStudentWithEnrollmentsQueryHandler> logger)
        {
            _studentRepository = studentRepository;
            _userService = userService;
            _mapper = mapper;
            _logger = logger;
        }

        // 2. تم تغيير قيمة الإرجاع من Task<Result<...>> إلى Task<...>
        public async Task<StudentWithEnrollmentsDto> Handle(
           GetStudentWithEnrollmentsQuery request,
           CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Getting student {StudentId} with enrollments", request.StudentId);

                // 1. Get student with enrollments
                var student = await _studentRepository.GetWithEnrollmentsAsync(request.StudentId, ct);
                if (student == null)
                {
                    _logger.LogWarning("Student {StudentId} not found", request.StudentId);
                    // رمي استثناء KeyNotFoundException بدلاً من Result.FromError
                    throw new KeyNotFoundException($"Student with ID {request.StudentId} not found.");
                }

                // 2. Get user data
                // تم الآن استدعاء IUserService مباشرةً. إذا لم يتم العثور على المستخدم، فستقوم الخدمة برمي استثناء.
                var user = await _userService.GetUserByIdAsync(student.UserId);

                // 3. Map to DTO and combine with user data
                var studentDto = _mapper.Map<StudentWithEnrollmentsDto>(student);

                // 4. Add user data to the DTO
                studentDto.Email = user.Email;
                studentDto.FirstName = user.FirstName;
                studentDto.LastName = user.LastName;
                studentDto.PhoneNumber = user.PhoneNumber;
                studentDto.UserName = user.UserName;

                _logger.LogInformation("Successfully retrieved student {StudentId} with {EnrollmentCount} enrollments",
                    request.StudentId, studentDto.Enrollments?.Count ?? 0);

                // 5. إرجاع القيمة المباشرة (بدلاً من Result.FromValue)
                return studentDto;
            }
            catch (KeyNotFoundException)
            {
                // إعادة رمي استثناءات Not Found
                throw;
            }
            catch (Exception ex)
            {
                // **Try/Catch:** تسجيل الخطأ ورمي استثناء تطبيقي عام
                _logger.LogError(ex, "Error getting student with enrollments: {StudentId}", request.StudentId);
                throw new ApplicationException($"Failed to retrieve student {request.StudentId} details. Details: {ex.Message}", ex);
            }
        }
    }
}