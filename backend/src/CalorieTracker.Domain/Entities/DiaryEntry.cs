namespace CalorieTracker.Domain.Entities;

public class DiaryEntry
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public decimal AmountGrams { get; set; } // сколько грамм съедено
    public DateTime EatenAt { get; set; } = DateTime.UtcNow;

    // завтрак/обед/ужин/перекус — для удобной группировки в UI
    public string MealType { get; set; } = "snack";
}