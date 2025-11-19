//using Application.Common.Behaviours.Interfaces;
//using Application.Features.Students.Dtos;
//using AutoMapper;
//using Core.Interfaces;
//// تم حذف using Domain.Common.Results;
//using Infrastructure.Interface;
//using MediatR;
//using Microsoft.Extensions.Caching.Hybrid;
//using Microsoft.Extensions.Logging;

//namespace Application.Features.Students.Commands.Students.CreateStudent
//{
//    // 1. تغيير قيمة الإرجاع من Result<StudentDto> إلى StudentDto
//    public class CreateStudentCommandHandler(
//        IUserService userService,
//        IStudentRepository studentRepository,
//        IMapper mapper,
//        ILogger<CreateStudentCommandHandler> logger,
//        IUnitOfWork unitOfWork,
//        HybridCache cache,
//        ICurrentUserService currentUserService) : IRequestHandler<CreateStudentCommand, StudentDto>
//    {
//        private readonly IStudentRepository _studentRepository = studentRepository;
//        private readonly IUserService _userService = userService;
//        private readonly IMapper _mapper = mapper;
//        private readonly IUnitOfWork _unitOfWork = unitOfWork;
//        private readonly HybridCache _cache = cache;
//        private readonly ILogger<CreateStudentCommandHandler> _logger = logger;
//        private readonly ICurrentUserService _currentUserService = currentUserService;

//        // 2. تغيير قيمة الإرجاع إلى Task<StudentDto>
//        public async Task<StudentDto> Handle(CreateStudentCommand request, CancellationToken ct)
//        {
//            try
//            {
//                // Simple user extraction - 2 lines!
//                if (_currentUserService.UserId is not Guid userId)
//                {
//                    // رمي UnauthorizedAccessException بدلاً من Result.FromError(Error.Unauthorized)
//                    _logger.LogWarning("Unauthorized attempt to create student profile.");
//                    throw new UnauthorizedAccessException("User must be authenticated to create a student profile.");
//                }

//                // جلب بيانات المستخدم (نفترض أن IUserService لا تزال تستخدم نمط Result، ونحن نتعامل معها هنا)
//                var userResult = await _userService.GetUserByIdAsync(userId);
//                if (!userResult.IsSuccess)
//                {
//                    // رمي KeyNotFoundException بدلاً من Result.FromError(Error.NotFound)
//                    _logger.LogError("Auth user {UserId} found but associated user data retrieval failed.", userId);
//                    throw new KeyNotFoundException($"User not found for authenticated ID: {userId}.");
//                }

//                var user = userResult.Value;

//                _logger.LogInformation("Creating student for user {UserId}", userId);

//                // Check if student already exists
//                var existingStudent = await _studentRepository.GetByUserIdAsnc(userId, ct);
//                if (existingStudent != null)
//                {
//                    // رمي InvalidOperationException (أو ConflictException مخصص) بدلاً من Result.FromError(Error.Conflict)
//                    throw new InvalidOperationException("Student profile already exists for this user.");
//                }

//                // Create student entity (نفترض أن Student.Create لا تزال تستخدم Result)
//                var studentResult = Student.Create(userId, request.gender);

//                if (!studentResult.IsSuccess)
//                {
//                    // رمي InvalidOperationException بدلاً من Result.FromError(Error.Failure)
//                    var errorMessage = studentResult.Error?.Message ?? "Student entity creation failed.";
//                    throw new InvalidOperationException(errorMessage);
//                }

//                var student = studentResult.Value;
//                await _studentRepository.AddAsync(student, ct);
//                await _unitOfWork.CommitAsync(ct);

//                // Cache
//                var studentDto = _mapper.Map<StudentDto>(student);

//                // دمج بيانات المستخدم مع الـ DTO (لتجنب الاعتماد على AutoMapper في هذه النقطة)
//                studentDto.Email = user.Email;
//                studentDto.FirstName = user.FirstName;
//                studentDto.LastName = user.LastName;
//                studentDto.PhoneNumber = user.PhoneNumber;
//                // يجب تعيين الخصائص الأخرى هنا

//                await _cache.SetAsync($"student_{student.Id}", studentDto);
//                await _cache.RemoveByTagAsync("students", ct);

//                _logger.LogInformation("Student created with ID: {StudentId}", student.Id);

//                // 3. إرجاع الـ DTO مباشرةً بدلاً من Result<StudentDto>.FromValue
//                return studentDto;
//            }
//            catch (UnauthorizedAccessException)
//            {
//                // إعادة رمي الاستثناءات المعروفة
//                throw;
//            }
//            catch (KeyNotFoundException)
//            {
//                // إعادة رمي الاستثناءات المعروفة
//                throw;
//            }
//            catch (InvalidOperationException)
//            {
//                // إعادة رمي استثناءات منطق المجال والتضارب
//                throw;
//            }
//            catch (Exception ex)
//            {
//                // **Try/Catch:** تسجيل أي خطأ غير متوقع ورمي استثناء تطبيقي عام
//                _logger.LogError(ex, "Error creating student profile for user {UserId}.", _currentUserService.UserId);
//                throw new ApplicationException("Failed to create student profile due to an unexpected error.", ex);
//            }
//        }
//    }
//}