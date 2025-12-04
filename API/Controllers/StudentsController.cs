
using Application.Features.Students.Commands.StudentAnswer.SubmitStudentAnswerCommand;
using Application.Features.Students.Commands.Students.CreateStudent;
using Application.Features.Students.Commands.Students.RemoveStudent;

using Application.Features.Students.Queries.Students.GetAllStudentsQuery;
using Application.Features.Students.Queries.Students.GetAllStudentsWithEnrollmentsQuery;
using Application.Features.Students.Queries.Students.GetStudentById;
using Application.Features.Students.Queries.Students.GetStudentCourseLecture;
using Application.Features.Students.Queries.Students.GetStudentWithEnrollmentsQuery;
using Core.Common.Results;
using Core.Entities.Students;
using Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [Route("api/Students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {

        private readonly IMediator _mediator;
  
        private readonly AppDBContext _context;


        public StudentsController(IMediator mediator, AppDBContext context)
        {
            _mediator = mediator;
           _context = context;
      
        }



        [HttpPost]
        public Task<IActionResult> CreateStudent([FromBody] CreateStudentCommand command, CancellationToken ct)
        {
           
            //var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            //if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            //    return Task.FromResult<IActionResult>(Unauthorized("Invalid user token"));


            return _mediator.Send(command, ct)
                .ContinueWith<IActionResult>(t =>
                {
                    var result = t.Result;
                    if (result.IsSuccess)
                        return CreatedAtAction(nameof(GetStudentById),
                            new { id = result.Value.Id }, result.Value);
                    return BadRequest(result.Errors);
                });
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<Student>> GetStudentByUserId(Guid userId)
        {
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (student == null)
                return NotFound();

            return student; 
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStudents()
        {
            try
            {
                var result = await _mediator.Send(new GetAllStudentsQuery());
                return Ok(result.Value);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentById(Guid id)
        {
            try
            {
                var result = await _mediator.Send(new GetStudentByIdQuery(id));

                if (result == null)
                    return NotFound($"Student with ID {id} not found");

                return Ok(result.Value);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpGet("CourseEnrollment/{StudentId:guid}")]
        public async Task<IActionResult> EnrollmentCourse(Guid StudentId)
        {
            var result = await _mediator.Send(new GetStudentWithEnrollmentsQuery(StudentId));

            if (result == null)
                return NotFound($"No enrollments found for course with Id {StudentId}.");

            return Ok(result.Value);
        }

        //[HttpGet("{studentId}/lectures")]
        //public async Task<IActionResult> GetStudentLectures(Guid studentId)
        //{
        //    var result = await _mediator.Send(new GetStudentLecturesQuery(studentId));
        //    return Ok(result.Value);
        //}


        [HttpPost("submit")]
        public async Task<ActionResult<Result<Guid>>> SubmitAnswer([FromBody] SubmitStudentAnswerCommand request,
            CancellationToken ct)
        {
            
           
            var result = await _mediator.Send(request, ct);

            if (result.IsError)
                return BadRequest(result.Errors);

            return Ok(new { StudentAnswerId = result.Value });
        }

        [HttpGet("GetAllWithEnrollments")]
        public async Task<IActionResult> GetAllStudentsWithEnrollments()
        {
            try
            {
                var result = await _mediator.Send(new GetAllStudentsWithEnrollmentsQuery());
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(Guid id, CancellationToken ct)
        {
            try
            {
                var result = await _mediator.Send(new DeleteStudentCommand(id), ct);
                if (result.IsSuccess)
                    return NoContent();
                return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
} 