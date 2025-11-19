using Application.Features.Students.Dtos;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

// افتراض وجود هذه الـ DTOs في المشروع
// using Application.Features.Courses.Dtos;
// using Application.Features.Lectures.Dtos;
// using Application.Features.Exams.Dtos;
// using Application.Features.Enrollments.Dtos; 
// using Application.Features.Users.Dtos; 

namespace Application.Common.Behaviours.Interfaces
{
    // تم حذف using Domain.Common.Results;

    public interface IStudentService
    {
        #region Curd

        // تغيير: Task<Result<StudentDto>> إلى Task<StudentDto>
        Task<StudentDto> CreateStudentAsync(CreateStudentDto dto, CancellationToken ct);

        // تغيير: Task<Result<StudentDto>> إلى Task<StudentDto>
        Task<StudentDto> UpdateStudentAsync(Guid id, StudentDto dto);

        Task<IEnumerable<StudentDto>> GetStudentsByCourseAsync(Guid courseId, CancellationToken ct);
        Task<StudentDto> GetStudentAsync(Guid studentId, CancellationToken ct);

        #endregion

        #region Enrollment

        // تم تفعيلها (نفترض وجود StudentWithEnrollmentsDto)
        Task<StudentWithEnrollmentsDto> GetWithEnrollmentsAsync(Guid studentId, CancellationToken ct);

        // تغيير: Task<Result<Success>> إلى Task (عملية كتابة لا تحتاج لقيمة إرجاع)
        Task EnrollInCourseAsync(Guid studentId, Guid courseId, CancellationToken ct);

        #endregion

        #region Courses & Lectures

        Task<IReadOnlyList<CourseDto>> GetCoursesAsync(Guid studentId); // only enrolled
        //Task<IReadOnlyList<LectureDto>> GetLecturesAsync(Guid studentId, Guid courseId);

        //#endregion


        //#region Exams

        //// تم تفعيلها (نفترض وجود ZoomDto)
        //Task<IReadOnlyList<ZoomDto>> GetExamsAsync(Guid studentId, Guid courseId); // only available exams

        //// تغيير: Task<Result<ExamSessionDto>> إلى Task<ExamSessionDto>
        //Task<ExamSessionDto> StartExamAsync(Guid studentId, Guid examId);

        //// تغيير: Task<Result<Success>> إلى Task (عملية كتابة لا تحتاج لقيمة إرجاع)
        //Task SubmitExamAnswersAsync(Guid studentId, Guid examId, List<StudentAnswerDto> answers);

        //Task<ExamResultDto> GetExamResultAsync(Guid studentId, Guid examId);

        #endregion
    }
}