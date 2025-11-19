using Application.Features.Students.Dtos;
using Application.Features.Students.Queries.Students.GetStudentWithEnrollmentsQuery;
using MediatR;
using System;
// تم حذف using Domain.Common.Results;
// using Application.Features.Students.Dtos; // يجب إضافة DTOs المستخدمة في مشروعك
// using Application.Common.Behaviours.Interfaces; // ICachedQuery

namespace Application.Features.Students.Queries.Students.GetStudentById
{
    // تم تغيير قيمة الإرجاع من Result<StudentDto> إلى StudentDto
    public record GetStudentByIdQuery(Guid StudentId)
     : IRequest<StudentDto>, ICachedQuery
    {
        // خصائص الكاش
        public string CacheKey => $"student:{StudentId}";
        public TimeSpan Expiration => TimeSpan.FromMinutes(30);
        public string[]? Tags => new[] { "students", $"student:{StudentId}" };
    }
}