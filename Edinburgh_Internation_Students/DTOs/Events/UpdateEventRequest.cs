namespace Edinburgh_Internation_Students.DTOs.Events;

public class UpdateEventRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Location { get; set; }
    public bool? IsVirtual { get; set; }
    public string? VirtualLink { get; set; }
    public int? MaxAttendees { get; set; }
}
