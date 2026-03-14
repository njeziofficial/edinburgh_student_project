namespace Edinburgh_Internation_Students.DTOs.Users;

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? Country { get; set; }
    public string? Campus { get; set; }
    public string? Major { get; set; }
    public string? Year { get; set; }
    public string? Bio { get; set; }
    public List<string> Interests { get; set; } = new();
    public List<string> Languages { get; set; } = new();
    public bool IsOnline { get; set; }
    public DateTime CreatedAt { get; set; }
}
