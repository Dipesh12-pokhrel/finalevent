using EventManagement.Data;
using EventManagement.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Services;

public class ActivityService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;
    public ActivityService(IDbContextFactory<AppDbContext> dbFactory) => _dbFactory = dbFactory;

    public async Task<List<Activity>> GetAllAsync()
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.Activities.OrderBy(a => a.Name).ToListAsync();
    }

    public async Task<Activity?> GetByIdAsync(int id)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.Activities
            .Include(a => a.EventActivities).ThenInclude(ea => ea.Event)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Activity> CreateAsync(Activity activity)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        activity.CreatedAt = DateTime.UtcNow;
        db.Activities.Add(activity);
        await db.SaveChangesAsync();
        return activity;
    }

    public async Task<bool> UpdateAsync(Activity activity)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        db.Activities.Update(activity);
        return await db.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var act = await db.Activities.FindAsync(id);
        if (act == null) return false;
        db.Activities.Remove(act);
        return await db.SaveChangesAsync() > 0;
    }
}
