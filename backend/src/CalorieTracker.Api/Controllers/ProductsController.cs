using CalorieTracker.Api.Dtos;
using CalorieTracker.Domain.Entities;
using CalorieTracker.Infrastructure.Data;
using CalorieTracker.Infrastructure.ExternalApis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly OpenFoodFactsClient _openFoodFacts;

    public ProductsController(AppDbContext db, OpenFoodFactsClient openFoodFacts)
    {
        _db = db;
        _openFoodFacts = openFoodFacts;
    }

    // GET /api/products/search?query=рис
    [HttpGet("search")]
    public async Task<ActionResult<List<ProductSearchResultDto>>> Search([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            return Ok(new List<ProductSearchResultDto>());

        var products = await _db.Products
            .Where(p => EF.Functions.Like(p.Name, $"%{query}%"))
            .OrderBy(p => p.Name)
            .Take(20)
            .Select(p => new ProductSearchResultDto(
                p.Id, p.Name, p.PreparationState,
                p.CaloriesPer100g, p.ProteinPer100g, p.FatPer100g, p.CarbsPer100g))
            .ToListAsync();

        return Ok(products);
    }

    // GET /api/products/barcode/1234567890123
    [HttpGet("barcode/{code}")]
    public async Task<ActionResult<ProductSearchResultDto>> GetByBarcode(string code)
    {
        // 1. Сначала своя БД - быстро и без внешних вызовов
        var existing = await _db.Barcodes
            .Include(b => b.Product)
            .FirstOrDefaultAsync(b => b.Code == code);

        if (existing is not null)
        {
            var p = existing.Product;
            return Ok(new ProductSearchResultDto(
                p.Id, p.Name, p.PreparationState,
                p.CaloriesPer100g, p.ProteinPer100g, p.FatPer100g, p.CarbsPer100g));
        }

        // 2. Если нет - идём в Open Food Facts
        var offProduct = await _openFoodFacts.GetProductByBarcodeAsync(code);

        if (offProduct is null || offProduct.Nutriments is null)
            return NotFound(new { message = "Продукт не найден. Добавьте вручную." });

        // 3. Сохраняем в свою БД, чтобы в следующий раз не ходить наружу
        var newProduct = new Product
        {
            Id = Guid.NewGuid(),
            Name = offProduct.ProductName ?? "Без названия",
            CaloriesPer100g = offProduct.Nutriments.CaloriesPer100g ?? 0,
            ProteinPer100g = offProduct.Nutriments.ProteinPer100g ?? 0,
            FatPer100g = offProduct.Nutriments.FatPer100g ?? 0,
            CarbsPer100g = offProduct.Nutriments.CarbsPer100g ?? 0,
            Source = "openfoodfacts"
        };

        newProduct.Barcodes.Add(new Barcode { Id = Guid.NewGuid(), Code = code, ProductId = newProduct.Id });

        _db.Products.Add(newProduct);
        await _db.SaveChangesAsync();

        return Ok(new ProductSearchResultDto(
            newProduct.Id, newProduct.Name, newProduct.PreparationState,
            newProduct.CaloriesPer100g, newProduct.ProteinPer100g, newProduct.FatPer100g, newProduct.CarbsPer100g));
    }

    // POST /api/products - ручное добавление (магазинный творожок, которого нет в OFF)
    [HttpPost]
    public async Task<ActionResult<ProductSearchResultDto>> Create(CreateProductDto dto)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            PreparationState = dto.PreparationState,
            CaloriesPer100g = dto.CaloriesPer100g,
            ProteinPer100g = dto.ProteinPer100g,
            FatPer100g = dto.FatPer100g,
            CarbsPer100g = dto.CarbsPer100g,
            Source = "user"
        };

        if (!string.IsNullOrWhiteSpace(dto.Barcode))
        {
            product.Barcodes.Add(new Barcode
            {
                Id = Guid.NewGuid(),
                Code = dto.Barcode,
                ProductId = product.Id
            });
        }

        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        return Ok(new ProductSearchResultDto(
            product.Id, product.Name, product.PreparationState,
            product.CaloriesPer100g, product.ProteinPer100g, product.FatPer100g, product.CarbsPer100g));
    }
}