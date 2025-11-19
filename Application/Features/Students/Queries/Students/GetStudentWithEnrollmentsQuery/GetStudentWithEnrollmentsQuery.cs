using Application.Features.Students.Dtos;
using MediatR;
using System;

// يجب تعريف DTOs والواجهات المستخدمة في مشروعك
// using Application.Features.Students.Dtos;
// using Domain.Common.Interface; // نفترض وجود IBaseRequest, ICachedQuery هنا

namespace Application.Features.Students.Queries.Students.GetStudentWithEnrollmentsQuery
{
    // Record يمثل استعلام جلب تفاصيل الطالب مع تسجيلاته.
    // يطبق IRequest<T> (CQRS) وواجهات الكاش المخصصة (ICachedQuery).
    public record GetStudentWithEnrollmentsQuery : IRequest<StudentWithEnrollmentsDto>, IBaseRequest, ICachedQuery, IEquatable<GetStudentWithEnrollmentsQuery>
    {
        public GetStudentWithEnrollmentsQuery(Guid studentId)
        {
            StudentId = studentId;
        }

        // المدخل الأساسي للاستعلام
        public Guid StudentId { get; init; }

        // خصائص الكاش (Clean Code: توفير مفاتيح وتفاصيل الكاش مباشرة في الاستعلام)
        public string CacheKey => $"student-enrollments:{StudentId}";

        public TimeSpan Expiration => TimeSpan.FromMinutes(15);

        // Tags للمساعدة في مسح الكاش بواسطة علامة
        public string[]? Tags => new[] { "students", $"student:{StudentId}", "enrollments" };

        // يمكن إضافة constructor هنا إذا لزم تعيين قيم افتراضية أو تنفيذ منطق تهيئة معقد.
        // لكن إذا كان التعيين يتم عبر initializer، فلا حاجة له:
        /*
        public GetStudentWithEnrollmentsQuery(Guid studentId)
        {
            StudentId = studentId;
        }
        */
    }
}