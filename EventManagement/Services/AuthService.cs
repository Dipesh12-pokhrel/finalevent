using EventManagement.Data;
using EventManagement.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Services;

public class AuthService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;

    public AuthService(IDbContextFactory<AppDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<(bool Success, string Message, User? User)> LoginAsync(string email, string password)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        if (user == null)
            return (false, "Invalid email or password.", null);

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return (false, "Invalid email or password.", null);

        return (true, "Login successful.", user);
    }

    public async Task<(bool Success, string Message, User? User)> RegisterAsync(string fullName, string email, string password)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();

        if (await db.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower()))
            return (false, "An account with this email already exists.", null);

        var user = new User
        {
            FullName = fullName,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            IsAdmin = false
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();
        return (true, "Registration successful.", user);
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.Users.FindAsync(id);
    }
}
