using EventManagement.Data;
using EventManagement.Data.Models;
using EventManagement.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// EF Core + SQLite
// ── Fix: always use ContentRootPath so the DB lives in the project folder
//    regardless of whether the app is launched from Visual Studio, dotnet run,
//    a published output, or any other working directory. ──────────────────────
var dbFolder = builder.Environment.ContentRootPath;
var dbFile   = Path.Combine(dbFolder, "eventmanagement.db");
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlite($"Data Source={dbFile}"));

// App services
builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<EventService>();
builder.Services.AddScoped<RegistrationService>();
builder.Services.AddScoped<VenueService>();
builder.Services.AddScoped<ActivityService>();
builder.Services.AddScoped<ParticipantService>();

var app = builder.Build();

// Initialize DB
// ── Fix: removed EnsureDeleted() — it was wiping the DB on every restart,
//    and creating a fresh database at whatever the current working-directory
//    happened to be (different in VS vs terminal).
//    Now we only create the schema if it doesn't exist, then seed. ──────────
using (var scope = app.Services.CreateScope())
{
    var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
    using var db = dbFactory.CreateDbContext();
    db.Database.EnsureCreated();   // create schema if not already present
    await SeedDataAsync(db);       // seed only if tables are empty (safe to call every start)
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

// ─── Seed ────────────────────────────────────────────────────────────────────
static async Task SeedDataAsync(AppDbContext db)
{
    // Users
    if (!db.Users.Any())
    {
        db.Users.AddRange(
            new User { FullName = "Admin", Email = "admin@event.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"), IsAdmin = true, CreatedAt = DateTime.UtcNow },
            new User { FullName = "Dipesh Pokhrel", Email = "dipesh@event.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"), IsAdmin = false, CreatedAt = DateTime.UtcNow }
        );
        await db.SaveChangesAsync();
    }

    // Venues
    if (!db.Venues.Any())
    {
        db.Venues.AddRange(
            new Venue { Name = "Kathmandu Convention Center", Address = "Bhrikutimandap, Kathmandu", City = "Kathmandu", Capacity = 1000, Description = "Premier convention center with state-of-the-art facilities.", CreatedAt = DateTime.UtcNow },
            new Venue { Name = "Innovation Hub Lalitpur", Address = "Pulchowk, Lalitpur", City = "Lalitpur", Capacity = 300, Description = "Modern co-working and event space ideal for tech meetups.", CreatedAt = DateTime.UtcNow },
            new Venue { Name = "Hotel Yak & Yeti Grand Hall", Address = "Durbar Marg, Kathmandu", City = "Kathmandu", Capacity = 500, Description = "Elegant banquet hall perfect for conferences and gala events.", CreatedAt = DateTime.UtcNow },
            new Venue { Name = "TechPark Auditorium", Address = "Subidhanagar, Kathmandu", City = "Kathmandu", Capacity = 200, Description = "Dedicated auditorium inside TechPark Nepal for tech talks.", CreatedAt = DateTime.UtcNow },
            new Venue { Name = "Pokhara Business Center", Address = "Lakeside, Pokhara", City = "Pokhara", Capacity = 250, Description = "Beautiful lakeside venue for seminars and corporate events.", CreatedAt = DateTime.UtcNow }
        );
        await db.SaveChangesAsync();
    }

    // Activities
    if (!db.Activities.Any())
    {
        db.Activities.AddRange(
            new Activity { Name = "Keynote Talk",             Description = "Inspiring keynote from industry leaders.",                          DurationMinutes = 60,  Type = "Talk",        CreatedAt = DateTime.UtcNow },
            new Activity { Name = "AI/ML Workshop",           Description = "Hands-on AI and Machine Learning workshop.",                         DurationMinutes = 180, Type = "Workshop",    CreatedAt = DateTime.UtcNow },
            new Activity { Name = "Startup Pitch Competition",Description = "Entrepreneurs pitch ideas to investors.",                            DurationMinutes = 120, Type = "Competition", CreatedAt = DateTime.UtcNow },
            new Activity { Name = "Panel Discussion",         Description = "Expert panel on trending topics with Q&A.",                          DurationMinutes = 90,  Type = "Panel",       CreatedAt = DateTime.UtcNow },
            new Activity { Name = "Networking Session",       Description = "Open networking with refreshments.",                                  DurationMinutes = 60,  Type = "Networking",  CreatedAt = DateTime.UtcNow },
            new Activity { Name = "Cloud Computing Demo",     Description = "Live demo of cloud architecture and deployment.",                    DurationMinutes = 90,  Type = "Demo",        CreatedAt = DateTime.UtcNow },
            new Activity { Name = "Yoga & Meditation",        Description = "Guided yoga and mindfulness session.",                                DurationMinutes = 60,  Type = "Wellness",    CreatedAt = DateTime.UtcNow },
            new Activity { Name = "React/Node.js Bootcamp",   Description = "Intensive coding session with React and Node.js.",                   DurationMinutes = 240, Type = "Workshop",    CreatedAt = DateTime.UtcNow },
            new Activity { Name = "Nutrition Talk",           Description = "Expert talk on balanced nutrition.",                                  DurationMinutes = 45,  Type = "Talk",        CreatedAt = DateTime.UtcNow },
            new Activity { Name = "Team Building Games",      Description = "Fun interactive games for teamwork and communication.",               DurationMinutes = 90,  Type = "Game",        CreatedAt = DateTime.UtcNow }
        );
        await db.SaveChangesAsync();
    }

    // Events
    if (!db.Events.Any())
    {
        db.Events.AddRange(
            new Event
            {
                Title = "Tech Conference 2026",
                Description = "Annual technology conference featuring AI, Cloud, and DevOps trends. Join hundreds of developers, engineers and innovators for a full day of learning, networking, and hands-on workshops.",
                Location = "Kathmandu", EventDate = new DateTime(2026, 8, 15, 9, 0, 0, DateTimeKind.Utc),
                MaxCapacity = 500, Category = "Technology", CreatedAt = DateTime.UtcNow,
                ImageUrl    = "https://images.unsplash.com/photo-1540575467063-178a50c2df87?w=900&auto=format&fit=crop",
                VideoUrl    = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
                WebsiteUrl  = "https://techconf2026.com",
                FacebookUrl = "https://facebook.com/techconf2026",
                TwitterUrl  = "https://twitter.com/techconf2026",
                LinkedInUrl = "https://linkedin.com/company/techconf2026",
                InstagramUrl= "https://instagram.com/techconf2026",
                YoutubeUrl  = "https://youtube.com/@techconf2026"
            },
            new Event
            {
                Title = "Startup Pitch Night",
                Description = "Entrepreneurs pitch their innovative ideas to a panel of seasoned investors and fellow founders. Top 3 pitches win seed funding. A fantastic networking opportunity for the entire startup ecosystem.",
                Location = "Lalitpur", EventDate = new DateTime(2026, 9, 5, 18, 0, 0, DateTimeKind.Utc),
                MaxCapacity = 200, Category = "Business", CreatedAt = DateTime.UtcNow,
                ImageUrl    = "https://images.unsplash.com/photo-1556761175-5973dc0f32e7?w=900&auto=format&fit=crop",
                WebsiteUrl  = "https://startuppitchnight.com",
                FacebookUrl = "https://facebook.com/startuppitch",
                TwitterUrl  = "https://twitter.com/startuppitch",
                LinkedInUrl = "https://linkedin.com/company/startuppitch",
                InstagramUrl= "https://instagram.com/startuppitch"
            },
            new Event
            {
                Title = "Health & Wellness Expo",
                Description = "A full-day expo covering mental health, physical fitness, nutrition, and holistic wellness practices. Expert speakers, live demonstrations, and interactive hands-on workshops for all levels.",
                Location = "Kathmandu", EventDate = new DateTime(2026, 10, 20, 10, 0, 0, DateTimeKind.Utc),
                MaxCapacity = 300, Category = "Health", CreatedAt = DateTime.UtcNow,
                ImageUrl    = "https://images.unsplash.com/photo-1544367567-0f2fcb009e0b?w=900&auto=format&fit=crop",
                VideoUrl    = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
                WebsiteUrl  = "https://wellnessexpo.com.np",
                FacebookUrl = "https://facebook.com/wellnessexponp",
                InstagramUrl= "https://instagram.com/wellnessexponp"
            },
            new Event
            {
                Title = "Web Development Bootcamp",
                Description = "An intensive 3-day bootcamp covering modern web development with React, Node.js, and cloud deployment strategies. Suitable for beginners and intermediate developers looking to level up their skills.",
                Location = "Kathmandu", EventDate = new DateTime(2026, 7, 10, 9, 0, 0, DateTimeKind.Utc),
                MaxCapacity = 50, Category = "Education", CreatedAt = DateTime.UtcNow,
                ImageUrl    = "https://images.unsplash.com/photo-1516321318423-f06f85e504b3?w=900&auto=format&fit=crop",
                VideoUrl    = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
                WebsiteUrl  = "https://webdevbootcamp.com.np",
                FacebookUrl = "https://facebook.com/webdevbootcamp",
                LinkedInUrl = "https://linkedin.com/company/webdevbootcamp",
                YoutubeUrl  = "https://youtube.com/@webdevbootcamp"
            },
            new Event
            {
                Title = "AI & Future of Work Summit",
                Description = "Explore how artificial intelligence is fundamentally reshaping industries and the future workforce. Renowned industry leaders share insights on automation, upskilling strategies, and the exciting opportunities ahead.",
                Location = "Pokhara", EventDate = new DateTime(2026, 11, 12, 9, 0, 0, DateTimeKind.Utc),
                MaxCapacity = 400, Category = "Technology", CreatedAt = DateTime.UtcNow,
                ImageUrl    = "https://images.unsplash.com/photo-1677442135703-1787eea5ce01?w=900&auto=format&fit=crop",
                VideoUrl    = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
                WebsiteUrl  = "https://aifuturesummit.com",
                FacebookUrl = "https://facebook.com/aifuturesummit",
                TwitterUrl  = "https://twitter.com/aifuturesummit",
                LinkedInUrl = "https://linkedin.com/company/aifuturesummit",
                YoutubeUrl  = "https://youtube.com/@aifuturesummit"
            }
        );
        await db.SaveChangesAsync();

        var venues = db.Venues.OrderBy(v => v.Id).ToList();
        var evs    = db.Events.OrderBy(e => e.Id).ToList();
        var acts   = db.Activities.OrderBy(a => a.Id).ToList();

        // Event-Venue links
        db.EventVenues.AddRange(
            new EventVenue { EventId = evs[0].Id, VenueId = venues[0].Id },
            new EventVenue { EventId = evs[0].Id, VenueId = venues[3].Id },
            new EventVenue { EventId = evs[1].Id, VenueId = venues[1].Id },
            new EventVenue { EventId = evs[2].Id, VenueId = venues[2].Id },
            new EventVenue { EventId = evs[3].Id, VenueId = venues[3].Id },
            new EventVenue { EventId = evs[4].Id, VenueId = venues[4].Id }
        );

        // Event-Activity links
        db.EventActivities.AddRange(
            // Tech Conf: Keynote, AI Workshop, Cloud Demo, Networking, Panel
            new EventActivity { EventId = evs[0].Id, ActivityId = acts[0].Id },
            new EventActivity { EventId = evs[0].Id, ActivityId = acts[1].Id },
            new EventActivity { EventId = evs[0].Id, ActivityId = acts[5].Id },
            new EventActivity { EventId = evs[0].Id, ActivityId = acts[4].Id },
            new EventActivity { EventId = evs[0].Id, ActivityId = acts[3].Id },
            // Startup Pitch: Competition, Keynote, Networking
            new EventActivity { EventId = evs[1].Id, ActivityId = acts[2].Id },
            new EventActivity { EventId = evs[1].Id, ActivityId = acts[0].Id },
            new EventActivity { EventId = evs[1].Id, ActivityId = acts[4].Id },
            // Health & Wellness: Yoga, Nutrition, Panel, Team Games
            new EventActivity { EventId = evs[2].Id, ActivityId = acts[6].Id },
            new EventActivity { EventId = evs[2].Id, ActivityId = acts[8].Id },
            new EventActivity { EventId = evs[2].Id, ActivityId = acts[3].Id },
            new EventActivity { EventId = evs[2].Id, ActivityId = acts[9].Id },
            // Web Dev Bootcamp: React Workshop, Cloud Demo, Networking
            new EventActivity { EventId = evs[3].Id, ActivityId = acts[7].Id },
            new EventActivity { EventId = evs[3].Id, ActivityId = acts[5].Id },
            new EventActivity { EventId = evs[3].Id, ActivityId = acts[4].Id },
            // AI Summit: Keynote, AI Workshop, Panel, Networking
            new EventActivity { EventId = evs[4].Id, ActivityId = acts[0].Id },
            new EventActivity { EventId = evs[4].Id, ActivityId = acts[1].Id },
            new EventActivity { EventId = evs[4].Id, ActivityId = acts[3].Id },
            new EventActivity { EventId = evs[4].Id, ActivityId = acts[4].Id }
        );

        await db.SaveChangesAsync();
    }
}
