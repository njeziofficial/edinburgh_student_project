namespace Edinburgh_Internation_Students.Configuration;

public class GroupRotationSettings
{
    public bool Enabled { get; set; } = true;
    public int IntervalInHours { get; set; } = 24;
    public int IntervalInMinutes { get; set; } = 0;
    public bool RunOnStartup { get; set; } = false;
}
