using Edinburgh_Internation_Students.Data;
using Edinburgh_Internation_Students.DTOs.Common;
using Edinburgh_Internation_Students.DTOs.Events;
using Edinburgh_Internation_Students.DTOs.Users;
using Edinburgh_Internation_Students.Models;
using Microsoft.EntityFrameworkCore;

namespace Edinburgh_Internation_Students.Services;

public class EventService(ApplicationDbContext context, ILogger<EventService> logger) : IEventService
{
    public async Task<ApiResponse<List<EventDto>>> GetAllEventsAsync()
    {
        try
        {
            var events = await context.Events
                .Include(e => e.Organizer)
                .Include(e => e.Attendees)
                .OrderByDescending(e => e.Date)
                .Select(e => new EventDto
                {
                    Id = e.Id.ToString(),
                    Title = e.Title,
                    Description = e.Description,
                    Location = e.Location,
                    StartDate = e.Date.Add(e.Time),
                    EndDate = null,
                    ImageUrl = e.ImageUrl,
                    IsVirtual = false,
                    VirtualLink = null,
                    MaxAttendees = e.MaxAttendees,
                    AttendeeCount = e.Attendees.Count,
                    Status = e.IsCancelled ? "Cancelled" : GetEventStatus(e.Date),
                    GroupId = null,
                    OrganizerId = e.OrganizerId.ToString(),
                    CreatedAt = e.CreatedAt
                })
                .ToListAsync();

            return ApiResponse<List<EventDto>>.SuccessResponse(events);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving all events");
            return ApiResponse<List<EventDto>>.ErrorResponse(
                "Failed to retrieve events",
                new List<string> { ex.Message },
                500
            );
        }
    }

    public async Task<ApiResponse<EventDto>> GetEventByIdAsync(Guid eventId)
    {
        try
        {
            var ev = await context.Events
                .Include(e => e.Organizer)
                .Include(e => e.Attendees)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (ev == null)
            {
                return ApiResponse<EventDto>.ErrorResponse("Event not found", null, 404);
            }

            var eventDto = new EventDto
            {
                Id = ev.Id.ToString(),
                Title = ev.Title,
                Description = ev.Description,
                Location = ev.Location,
                StartDate = ev.Date.Add(ev.Time),
                EndDate = null,
                ImageUrl = ev.ImageUrl,
                IsVirtual = false,
                VirtualLink = null,
                MaxAttendees = ev.MaxAttendees,
                AttendeeCount = ev.Attendees.Count,
                Status = ev.IsCancelled ? "Cancelled" : GetEventStatus(ev.Date),
                GroupId = null,
                OrganizerId = ev.OrganizerId.ToString(),
                CreatedAt = ev.CreatedAt
            };

            return ApiResponse<EventDto>.SuccessResponse(eventDto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving event {EventId}", eventId);
            return ApiResponse<EventDto>.ErrorResponse(
                "Failed to retrieve event",
                new List<string> { ex.Message },
                500
            );
        }
    }

    public async Task<ApiResponse<EventDto>> CreateEventAsync(int creatorId, CreateEventRequest request)
    {
        try
        {
            var ev = new Event
            {
                Title = request.Title,
                Description = request.Description,
                Location = request.Location,
                Date = request.StartDate.Date,
                Time = request.StartDate.TimeOfDay,
                MaxAttendees = request.MaxAttendees,
                Category = Models.EventCategory.Social,
                OrganizerId = creatorId,
                IsCancelled = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await context.Events.AddAsync(ev);
            await context.SaveChangesAsync();

            // Automatically add creator as attendee
            var attendee = new EventAttendee
            {
                EventId = ev.Id,
                UserId = creatorId,
                RsvpAt = DateTime.UtcNow
            };

            await context.EventAttendees.AddAsync(attendee);
            await context.SaveChangesAsync();

            var eventDto = new EventDto
            {
                Id = ev.Id.ToString(),
                Title = ev.Title,
                Description = ev.Description,
                Location = ev.Location,
                StartDate = ev.Date.Add(ev.Time),
                EndDate = null,
                ImageUrl = ev.ImageUrl,
                IsVirtual = false,
                VirtualLink = null,
                MaxAttendees = ev.MaxAttendees,
                AttendeeCount = 1,
                Status = GetEventStatus(ev.Date),
                GroupId = null,
                OrganizerId = ev.OrganizerId.ToString(),
                CreatedAt = ev.CreatedAt
            };

            return ApiResponse<EventDto>.SuccessResponse(eventDto, "Event created successfully", 201);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating event");
            return ApiResponse<EventDto>.ErrorResponse(
                "Failed to create event",
                new List<string> { ex.Message },
                500
            );
        }
    }

    public async Task<ApiResponse<EventDto>> UpdateEventAsync(Guid eventId, int userId, UpdateEventRequest request)
    {
        try
        {
            var ev = await context.Events
                .Include(e => e.Attendees)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (ev == null)
            {
                return ApiResponse<EventDto>.ErrorResponse("Event not found", null, 404);
            }

            if (ev.OrganizerId != userId)
            {
                return ApiResponse<EventDto>.ErrorResponse(
                    "You don't have permission to update this event",
                    null,
                    403
                );
            }

            ev.Title = request.Title ?? ev.Title;
            ev.Description = request.Description ?? ev.Description;
            ev.Location = request.Location ?? ev.Location;
            if (request.StartDate.HasValue)
            {
                ev.Date = request.StartDate.Value.Date;
                ev.Time = request.StartDate.Value.TimeOfDay;
            }
            ev.MaxAttendees = request.MaxAttendees ?? ev.MaxAttendees;
            ev.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            var eventDto = new EventDto
            {
                Id = ev.Id.ToString(),
                Title = ev.Title,
                Description = ev.Description,
                Location = ev.Location,
                StartDate = ev.Date.Add(ev.Time),
                EndDate = null,
                ImageUrl = ev.ImageUrl,
                IsVirtual = false,
                VirtualLink = null,
                MaxAttendees = ev.MaxAttendees,
                AttendeeCount = ev.Attendees.Count,
                Status = ev.IsCancelled ? "Cancelled" : GetEventStatus(ev.Date),
                GroupId = null,
                OrganizerId = ev.OrganizerId.ToString(),
                CreatedAt = ev.CreatedAt
            };

            return ApiResponse<EventDto>.SuccessResponse(eventDto, "Event updated successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating event {EventId}", eventId);
            return ApiResponse<EventDto>.ErrorResponse(
                "Failed to update event",
                new List<string> { ex.Message },
                500
            );
        }
    }

    public async Task<ApiResponse<bool>> DeleteEventAsync(Guid eventId, int userId)
    {
        try
        {
            var ev = await context.Events.FirstOrDefaultAsync(e => e.Id == eventId);

            if (ev == null)
            {
                return ApiResponse<bool>.ErrorResponse("Event not found", null, 404);
            }

            if (ev.OrganizerId != userId)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "You don't have permission to delete this event",
                    null,
                    403
                );
            }

            context.Events.Remove(ev);
            await context.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Event deleted successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting event {EventId}", eventId);
            return ApiResponse<bool>.ErrorResponse(
                "Failed to delete event",
                new List<string> { ex.Message },
                500
            );
        }
    }

    public async Task<ApiResponse<EventAttendeeDto>> RsvpEventAsync(Guid eventId, int userId, string status)
    {
        try
        {
            var ev = await context.Events
                .Include(e => e.Attendees)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (ev == null)
            {
                return ApiResponse<EventAttendeeDto>.ErrorResponse("Event not found", null, 404);
            }

            // Check if max attendees reached (only for "Going" status)
            if (status == "Going" && ev.MaxAttendees.HasValue)
            {
                var goingCount = ev.Attendees.Count;
                if (goingCount >= ev.MaxAttendees.Value)
                {
                    return ApiResponse<EventAttendeeDto>.ErrorResponse(
                        "Event is full",
                        new List<string> { $"Maximum {ev.MaxAttendees.Value} attendees allowed" },
                        400
                    );
                }
            }

            var existingAttendee = await context.EventAttendees
                .FirstOrDefaultAsync(ea => ea.EventId == eventId && ea.UserId == userId);

            if (existingAttendee != null)
            {
                // Update existing RSVP - just update the timestamp
                existingAttendee.RsvpAt = DateTime.UtcNow;
            }
            else
            {
                // Create new RSVP
                existingAttendee = new EventAttendee
                {
                    EventId = eventId,
                    UserId = userId,
                    RsvpAt = DateTime.UtcNow
                };
                await context.EventAttendees.AddAsync(existingAttendee);
            }

            await context.SaveChangesAsync();

            var user = await context.Users.FindAsync(userId);

            var attendeeDto = new EventAttendeeDto
            {
                Id = existingAttendee.Id.ToString(),
                EventId = eventId.ToString(),
                UserId = userId.ToString(),
                Status = status,
                RegisteredAt = existingAttendee.RsvpAt,
                User = new UserSummaryDto
                {
                    Id = user!.Id.ToString(),
                    Name = $"{user.FirstName} {user.LastName}".Trim(),
                    AvatarUrl = null,
                    Country = null,
                    IsOnline = false
                }
            };

            return ApiResponse<EventAttendeeDto>.SuccessResponse(
                attendeeDto,
                "RSVP updated successfully"
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating RSVP for event {EventId}", eventId);
            return ApiResponse<EventAttendeeDto>.ErrorResponse(
                "Failed to update RSVP",
                new List<string> { ex.Message },
                500
            );
        }
    }

    public async Task<ApiResponse<List<EventAttendeeDto>>> GetEventAttendeesAsync(Guid eventId)
    {
        try
        {
            var eventExists = await context.Events.AnyAsync(e => e.Id == eventId);
            if (!eventExists)
            {
                return ApiResponse<List<EventAttendeeDto>>.ErrorResponse("Event not found", null, 404);
            }

            var attendees = await context.EventAttendees
                .Where(ea => ea.EventId == eventId)
                .Include(ea => ea.User)
                .Select(ea => new EventAttendeeDto
                {
                    Id = ea.Id.ToString(),
                    EventId = ea.EventId.ToString(),
                    UserId = ea.UserId.ToString(),
                    Status = "Going",
                    RegisteredAt = ea.RsvpAt,
                    User = new UserSummaryDto
                    {
                        Id = ea.User.Id.ToString(),
                        Name = $"{ea.User.FirstName} {ea.User.LastName}".Trim(),
                        AvatarUrl = null,
                        Country = null,
                        IsOnline = false
                    }
                })
                .ToListAsync();

            return ApiResponse<List<EventAttendeeDto>>.SuccessResponse(attendees);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving attendees for event {EventId}", eventId);
            return ApiResponse<List<EventAttendeeDto>>.ErrorResponse(
                "Failed to retrieve event attendees",
                new List<string> { ex.Message },
                500
            );
        }
    }

    private static string GetEventStatus(DateTime eventDate)
    {
        var now = DateTime.UtcNow.Date;
        if (eventDate.Date > now)
            return "Upcoming";
        if (eventDate.Date < now)
            return "Past";
        return "Ongoing";
    }
}
