namespace CalorieTracker.Domain.Entities;

public enum Gender { Male, Female }
public enum ActivityLevel { Sedentary, Light, Moderate, High, Athlete }
public enum Goal { Lose, Maintain, Gain }

public class UserProfile
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public decimal WeightKg { get; set; }
    public decimal HeightCm { get; set; }
    public int Age { get; set; }
    public Gender Gender { get; set; }
    public ActivityLevel ActivityLevel { get; set; }
    public Goal Goal { get; set; }

    // если null - считаем автоматически по формуле; если задано вручную - используем это значение
    public int? ManualCalorieTarget { get; set; }

    // проценты БЖУ от итоговых калорий, по умолчанию 30/30/40
    public int ProteinPercent { get; set; } = 30;
    public int FatPercent { get; set; } = 30;
    public int CarbsPercent { get; set; } = 40;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}