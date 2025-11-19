using Application.Features.Enrollments.Dto;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Enrollments.Commands.UpdateEnrollment
{
    public record UpdateEnrollmentStatusCommand : IRequest<EnrollmentDto>
    {
        public Guid EnrollmentId { get; init; }
        public string Status { get; init; } = string.Empty; // الحالة الجديدة للتسجيل
        public string? Reason { get; init; } // سبب تغيير الحالة (اختياري)
    }
}
