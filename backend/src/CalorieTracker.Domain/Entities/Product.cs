namespace CalorieTracker.Domain.Entities;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // БЖУ и калории на 100г - базовая единица для всех расчётов
    public decimal CaloriesPer100g { get; set; }
    public decimal ProteinPer100g { get; set; }
    public decimal FatPer100g { get; set; }
    public decimal CarbsPer100g { get; set; }

    // например: "raw", "boiled", "fried" - чтобы отличать "рис сырой" от "рис варёный"
    public string? PreparationState { get; set; }

    // "user" - добавлено вручную, "openfoodfacts" - импортировано из внешнего API
    public string Source { get; set; } = "user";

    public List<Barcode> Barcodes { get; set; } = new();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}