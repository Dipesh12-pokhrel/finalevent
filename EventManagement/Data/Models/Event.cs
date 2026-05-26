using System.ComponentModel.DataAnnotations;

namespace EventManagement.Data.Models;

public class Event
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [MaxLength(300)]
    public string Location { get; set; } = string.Empty;

    [Required]
    public DateTime EventDate { get; set; }

    public int MaxCapacity { get; set; } = 100;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(100)]
    public string Category { get; set; } = "General";

    // ── Media ────────────────────────────────────
    /// <summary>Direct URL to a banner/cover image (Unsplash, CDN, etc.)</summary>
    [MaxLength(1000)]
    public string? ImageUrl { get; set; }

    /// <summary>YouTube or other video URL to embed on the detail page</summary>
    [MaxLength(1000)]
    public string? VideoUrl { get; set; }

    // ── Social / Contact links ───────────────────
    [MaxLength(500)]
    public string? WebsiteUrl { get; set; }

    [MaxLength(500)]
    public string? FacebookUrl { get; set; }

    [MaxLength(500)]
    public string? TwitterUrl { get; set; }

    [MaxLength(500)]
    public string? LinkedInUrl { get; set; }

    [MaxLength(500)]
    public string? InstagramUrl { get; set; }

    [MaxLength(500)]
    public string? YoutubeUrl { get; set; }

    // ── Navigation ───────────────────────────────
    public ICollection<EventRegistration> Registrations  { get; set; } = new List<EventRegistration>();
    public ICollection<EventVenue>        EventVenues    { get; set; } = new List<EventVenue>();
    public ICollection<EventActivity>     EventActivities{ get; set; } = new List<EventActivity>();
}
