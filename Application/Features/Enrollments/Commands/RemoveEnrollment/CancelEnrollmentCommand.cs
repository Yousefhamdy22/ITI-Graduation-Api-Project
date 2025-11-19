using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Enrollments.Commands.RemoveEnrollment
{
    public record CancelEnrollmentCommand : IRequest<bool>
    {
        // استخدام Guid كما هو محدد لديك
        public Guid EnrollmentId { get; init; }

        // سبب الإلغاء (لأغراض سجل التدقيق/Clean Code)
        public string? CancellationReason { get; init; }

        // يتم التعيين عبر الـ record initializer أو constructor بشكل نظيف
        public CancelEnrollmentCommand(Guid enrollmentId, string? cancellationReason)
        {
            EnrollmentId = enrollmentId;
            CancellationReason = cancellationReason;
        }
    }
}
