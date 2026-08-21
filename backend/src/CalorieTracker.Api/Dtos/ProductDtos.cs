namespace CalorieTracker.Api.Dtos;

public record ProductSearchResultDto(
    Guid Id,
    string Name,
    string? PreparationState,
    decimal CaloriesPer100g,
    decimal ProteinPer100g,
    decimal FatPer100g,
    decimal CarbsPer100g);

public record CreateProductDto(
    string Name,
    string? PreparationState,
    decimal CaloriesPer100g,
    decimal ProteinPer100g,
    decimal FatPer100g,
    decimal CarbsPer100g,
    string? Barcode);