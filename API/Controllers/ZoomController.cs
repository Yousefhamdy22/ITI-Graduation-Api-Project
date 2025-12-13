using Application.Features.Zoom.Commands.CreateZoomMeeting;
using Application.Features.Zoom.Queries.GetZoomMeetingsQuery;
using Core.Interfaces.Services;
using Infrastructure.ZoomServices;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace Presentation.Controllers
{
    [Route("api/Zoom")]
    [ApiController]
    public class ZoomController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IZoomService _zoomService;
        public ZoomController(IMediator mediator , IZoomService zoomService)
        {
            _mediator = mediator;
            _zoomService = zoomService;
        }



        [HttpPost]
        public async Task<IActionResult> Create(CreateZoomMeetingCommand command)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
          
                var meetingId = await _mediator.Send(command);
                return CreatedAtAction(
                    nameof(Create),
                    new { id = meetingId },
                    new { Id = meetingId }
                );
          
        }

        [HttpGet("id/{meetingId}")]
        public async Task<ActionResult> GetMeetById(long meetingId)
        {
            var query = new GetZoomMeetingsQuery(meetingId);
            var result = await _mediator.Send(query);

            if (result.IsError)
            {
                return BadRequest(result.Errors);
            }

            if (result.Value == null || !result.Value.Any())
            {
                return NotFound("No meetings found");
            }

            return Ok(result.Value);
        }

      
    }
}
