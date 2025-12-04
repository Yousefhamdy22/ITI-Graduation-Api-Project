using Infrastructure.ZoomServices.RecordingService.RecordingStateManager;

using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using Core.Interfaces.Services;
using Application.Common.Dtos;

namespace Presentation.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class ZoomWebHookController : ControllerBase
    {
        private readonly IMeetingRecordingStateService _stateService;
        private readonly IRecordingService _recordingService;
        private readonly ILogger<ZoomWebHookController> _logger;
        public ZoomWebHookController(
            IRecordingService recordingService,
            IMeetingRecordingStateService stateService,
            ILogger<ZoomWebHookController> logger)
        {
            _recordingService = recordingService;
            _stateService = stateService;
            _logger = logger;
        }

        [HttpPost] 
        public async Task<IActionResult> Receive(CancellationToken ct)
        {
            _logger.LogInformation("Webhook request received with headers: {@Headers}",
            Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()));


           Request.EnableBuffering(); 

            string requestBody;
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true))
            {
                requestBody = await reader.ReadToEndAsync();
                Request.Body.Position = 0;
            }

            var signature = Request.Headers["x-zm-signature"].FirstOrDefault();
            var timestamp = Request.Headers["x-zm-request-timestamp"].FirstOrDefault();

            if (string.IsNullOrEmpty(signature) || string.IsNullOrEmpty(timestamp))
            {
                _logger.LogWarning("Missing Zoom headers, skipping validation in dev mode");
            }
            else if (!ValidateZoomWebhook(signature, timestamp, requestBody))
            {
                _logger.LogWarning("Invalid Zoom webhook signature");
                return Unauthorized();
            }


            // Deserialize manually
            var payload = JsonSerializer.Deserialize<ZoomWebhookPayload>(requestBody);
            
            if (payload == null)
            {
                _logger.LogWarning("Failed to deserialize Zoom webhook payload");
                return BadRequest("Invalid payload format");
            }

            _logger.LogInformation("Received Zoom webhook for event: {Event}", payload.Event);

            // Handle different event types
            switch (payload.Event)
            {
                case "recording.completed":
                    await HandleRecordingCompletedEvent(payload, ct);
                    break;

                case "meeting.ended":
                    await HandleMeetingEndedEvent(payload, ct);
                    break;

                default:
                    _logger.LogInformation("Unhandled Zoom event type: {EventType}", payload.Event);
                    break;
            }

            return Ok();
        }


        private async Task HandleRecordingCompletedEvent(ZoomWebhookPayload payload, CancellationToken ct)
        {
            var meetingId = payload.Payload.Object.Id;

            //Tracking meet
            _stateService.MarkRecordingCompleted(meetingId, payload);

            _logger.LogInformation("Recording completed for meeting {MeetingId}", meetingId);

   
            foreach (var file in payload.Payload.Object.RecordingFiles)
            {
                await _recordingService.HandleRecordingCompletedAsync(
                    meetingId: long.Parse(meetingId),
                    recordingId: file.Id,
                    fileUrl: file.DownloadUrl,
                    fileType: file.FileType,
                    fileSize: file.FileSize,
                    start: file.RecordingStart,
                    end: file.RecordingEnd,
                    ct
                );
            }

            // Check if meeting already ended
            if (_stateService.IsRecordingPending(meetingId))
            {
                var meetingEndPayload = _stateService.GetMeetingEndPayload(meetingId);
                await TriggerFinalMeetingProcessing(meetingId, meetingEndPayload, payload, ct);
            }
        }
      
        private async Task HandleMeetingEndedEvent(ZoomWebhookPayload payload, CancellationToken ct)
        {
            var meetingId = payload.Payload.Object.Id;

            //  Track meeting end
            _stateService.MarkMeetingEnded(meetingId, payload);

            _logger.LogInformation("Meeting ended: ID={MeetingId}", meetingId);

            
            if (!_stateService.IsRecordingPending(meetingId))
            {
                await TriggerFinalMeetingProcessing(meetingId, payload, null, ct);
            }
            else
            {
                _logger.LogInformation("Waiting for recording completion for meeting {MeetingId}", meetingId);
            }
        }
        
        
        #region Helper
        private bool ValidateZoomWebhook(string signatureHeader, string timestamp, string body)
        {
            try
            {
                const string webhookSecret = "rvR42MOXQU2uv80UFXmt5A"; 
                var message = $"v0:{timestamp}:{body}";

                using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(webhookSecret));
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
                var expectedSignature = "v0=" + BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

                var isValid = signatureHeader == expectedSignature;

                if (!isValid)
                {
                    _logger.LogWarning("Zoom signature mismatch. Expected: {Expected}, Received: {Received}",
                        expectedSignature, signatureHeader);
                }

                return isValid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating Zoom webhook signature");
                return false;
            }
        }


        private async Task TriggerFinalMeetingProcessing(
                            string meetingId,
                            ZoomWebhookPayload meetingEndPayload,
                            ZoomWebhookPayload recordingPayload,
                            CancellationToken ct)
        {
            _logger.LogInformation("Final processing for meeting {MeetingId} - both events received", meetingId);

            // Add your final business logic here
            // Example: Update database, send notifications, etc.

            // Clean up state
            _stateService.RemoveMeetingState(meetingId);
        }
        #endregion
    }
}
