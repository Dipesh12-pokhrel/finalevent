using EventManagement.Data;
using EventManagement.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Services;

public class ParticipantService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;
    public ParticipantService(IDbContextFactory<AppDbContext> dbFactory) => _dbFactory = dbFactory;

    public async Task<List<User>> GetAllAsync()
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.Users
            .Include(u => u.Registrations).ThenInclude(r => r.Event)
            .OrderBy(u => u.FullName)
            .ToListAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.Users
            .Include(u => u.Registrations).ThenInclude(r => r.Event)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var user = await db.Users.FindAsync(id);
        if (user == null) return false;
        db.Users.Remove(user);
        return await db.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateStatusAsync(int id, bool isAdmin)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var user = await db.Users.FindAsync(id);
        if (user == null) return false;
        user.IsAdmin = isAdmin;
        return await db.SaveChangesAsync() > 0;
    }
}
