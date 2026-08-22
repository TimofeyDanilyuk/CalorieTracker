namespace CalorieTracker.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public List<DiaryEntry> DiaryEntries { get; set; } = new();
    public UserProfile? Profile { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}