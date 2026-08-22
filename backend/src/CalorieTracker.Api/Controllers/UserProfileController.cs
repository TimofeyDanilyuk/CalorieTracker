using System.Security.Claims;
using CalorieTracker.Api.Dtos;
using CalorieTracker.Domain.Entities;
using CalorieTracker.Domain.Services;
using CalorieTracker.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Api.Controllers;

[ApiController]
[Route("api/profile")]
[Authorize]
public class UserProfileController : ControllerBase
{
    private readonly AppDbContext _db;

    public UserProfileController(AppDbContext db) => _db = db;

    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<ProfileWithTargetsDto>> Get()
    {
        var profile = await _db.UserProfiles.FirstOrDefaultAsync(p => p.UserId == CurrentUserId);
        if (profile is null) return NotFound();

        var targets = NutritionCalculator.Calculate(profile);

        return Ok(new ProfileWithTargetsDto(
            profile.WeightKg, profile.HeightCm, profile.Age, profile.Gender,
            profile.ActivityLevel, profile.Goal,
            targets.Calories, targets.ProteinGrams, targets.FatGrams, targets.CarbsGrams));
    }

    [HttpPut]
    public async Task<ActionResult<ProfileWithTargetsDto>> Upsert(UpsertProfileDto dto)
    {
        var userId = CurrentUserId;
        var profile = await _db.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile is null)
        {
            profile = new UserProfile { Id = Guid.NewGuid(), UserId = userId };
            _db.UserProfiles.Add(profile);
        }

        profile.WeightKg = dto.WeightKg;
        profile.HeightCm = dto.HeightCm;
        profile.Age = dto.Age;
        profile.Gender = dto.Gender;
        profile.ActivityLevel = dto.ActivityLevel;
        profile.Goal = dto.Goal;
        profile.ManualCalorieTarget = dto.ManualCalorieTarget;
        profile.ProteinPercent = dto.ProteinPercent;
        profile.FatPercent = dto.FatPercent;
        profile.CarbsPercent = dto.CarbsPercent;
        profile.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        var targets = NutritionCalculator.Calculate(profile);

        return Ok(new ProfileWithTargetsDto(
            profile.WeightKg, profile.HeightCm, profile.Age, profile.Gender,
            profile.ActivityLevel, profile.Goal,
            targets.Calories, targets.ProteinGrams, targets.FatGrams, targets.CarbsGrams));
    }
}