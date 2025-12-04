using Application.Features.Lectures.Commands.createLectures;
using Application.Features.Lectures.Commands.RemoveLectures;
using Application.Features.Lectures.Dtos;
using Application.Features.Lectures.Queries.GetWithModules;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace nafizli_elearning.Controllers
{
    [Route("api/Lectures")]
    [ApiController]
    public class LecturesController : ControllerBase
    {
        private readonly ILogger<LecturesController> _logger;
        private readonly IMediator _mediator;
        public LecturesController(ILogger<LecturesController> logger , IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet("ByModule/{moduleId}")]
        public async Task<IActionResult> GetLecturesByModuleId(Guid moduleId)
        {
            var query = new GetLecturesByModuleIdQuery(moduleId);
            var result = await _mediator.Send(query);
            if (result.IsError)
            {
                return BadRequest(result.Errors);
            }
            return Ok(result.Value);
        }


        [HttpDelete("{lectureId}")]
        public async Task<IActionResult> DeleteLecture(Guid lectureId, CancellationToken ct)
        {
            var command = new DeleteLectureCommand();
            var result = await _mediator.Send(command, ct);
            if (result.IsError)
            {
                _logger.LogWarning("Failed to delete lecture {LectureId}: {Errors}", lectureId, result.Errors);
                return BadRequest(result.Errors);
            }
            _logger.LogInformation("Lecture {LectureId} deleted successfully.", lectureId);
            return Ok(new { Message = "Lecture deleted successfully." });
        }



        #region Create Lecture

        //[HttpPost]
        //public async Task<IActionResult> CreateLecture([FromBody] CreateLectureDto dto, CancellationToken ct)
        //{
        //    if (dto == null)
        //        return BadRequest("Invalid lecture data.");

        //    try
        //    {

        //        var command = new CreateLectureCommand(
        //            dto.Title,
        //            dto.ScheduledAt = DateTimeOffset.UtcNow.AddHours(1),
        //            TimeSpan.FromMinutes(60), // default duration; you can pass from dto if available

        //            dto.ModuleId,
        //            Guid.Empty,  // ZoomMeetingId will be auto-created in handler
        //            Guid.Empty   // ZoomRecordingId (no recording yet)
        //        );


        //        var result = await _mediator.Send(command, ct);

        //        if (!result.IsSuccess)
        //        {
        //            _logger.LogWarning("Lecture creation failed: {Error}", result.TopError.Description);
        //            return BadRequest(result.TopError.Description);
        //        }

        //        _logger.LogInformation("Lecture '{Title}' created successfully.", dto.Title);
        //        return Ok(result.Value);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error creating lecture with Title={Title}", dto.Title);
        //        return StatusCode(500, "An error occurred while creating the lecture.");
        //    }
        //}

        #endregion
    }
}
