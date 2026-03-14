namespace Edinburgh_Internation_Students.Configuration;

public class OnlineStatusSettings
{
    public int OnlineThresholdMinutes { get; set; } = 10;
    public bool UpdateOnEveryRequest { get; set; } = true;
}
