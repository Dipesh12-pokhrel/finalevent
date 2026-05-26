using EventManagement.Data;
using EventManagement.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Services;

public class RegistrationService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;
    public RegistrationService(IDbContextFactory<AppDbContext> dbFactory) => _dbFactory = dbFactory;

    public async Task<bool> IsRegisteredAsync(int userId, int eventId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.EventRegistrations
            .AnyAsync(r => r.UserId == userId && r.EventId == eventId
                        && r.Status != RegistrationStatus.Cancelled);
    }

    public async Task<(bool Success, string Message)> RegisterAsync(int userId, int eventId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();

        var existing = await db.EventRegistrations
            .FirstOrDefaultAsync(r => r.UserId == userId && r.EventId == eventId);

        if (existing != null)
        {
            if (existing.Status == RegistrationStatus.Cancelled)
            {
                // Re-register
                var ev2 = await db.Events.Include(e => e.Registrations)
                    .FirstOrDefaultAsync(e => e.Id == eventId);
                var activeCount = ev2!.Registrations.Count(r => r.Status != RegistrationStatus.Cancelled);
                if (activeCount >= ev2.MaxCapacity)
                    return (false, "This event is fully booked.");
                existing.Status = RegistrationStatus.Confirmed;
                existing.RegisteredAt = DateTime.UtcNow;
                await db.SaveChangesAsync();
                return (true, "Successfully re-registered for the event!");
            }
            return (false, "You are already registered for this event.");
        }

        var ev = await db.Events.Include(e => e.Registrations)
            .FirstOrDefaultAsync(e => e.Id == eventId);
        if (ev == null) return (false, "Event not found.");

        var activeRegs = ev.Registrations.Count(r => r.Status != RegistrationStatus.Cancelled);
        if (activeRegs >= ev.MaxCapacity)
            return (false, "This event is fully booked.");

        db.EventRegistrations.Add(new EventRegistration
        {
            UserId       = userId,
            EventId      = eventId,
            RegisteredAt = DateTime.UtcNow,
            Status       = RegistrationStatus.Confirmed
        });
        await db.SaveChangesAsync();
        return (true, "Successfully registered for the event!");
    }

    public async Task<(bool Success, string Message)> UnregisterAsync(int userId, int eventId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var reg = await db.EventRegistrations
            .FirstOrDefaultAsync(r => r.UserId == userId && r.EventId == eventId);
        if (reg == null) return (false, "Registration not found.");

        reg.Status = RegistrationStatus.Cancelled;
        await db.SaveChangesAsync();
        return (true, "Successfully cancelled your registration.");
    }

    public async Task<List<EventRegistration>> GetUserRegistrationsAsync(int userId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.EventRegistrations
            .Include(r => r.Event).ThenInclude(e => e.EventVenues).ThenInclude(ev => ev.Venue)
            .Where(r => r.UserId == userId)
            .OrderBy(r => r.Event.EventDate)
            .ToListAsync();
    }

    public async Task<List<EventRegistration>> GetEventRegistrationsAsync(int eventId)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.EventRegistrations
            .Include(r => r.User)
            .Where(r => r.EventId == eventId)
            .ToListAsync();
    }

    public async Task<bool> UpdateStatusAsync(int registrationId, RegistrationStatus status)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var reg = await db.EventRegistrations.FindAsync(registrationId);
        if (reg == null) return false;
        reg.Status = status;
        return await db.SaveChangesAsync() > 0;
    }
}
