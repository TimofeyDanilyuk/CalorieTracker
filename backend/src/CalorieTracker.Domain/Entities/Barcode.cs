namespace CalorieTracker.Domain.Entities;

public class Barcode
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty; // EAN-13, UPC и т.д.

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
}