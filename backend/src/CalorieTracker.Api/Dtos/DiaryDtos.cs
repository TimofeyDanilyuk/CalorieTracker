namespace CalorieTracker.Api.Dtos;

public record DiaryEntryDto(
    Guid Id, Guid ProductId, string ProductName,
    decimal AmountGrams, string MealType, DateTime EatenAt,
    decimal Calories, decimal Protein, decimal Fat, decimal Carbs);

public record MealGroupDto(
    string MealType, decimal TotalCalories,
    decimal TotalProtein, decimal TotalFat, decimal TotalCarbs,
    List<DiaryEntryDto> Entries);

public record DailyDiaryDto(
    DateOnly Date, decimal TotalCalories,
    decimal TotalProtein, decimal TotalFat, decimal TotalCarbs,
    List<MealGroupDto> Meals);

public record CreateDiaryEntryDto(Guid ProductId, decimal AmountGrams, string MealType, DateTime? EatenAt);

public record UpdateDiaryEntryDto(decimal AmountGrams, string MealType);