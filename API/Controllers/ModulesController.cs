//using Application.Features.Modules.Commands.CreateModules;
//using Application.Features.Modules.Commands.RemoveModule;
//using Application.Features.Modules.Commands.UpdateModules;
//using Application.Features.Modules.Dtos;
//using Application.Features.Modules.Queries.GetModulesBiIdQuery;
//using Application.Features.Modules.Queries.GetModulesQuery;
//using MediatR;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace nafizli_elearning.Controllers
//{
//    [Route("api/Modules")]
//    [ApiController]
//    public class ModulesController : ControllerBase
//    {
//        private readonly ILogger<ModulesController> _logger;
//        private readonly IMediator _mediator;
//        public ModulesController(ILogger<ModulesController> logger , IMediator mediator )
//        {
//            _logger = logger;
//            _mediator = mediator;
//        }

//        [HttpGet("{id}")]
//        public async Task<IActionResult> GetModuleById(Guid id)
//        {
//            var query = new GetModuleByIdQuery{ ModuleId = id };
//            var result = await _mediator.Send(query);

//            if (result.IsError)
//            {
//                return BadRequest(result.Errors);
//            }
//            return Ok();
//        }


//        //[HttpPut("{id}")]
//        //public async Task<IActionResult> UpdateModule(Guid id, [FromBody] UpdateModuleCommand request)
//        //{
//        //    if (id != request.ModuleId)
//        //    {
//        //        return BadRequest("Module ID mismatch.");
//        //    }
//        //    var result = await _mediator.Send(request);
//        //    if (result.IsError)
//        //    {
//        //        return BadRequest(result.Errors);
//        //    }
//        //    return NoContent();
//        //}

//        [HttpPut("{id}")]
//        public async Task<IActionResult> UpdateModule(Guid id, [FromBody] UpdatModuleDto body)
//        {
//            var command = new UpdateModuleCommand(
//                id,
//                body.Title,
//                body.Description
//            );

//            var result = await _mediator.Send(command);

//            if (result.IsError)
//                return BadRequest(result.Errors);

//            return NoContent();
//        }



//        [HttpDelete("{id}")]
//        public async Task<IActionResult> DeleteModule(Guid id)
//        {
//            var command = new DeleteModuleCommand(id);
//            var result = await _mediator.Send(command);
//            if (result.IsError)
//            {
//                return BadRequest(result.Errors);
//            }
//            return NoContent();
//        }
//        #region Old CreateModule Method

//        //[HttpPost]
//        //public async Task<IActionResult> CreateModule([FromBody] CreateModuleCommand request, CancellationToken ct)
//        //{
//        //    if (request == null)
//        //        return BadRequest("Invalid module data.");

//        //    try
//        //    {   
//        //        var command = new CreateModuleCommand(
//        //            request.Title,
//        //            request.Description,
//        //            request.CourseId
//        //        );


//        //        var result = await _mediator.Send(command, ct);


//        //        if (result.IsError)
//        //        {
//        //            return BadRequest(new
//        //            {
//        //                message = "Module creation failed.",
//        //                errors = result.Errors
//        //            });
//        //        }


//        //        return CreatedAtAction(
//        //            nameof(GetModuleById),    
//        //            new { id = result.Value.ModuleId },
//        //            result.Value
//        //        );
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        return StatusCode(500, new
//        //        {
//        //            message = "An error occurred while creating the module.",
//        //            error = ex.Message
//        //        });
//        //    }
//        //}


//        //[HttpPost]
//        //public async Task<IActionResult> CreateModule(CreateModuleCommand request)
//        //{
//        //    var command = new CreateModuleCommand(
//        //                        request.Title,
//        //                        request.Description,
//        //                        request.CourseId
//        //       );

//        //    var result = await _mediator.Send(command);

//        //    if (result.IsError)
//        //    {
//        //        return BadRequest(result.Errors);
//        //    }

//        //    return Ok();
//        //}
//        #endregion

//        [HttpGet("ByCourse/{courseId}")]
//        public async Task<IActionResult> GetModulesByCourseId(Guid courseId)
//        {
//            var query = new GetModulesByCourseIdQuery(courseId);
//            var result = await _mediator.Send(query);
//            if (result.IsError)
//            {
//                return BadRequest(result.Errors);
//            }
//            return Ok(result);
//        }
//    }
//}
