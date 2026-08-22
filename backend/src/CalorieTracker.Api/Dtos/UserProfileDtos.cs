using CalorieTracker.Domain.Entities;

namespace CalorieTracker.Api.Dtos;

public record UpsertProfileDto(
    decimal WeightKg,
    decimal HeightCm,
    int Age,
    Gender Gender,
    ActivityLevel ActivityLevel,
    Goal Goal,
    int? ManualCalorieTarget,
    int ProteinPercent,
    int FatPercent,
    int CarbsPercent);

public record ProfileWithTargetsDto(
    decimal WeightKg,
    decimal HeightCm,
    int Age,
    Gender Gender,
    ActivityLevel ActivityLevel,
    Goal Goal,
    int Calories,
    int ProteinGrams,
    int FatGrams,
    int CarbsGrams);