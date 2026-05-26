using EventManagement.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User>              Users              => Set<User>();
    public DbSet<Event>             Events             => Set<Event>();
    public DbSet<EventRegistration> EventRegistrations => Set<EventRegistration>();
    public DbSet<Venue>             Venues             => Set<Venue>();
    public DbSet<Activity>          Activities         => Set<Activity>();
    public DbSet<EventVenue>        EventVenues        => Set<EventVenue>();
    public DbSet<EventActivity>     EventActivities    => Set<EventActivity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── User ───────────────────────────────────────────────────────
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // ── EventRegistration (User ↔ Event) ───────────────────────────
        modelBuilder.Entity<EventRegistration>()
            .HasIndex(r => new { r.UserId, r.EventId })
            .IsUnique();

        modelBuilder.Entity<EventRegistration>()
            .HasOne(r => r.User)
            .WithMany(u => u.Registrations)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EventRegistration>()
            .HasOne(r => r.Event)
            .WithMany(e => e.Registrations)
            .HasForeignKey(r => r.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        // ── EventVenue (Event ↔ Venue, many-to-many) ──────────────────
        modelBuilder.Entity<EventVenue>()
            .HasKey(ev => new { ev.EventId, ev.VenueId });

        modelBuilder.Entity<EventVenue>()
            .HasOne(ev => ev.Event)
            .WithMany(e => e.EventVenues)
            .HasForeignKey(ev => ev.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EventVenue>()
            .HasOne(ev => ev.Venue)
            .WithMany(v => v.EventVenues)
            .HasForeignKey(ev => ev.VenueId)
            .OnDelete(DeleteBehavior.Cascade);

        // ── EventActivity (Event ↔ Activity, many-to-many) ────────────
        modelBuilder.Entity<EventActivity>()
            .HasKey(ea => new { ea.EventId, ea.ActivityId });

        modelBuilder.Entity<EventActivity>()
            .HasOne(ea => ea.Event)
            .WithMany(e => e.EventActivities)
            .HasForeignKey(ea => ea.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EventActivity>()
            .HasOne(ea => ea.Activity)
            .WithMany(a => a.EventActivities)
            .HasForeignKey(ea => ea.ActivityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
