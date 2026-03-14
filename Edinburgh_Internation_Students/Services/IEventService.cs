using Edinburgh_Internation_Students.DTOs.Common;
using Edinburgh_Internation_Students.DTOs.Events;

namespace Edinburgh_Internation_Students.Services;

public interface IEventService
{
    Task<ApiResponse<List<EventDto>>> GetAllEventsAsync();
    Task<ApiResponse<EventDto>> GetEventByIdAsync(Guid eventId);
    Task<ApiResponse<EventDto>> CreateEventAsync(int creatorId, CreateEventRequest request);
    Task<ApiResponse<EventDto>> UpdateEventAsync(Guid eventId, int userId, UpdateEventRequest request);
    Task<ApiResponse<bool>> DeleteEventAsync(Guid eventId, int userId);
    Task<ApiResponse<EventAttendeeDto>> RsvpEventAsync(Guid eventId, int userId, string status);
    Task<ApiResponse<List<EventAttendeeDto>>> GetEventAttendeesAsync(Guid eventId);
}
