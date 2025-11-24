using Infrastructure.ZoomServices.Dtos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ZoomServices.RecordingService.RecordingStateManager
{
    public class MeetingRecordingStateService : IMeetingRecordingStateService
    {
        private readonly ConcurrentDictionary<string, MeetingState> _meetingStates = new();
        private readonly ILogger<MeetingRecordingStateService> _logger;

        public MeetingRecordingStateService(ILogger<MeetingRecordingStateService> logger)
        {
            _logger = logger;
        }

        public void MarkMeetingEnded(string meetingId, ZoomWebhookPayload payload)
        {
            _meetingStates.AddOrUpdate(meetingId,
                new MeetingState { MeetingEnded = true, MeetingEndPayload = payload },
                (key, existing) =>
                {
                    existing.MeetingEnded = true;
                    existing.MeetingEndPayload = payload;
                    return existing;
                });
        }

        public void MarkRecordingCompleted(string meetingId, ZoomWebhookPayload payload)
        {
            _meetingStates.AddOrUpdate(meetingId,
                new MeetingState { RecordingCompleted = true, RecordingPayload = payload },
                (key, existing) =>
                {
                    existing.RecordingCompleted = true;
                    existing.RecordingPayload = payload;
                    return existing;
                });
        }

        public bool IsRecordingPending(string meetingId)
        {
            return _meetingStates.TryGetValue(meetingId, out var state) &&
                   state.MeetingEnded && !state.RecordingCompleted;
        }

        public ZoomWebhookPayload GetMeetingEndPayload(string meetingId)
        {
            return _meetingStates.TryGetValue(meetingId, out var state) ?
                   state.MeetingEndPayload : null;
        }

        public void RemoveMeetingState(string meetingId)
        {
            _meetingStates.TryRemove(meetingId, out _);
        }

        private class MeetingState
        {
            public bool MeetingEnded { get; set; }
            public bool RecordingCompleted { get; set; }
            public ZoomWebhookPayload MeetingEndPayload { get; set; }
            public ZoomWebhookPayload RecordingPayload { get; set; }
        }
    }
}
