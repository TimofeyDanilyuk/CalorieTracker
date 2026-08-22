using System.Security.Claims;
using CalorieTracker.Api.Dtos;
using CalorieTracker.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Api.Controllers;

[ApiController]
[Route("api/stats")]
[Authorize]
public class StatsController : ControllerBase
{
    private readonly AppDbContext _db;

    public StatsController(AppDbContext db) => _db = db;

    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // GET /api/stats/daily?from=2026-08-01&to=2026-08-22
    [HttpGet("daily")]
    public async Task<ActionResult<List<DailyStatDto>>> Daily(DateOnly from, DateOnly to)
    {
        var userId = CurrentUserId;
        var fromDate = from.ToDateTime(TimeOnly.MinValue);
        var toDate = to.ToDateTime(TimeOnly.MinValue).AddDays(1);

        var entries = await _db.DiaryEntries
            .Include(e => e.Product)
            .Where(e => e.UserId == userId && e.EatenAt >= fromDate && e.EatenAt < toDate)
            .ToListAsync();

        var result = entries
            .GroupBy(e => DateOnly.FromDateTime(e.EatenAt))
            .Select(g => new DailyStatDto(
                g.Key,
                Math.Round(g.Sum(e => e.Product.CaloriesPer100g * e.AmountGrams / 100), 1),
                Math.Round(g.Sum(e => e.Product.ProteinPer100g * e.AmountGrams / 100), 1),
                Math.Round(g.Sum(e => e.Product.FatPer100g * e.AmountGrams / 100), 1),
                Math.Round(g.Sum(e => e.Product.CarbsPer100g * e.AmountGrams / 100), 1)))
            .OrderBy(d => d.Date)
            .ToList();

        return Ok(result);
    }
}