using Application.Features.Enrollments.Commands.CreateEnrollment;
using Application.Features.Enrollments.Commands.RemoveEnrollment;
using Application.Features.Enrollments.Commands.UpdateEnrollment;
using Application.Features.Enrollments.Dto;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Core.Entities.Courses;

namespace API.Controllers
{
    [Route("api/Enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public EnrollmentsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EnrollmentDetailsDto>> GetById(Guid id, CancellationToken ct)
        {
            var enrollment = await _unitOfWork.GetEnrollmentByIdAsync(id, ct);
            if (enrollment == null) return NotFound();

            var dto = new EnrollmentDetailsDto
            {
                StudentId = enrollment.StudentId,
                CourseId = enrollment.CourseId,
                EnrollmentDate = enrollment.EnrollmentDate,
                Status = enrollment.Status,
                Student = null,
                Course = null
            };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<EnrollmentDto>> Create([FromBody] EnrollInCourseCommand command, CancellationToken ct)
        {
            var created = await _unitOfWork.CreateEnrollmentAsync(command.StudentId, command.CourseId, command.EnrollmentDate, ct);

            if (created == null) return BadRequest("Unable to create enrollment.");

            var dto = new EnrollmentDto
            {
                StudentId = created.StudentId,
                CourseId = created.CourseId,
                EnrollmentDate = created.EnrollmentDate
            };

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<EnrollmentDto>> Update(Guid id, [FromBody] UpdateEnrollmentStatusCommand command, CancellationToken ct)
        {
            if (id != command.EnrollmentId)
                return BadRequest("ID mismatch");

            var updated = await _unitOfWork.UpdateEnrollmentStatusAsync(command.EnrollmentId, command.Status, command.Reason, ct);
            if (updated == null) return NotFound();

            var dto = new EnrollmentDto
            {
                StudentId = updated.StudentId,
                CourseId = updated.CourseId,
                EnrollmentDate = updated.EnrollmentDate
            };

            return Ok(dto);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id, [FromBody] CancelEnrollmentCommand command, CancellationToken ct)
        {
            var reason = command?.CancellationReason;
            var result = await _unitOfWork.CancelEnrollmentAsync(id, reason, ct);
            return result ? NoContent() : NotFound();
        }
    }
}
