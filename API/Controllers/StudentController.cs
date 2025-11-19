using Application.Common.Exceptions;
using Application.Features.Students.Commands.StudentAnswer.SubmitStudentAnswerCommand;
using Application.Features.Students.Commands.Students.RemoveStudent;
using Application.Features.Students.Commands.Students.UpdateStudent;
using Application.Features.Students.Dtos;
using Core.Entities;
using Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public StudentController(IUnitOfWork unitOfWork, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }

        [HttpGet("GetAllStudents")]
        public async Task<IActionResult> GetAllStudents(CancellationToken ct)
        {
            var students = await _unitOfWork.GetAllStudentsAsync(ct);
            return Ok(students);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentById(Guid id, CancellationToken ct)
        {
            try
            {
                var result = await _unitOfWork.GetStudentByIdAsync(id, ct);

                if (result == null)
                    return NotFound($"Student with ID {id} not found");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpGet("CourseEnrollment/{StudentId:guid}")]
        public async Task<IActionResult> EnrollmentCourse(Guid StudentId, CancellationToken ct)
        {
            var result = await _unitOfWork.GetStudentWithEnrollmentsAsync(StudentId, ct);

            if (result == null)
                return NotFound($"No enrollments found for student with Id {StudentId}.");

            return Ok(result);
        }


        [HttpPost("submit")]
        public async Task<IActionResult> SubmitAnswer([FromBody] SubmitStudentAnswerCommand request,
            CancellationToken ct)
        {
            if (request == null || request.Answers == null || !request.Answers.Any())
                return BadRequest("Request or answers cannot be empty.");

            // Convert answers to expected (QuestionId, SelectedAnswerId)
            var answers = new List<(Guid QuestionId, Guid SelectedAnswerId)>();

            foreach (var a in request.Answers)
            {
                if (a.QuestionId == Guid.Empty)
                    return BadRequest("QuestionId is required for each answer.");

                if (a.SelectedAnswerId == Guid.Empty)
                    return BadRequest($"SelectedAnswerId is required for question {a.QuestionId}.");

                answers.Add((QuestionId: a.QuestionId, SelectedAnswerId: a.SelectedAnswerId));
            }

            var result = await _unitOfWork.SubmitStudentAnswersAsync(request.ExamResultId, answers, ct);

            if (result == Guid.Empty)
                return BadRequest("Unable to submit answers. ExamResult may not exist or operation failed.");

            return Ok(new { StudentAnswerId = result });
        }

        // DTO for update endpoint
        public record UpdateStudentRequest(string Gender);

        /// <summary>
        /// Update student (partial/simple: update Gender).
        /// Uses MediatR handler UpdateStudentCommand -> returns StudentDto
        /// </summary>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateStudent(Guid id, [FromBody] UpdateStudentRequest request, CancellationToken ct)
        {
            if (request == null)
                return BadRequest("Request body is required.");

            if (string.IsNullOrWhiteSpace(request.Gender))
                return BadRequest("Gender is required.");

            try
            {
                var cmd = new UpdateStudentCommand(id, request.Gender);
                var updated = await _mediator.Send(cmd, ct);
                return Ok(updated);
            }
            catch (KeyNotFoundException knf)
            {
                return NotFound(knf.Message);
            }
            catch (InvalidOperationException ioe)
            {
                return Conflict(ioe.Message);
            }
            catch (ApplicationException ae)
            {
                return StatusCode(500, ae.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Delete student by id.
        /// Uses MediatR handler DeleteStudentCommand -> returns bool
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteStudent(Guid id, CancellationToken ct)
        {
            try
            {
                var cmd = new DeleteStudentCommand(id);
                var deleted = await _mediator.Send(cmd, ct);
                if (!deleted)
                    return BadRequest("Student could not be deleted.");
                return NoContent();
            }
            catch (KeyNotFoundException knf)
            {
                return NotFound(knf.Message);
            }
            catch (InvalidOperationException ioe)
            {
                return Conflict(ioe.Message);
            }
            catch (ApplicationException ae)
            {
                return StatusCode(500, ae.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
    