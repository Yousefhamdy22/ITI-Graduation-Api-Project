using Application.Common.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ZoomServices.RecordingService.RecordingStateManager
{
    public interface IMeetingRecordingStateService 
    {
        void MarkMeetingEnded(string meetingId, ZoomWebhookPayload payload);
        void MarkRecordingCompleted(string meetingId, ZoomWebhookPayload payload);
        bool IsRecordingPending(string meetingId);
        ZoomWebhookPayload GetMeetingEndPayload(string meetingId);
        void RemoveMeetingState(string meetingId);
    }
}
