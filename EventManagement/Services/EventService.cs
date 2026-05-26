using EventManagement.Data;
using EventManagement.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Services;

public class EventService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;
    public EventService(IDbContextFactory<AppDbContext> dbFactory) => _dbFactory = dbFactory;

    public async Task<List<Event>> GetAllEventsAsync()
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.Events
            .Include(e => e.Registrations)
            .Include(e => e.EventVenues).ThenInclude(ev => ev.Venue)
            .Include(e => e.EventActivities).ThenInclude(ea => ea.Activity)
            .OrderBy(e => e.EventDate)
            .ToListAsync();
    }

    public async Task<Event?> GetEventByIdAsync(int id)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.Events
            .Include(e => e.Registrations).ThenInclude(r => r.User)
            .Include(e => e.EventVenues).ThenInclude(ev => ev.Venue)
            .Include(e => e.EventActivities).ThenInclude(ea => ea.Activity)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Event> CreateEventAsync(Event ev, List<int> venueIds, List<int> activityIds)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        ev.CreatedAt = DateTime.UtcNow;
        db.Events.Add(ev);
        await db.SaveChangesAsync();

        foreach (var vid in venueIds)
            db.EventVenues.Add(new EventVenue { EventId = ev.Id, VenueId = vid });
        foreach (var aid in activityIds)
            db.EventActivities.Add(new EventActivity { EventId = ev.Id, ActivityId = aid });

        await db.SaveChangesAsync();
        return ev;
    }

    public async Task<bool> UpdateEventAsync(Event ev, List<int> venueIds, List<int> activityIds)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var existing = await db.Events
            .Include(e => e.EventVenues)
            .Include(e => e.EventActivities)
            .FirstOrDefaultAsync(e => e.Id == ev.Id);
        if (existing == null) return false;

        existing.Title       = ev.Title;
        existing.Description = ev.Description;
        existing.Location    = ev.Location;
        existing.EventDate   = ev.EventDate;
        existing.MaxCapacity = ev.MaxCapacity;
        existing.Category    = ev.Category;
        existing.ImageUrl    = ev.ImageUrl;

        // Replace venue links
        db.EventVenues.RemoveRange(existing.EventVenues);
        foreach (var vid in venueIds)
            db.EventVenues.Add(new EventVenue { EventId = ev.Id, VenueId = vid });

        // Replace activity links
        db.EventActivities.RemoveRange(existing.EventActivities);
        foreach (var aid in activityIds)
            db.EventActivities.Add(new EventActivity { EventId = ev.Id, ActivityId = aid });

        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteEventAsync(int id)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var ev = await db.Events.FindAsync(id);
        if (ev == null) return false;
        db.Events.Remove(ev);
        await db.SaveChangesAsync();
        return true;
    }
}
