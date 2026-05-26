using EventManagement.Data;
using EventManagement.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Services;

public class VenueService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;
    public VenueService(IDbContextFactory<AppDbContext> dbFactory) => _dbFactory = dbFactory;

    public async Task<List<Venue>> GetAllAsync()
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.Venues.OrderBy(v => v.Name).ToListAsync();
    }

    public async Task<Venue?> GetByIdAsync(int id)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.Venues
            .Include(v => v.EventVenues).ThenInclude(ev => ev.Event)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<Venue> CreateAsync(Venue venue)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        venue.CreatedAt = DateTime.UtcNow;
        db.Venues.Add(venue);
        await db.SaveChangesAsync();
        return venue;
    }

    public async Task<bool> UpdateAsync(Venue venue)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        db.Venues.Update(venue);
        return await db.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var venue = await db.Venues.FindAsync(id);
        if (venue == null) return false;
        db.Venues.Remove(venue);
        return await db.SaveChangesAsync() > 0;
    }
}
