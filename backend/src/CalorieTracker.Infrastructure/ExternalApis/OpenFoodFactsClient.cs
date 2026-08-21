using System.Text.Json;
using System.Text.Json.Serialization;

namespace CalorieTracker.Infrastructure.ExternalApis;

public class OpenFoodFactsClient
{
    private readonly HttpClient _httpClient;

    public OpenFoodFactsClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://world.openfoodfacts.org/api/v2/");
    }

    public async Task<OpenFoodFactsProduct?> GetProductByBarcodeAsync(string barcode)
    {
        var response = await _httpClient.GetAsync($"product/{barcode}.json");

        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<OpenFoodFactsResponse>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        // status == 0 значит "продукт не найден" в терминах их API
        if (result is null || result.Status == 0 || result.Product is null)
            return null;

        return result.Product;
    }
}

public class OpenFoodFactsResponse
{
    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("product")]
    public OpenFoodFactsProduct? Product { get; set; }
}

public class OpenFoodFactsProduct
{
    [JsonPropertyName("product_name")]
    public string? ProductName { get; set; }

    [JsonPropertyName("nutriments")]
    public OpenFoodFactsNutriments? Nutriments { get; set; }
}

public class OpenFoodFactsNutriments
{
    [JsonPropertyName("energy-kcal_100g")]
    public decimal? CaloriesPer100g { get; set; }

    [JsonPropertyName("proteins_100g")]
    public decimal? ProteinPer100g { get; set; }

    [JsonPropertyName("fat_100g")]
    public decimal? FatPer100g { get; set; }

    [JsonPropertyName("carbohydrates_100g")]
    public decimal? CarbsPer100g { get; set; }
}