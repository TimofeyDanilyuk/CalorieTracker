using System.Security.Claims;
using CalorieTracker.Api.Dtos;
using CalorieTracker.Domain.Entities;
using CalorieTracker.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Api.Controllers;

[ApiController]
[Route("api/diary")]
[Authorize]
public class DiaryController : ControllerBase
{
    private static readonly string[] MealOrder = { "breakfast", "lunch", "dinner", "snack" };

    private readonly AppDbContext _db;

    public DiaryController(AppDbContext db) => _db = db;

    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // GET /api/diary?date=2026-08-22
    [HttpGet]
    public async Task<ActionResult<DailyDiaryDto>> GetByDate(DateOnly date)
    {
        var userId = CurrentUserId;
        var dayStart = date.ToDateTime(TimeOnly.MinValue);
        var dayEnd = dayStart.AddDays(1);

        var entries = await _db.DiaryEntries
            .Include(e => e.Product)
            .Where(e => e.UserId == userId && e.EatenAt >= dayStart && e.EatenAt < dayEnd)
            .ToListAsync();

        var entryDtos = entries.Select(e => new DiaryEntryDto(
            e.Id, e.ProductId, e.Product.Name, e.AmountGrams, e.MealType, e.EatenAt,
            Math.Round(e.Product.CaloriesPer100g * e.AmountGrams / 100, 1),
            Math.Round(e.Product.ProteinPer100g * e.AmountGrams / 100, 1),
            Math.Round(e.Product.FatPer100g * e.AmountGrams / 100, 1),
            Math.Round(e.Product.CarbsPer100g * e.AmountGrams / 100, 1)
        )).ToList();

        var meals = MealOrder.Select(mealType =>
        {
            var mealEntries = entryDtos.Where(e => e.MealType == mealType).ToList();
            return new MealGroupDto(
                mealType,
                mealEntries.Sum(e => e.Calories),
                mealEntries.Sum(e => e.Protein),
                mealEntries.Sum(e => e.Fat),
                mealEntries.Sum(e => e.Carbs),
                mealEntries);
        }).ToList();

        return Ok(new DailyDiaryDto(
            date,
            entryDtos.Sum(e => e.Calories),
            entryDtos.Sum(e => e.Protein),
            entryDtos.Sum(e => e.Fat),
            entryDtos.Sum(e => e.Carbs),
            meals));
    }

    [HttpPost]
    public async Task<ActionResult<DiaryEntryDto>> Create(CreateDiaryEntryDto dto)
    {
        var product = await _db.Products.FindAsync(dto.ProductId);
        if (product is null) return NotFound(new { message = "Продукт не найден" });

        var entry = new DiaryEntry
        {
            Id = Guid.NewGuid(),
            UserId = CurrentUserId,
            ProductId = dto.ProductId,
            AmountGrams = dto.AmountGrams,
            MealType = dto.MealType,
            EatenAt = dto.EatenAt ?? DateTime.UtcNow
        };

        _db.DiaryEntries.Add(entry);
        await _db.SaveChangesAsync();

        return Ok(new DiaryEntryDto(
            entry.Id, product.Id, product.Name, entry.AmountGrams, entry.MealType, entry.EatenAt,
            Math.Round(product.CaloriesPer100g * entry.AmountGrams / 100, 1),
            Math.Round(product.ProteinPer100g * entry.AmountGrams / 100, 1),
            Math.Round(product.FatPer100g * entry.AmountGrams / 100, 1),
            Math.Round(product.CarbsPer100g * entry.AmountGrams / 100, 1)));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateDiaryEntryDto dto)
    {
        var entry = await _db.DiaryEntries.FirstOrDefaultAsync(e => e.Id == id && e.UserId == CurrentUserId);
        if (entry is null) return NotFound();

        entry.AmountGrams = dto.AmountGrams;
        entry.MealType = dto.MealType;
        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var entry = await _db.DiaryEntries.FirstOrDefaultAsync(e => e.Id == id && e.UserId == CurrentUserId);
        if (entry is null) return NotFound();

        _db.DiaryEntries.Remove(entry);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}