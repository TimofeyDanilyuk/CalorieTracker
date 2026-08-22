using CalorieTracker.Domain.Entities;

namespace CalorieTracker.Domain.Services;

public record NutritionTargets(int Calories, int ProteinGrams, int FatGrams, int CarbsGrams);

public static class NutritionCalculator
{
    private static readonly Dictionary<ActivityLevel, decimal> ActivityMultipliers = new()
    {
        [ActivityLevel.Sedentary] = 1.2m,
        [ActivityLevel.Light] = 1.375m,
        [ActivityLevel.Moderate] = 1.55m,
        [ActivityLevel.High] = 1.725m,
        [ActivityLevel.Athlete] = 1.9m
    };

    private static readonly Dictionary<Goal, decimal> GoalAdjustment = new()
    {
        [Goal.Lose] = -500,
        [Goal.Maintain] = 0,
        [Goal.Gain] = 500
    };

    public static NutritionTargets Calculate(UserProfile profile)
    {
        int calories;

        if (profile.ManualCalorieTarget is { } manual)
        {
            calories = manual;
        }
        else
        {
            // Mifflin-St Jeor: BMR
            var bmr = profile.Gender == Gender.Male
                ? 10 * profile.WeightKg + 6.25m * profile.HeightCm - 5 * profile.Age + 5
                : 10 * profile.WeightKg + 6.25m * profile.HeightCm - 5 * profile.Age - 161;

            var tdee = bmr * ActivityMultipliers[profile.ActivityLevel];
            calories = (int)Math.Round(tdee + GoalAdjustment[profile.Goal]);
        }

        // 1г белка/углеводов = 4 ккал, 1г жира = 9 ккал
        var proteinGrams = (int)Math.Round(calories * profile.ProteinPercent / 100m / 4);
        var fatGrams = (int)Math.Round(calories * profile.FatPercent / 100m / 9);
        var carbsGrams = (int)Math.Round(calories * profile.CarbsPercent / 100m / 4);

        return new NutritionTargets(calories, proteinGrams, fatGrams, carbsGrams);
    }
}