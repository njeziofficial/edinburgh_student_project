namespace Edinburgh_Internation_Students.DTOs.Users;

public class UpdateUserRequest
{
    public string? Name { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Country { get; set; }
    public string? Campus { get; set; }
    public string? Major { get; set; }
    public string? Year { get; set; }
    public string? Bio { get; set; }
    public List<string>? Interests { get; set; }
    public List<string>? Languages { get; set; }
}
