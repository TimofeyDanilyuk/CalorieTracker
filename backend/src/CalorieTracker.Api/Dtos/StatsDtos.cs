namespace CalorieTracker.Api.Dtos;

public record DailyStatDto(DateOnly Date, decimal Calories, decimal Protein, decimal Fat, decimal Carbs);