using Application.Features.Enrollments.Dto;
using AutoMapper;
using Core.Entities.Courses;
using Core.Interfaces;
using Infrastructure.Interface;
using MediatR;
using Enrollment = Core.Entities.Courses.Enrollment;

namespace Application.Features.Enrollments.Commands.CreateEnrollment
{
    public class EnrollInCourseCommandHandler : IRequestHandler<EnrollInCourseCommand, EnrollmentDto>
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EnrollInCourseCommandHandler(
            IEnrollmentRepository enrollmentRepository,
            ICourseRepository courseRepository,
            IStudentRepository studentRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _enrollmentRepository = enrollmentRepository;
            _courseRepository = courseRepository;
            _studentRepository = studentRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<EnrollmentDto> Handle(EnrollInCourseCommand request, CancellationToken ct)
        {
            // Validate student
            var student = await _studentRepository.GetWithEnrollmentsAsync(request.StudentId, ct);
            if (student == null)
                throw new Exception("Student not found");

            // Validate course
            var course = await _courseRepository.GetWithLecturesAsync(request.CourseId, ct);
            if (course == null)
                throw new Exception("Course not found");

            // Check existing enrollment
            var existingEnrollment = await _enrollmentRepository
                .GetByStudentAndCourseAsync(request.StudentId, request.CourseId, request.EnrollmentDate, ct);

            if (existingEnrollment != null)
                throw new Exception("Already enrolled in this course");

            // Determine status
            string status = course.TypeStatus == Course.TypeFree
                ? Enrollment.StatusActive
                : Enrollment.StatusPending;

            // Create enrollment via factory
            var enrollment = Enrollment.Create(request.StudentId, request.CourseId, status, request.EnrollmentDate);

            // Save
            await _enrollmentRepository.AddAsync(enrollment, ct);
            await _unitOfWork.CommitAsync(ct);

            return _mapper.Map<EnrollmentDto>(enrollment);
        }
    }
}
