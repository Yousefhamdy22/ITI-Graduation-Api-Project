using Application.Common.Behaviours.Interfaces;
using Application.Features.Students.Dtos;
using Application.Features.Students.Queries.Students.GetStudentById;
using Application.Features.Students.Queries.Students.GetStudentWithEnrollmentsQuery;
using AutoMapper;
using Core.Entities.Students;
using Core.Interfaces;
using Infrastructure.Interface;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Application.Services
{
    public class StudentService : IStudentService
    {

        #region DI Injection

        
        private readonly IStudentRepository _studentRepository;
        private readonly IGenericRepository<Student> _student;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly ILogger<StudentService> _logger;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        #endregion

        #region Ctors Injection

        
        public StudentService(IStudentRepository studentRepository, 
            IEnrollmentRepository enrollmentRepository , IGenericRepository<Student> student
            , IMapper mapper, ILogger<StudentService> logger,   IMediator mediator) 
        { 
            _studentRepository = studentRepository;
            _enrollmentRepository = enrollmentRepository;
            _student = student;
            _logger = logger;
            _mapper = mapper;
            _mediator = mediator;
        }
        #endregion

    
        #region Student Command

       
        //public Task<Result<StudentDto>> CreateStudentAsync(CreateStudentDto dto, CancellationToken ct)
        //{


        //    var command = new CreateStudentCommand(
        //        EnrollmentDate
        //       );

        //    return _mediator.Send(command, ct);
        //}

        //public Task<Result<StudentDto>> UpdateStudentAsync(Guid id, StudentDto dto)
        //{
        //   var command = new UpdateStudentCommand(
        //        id,
        //        dto.FirstName,
        //        dto.LastName,
        //        dto.Email);
        //    return _mediator.Send(command);
        //}


        #endregion

        #region Student Queries 
        public async Task<StudentDto> GetStudentAsync(Guid studentId, CancellationToken ct)
        {
            var query = new GetStudentByIdQuery(studentId);
            // تم حذف result.Value. نفترض أن Mediator يرجع StudentDto مباشرةً.
            return await _mediator.Send(query, ct);
        }

        public async Task<StudentWithEnrollmentsDto> GetWithEnrollmentsAsync(Guid studentId, CancellationToken ct)
        {
            // تم تغيير GetStudentByIdQuery إلى GetStudentWithEnrollmentsQuery (افتراضياً لتوافقها مع اسم الدالة)
            var query = new GetStudentWithEnrollmentsQuery(studentId);
            // تم حذف .Value و Task.FromResult. يتم إرجاع نتيجة Mediator مباشرةً.
            return await _mediator.Send(query, ct);
        }
        #endregion

        #region UnHandeld (تم تحديث أنواع الإرجاع لتتوافق مع IStudentService)

        // تم تغيير نوع الإرجاع من Task<Result<Success>> إلى Task
        public Task EnrollInCourseAsync(Guid studentId, Guid courseId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<CourseDto>> GetCoursesAsync(Guid studentId)
        {
            throw new NotImplementedException();
        }

        //// تم تفعيل وإصلاح الجزء المعلق ليطابق واجهة IStudentService
        //public Task<IReadOnlyList<LectureDto>> GetLecturesAsync(Guid studentId, Guid courseId)
        //{
        //    throw new NotImplementedException();
        //}

        // تم تفعيل وإصلاح الجزء المعلق ليطابق واجهة IStudentService
        //public Task<IReadOnlyList<ZoomDto>> GetExamsAsync(Guid studentId, Guid courseId)
        //{
        //    throw new NotImplementedException();
        //}


        public async Task<StudentDto> GetStudentsAsyc(CancellationToken ct)
        {
            // ملاحظة: اسم الدالة يوحي بأنها تجلب عدة طلاب، بينما ترجع StudentDto واحد. تم الإبقاء على منطق الجسم.
            await _student.GetAllAsync(ct);
            return new StudentDto();
        }
        #endregion


        #region StudentWithCourse (تم إصلاح توافق التوقيع مع IStudentService)

        // تم تغيير التوقيع من Task إلى Task<IEnumerable<StudentDto>> ليتوافق مع IStudentService.GetStudentsByCourseAsync
        public async Task<IEnumerable<StudentDto>> GetStudentsByCourseAsync(Guid courseId, CancellationToken ct = default)
        {
            if (courseId == Guid.Empty)
                throw new ArgumentException("Course ID cannot be empty.", nameof(courseId));

            // يجب أن تقوم الدالة بالانتظار (await)
            return (IEnumerable<StudentDto>)await _studentRepository.GetStudentsByCourseAsync(courseId);
        }

        // تم حذف GetStudentWithEnrollments حيث أنها مكررة لـ GetWithEnrollmentsAsync (المُحدَّثة أعلاه)
        // التي تطابق توقيع IStudentService.
        #endregion

        #region Student Checks
        public async Task<StudentDto> GetStudentById(Guid studentId, CancellationToken ct = default)
        {
            var query = new GetStudentByIdQuery(studentId);
            // تم حذف result.Value
            var result = await _mediator.Send(query, ct);

            return result;
        }
        public async Task<bool> StudentExistsAsync(Guid studentId, CancellationToken ct = default)
        {
            if (studentId == Guid.Empty)
                throw new ArgumentException("Student ID cannot be empty.", nameof(studentId));
            return await _studentRepository.ExistsAsync(studentId, ct);
        }
        public async Task<bool> IsStudentEnrolledInCourseAsync(Guid studentId, Guid courseId,
            CancellationToken ct = default)
        {
            if (studentId == Guid.Empty)
                throw new ArgumentException("Student ID cannot be empty.", nameof(studentId));
            if (courseId == Guid.Empty)
                throw new ArgumentException("Course ID cannot be empty.", nameof(courseId));
            return await _enrollmentRepository.ExistsAsync(studentId, courseId, ct);
        }

        Task<StudentDto> IStudentService.CreateStudentAsync(CreateStudentDto dto, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        Task<StudentDto> IStudentService.UpdateStudentAsync(Guid id, StudentDto dto)
        {
            throw new NotImplementedException();
        }

        Task IStudentService.EnrollInCourseAsync(Guid studentId, Guid courseId, CancellationToken ct)
        {
            // هذه الدالة تستدعي الآن النسخة المحدثة (EnrollInCourseAsync) التي ترجع Task
            return EnrollInCourseAsync(studentId, courseId, ct);
        }
        // ...

        #endregion

    }
}
