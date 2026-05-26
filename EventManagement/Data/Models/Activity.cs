using System.ComponentModel.DataAnnotations;

namespace EventManagement.Data.Models;

public class Activity
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    /// <summary>Duration in minutes</summary>
    public int DurationMinutes { get; set; } = 60;

    [MaxLength(100)]
    public string Type { get; set; } = "Workshop"; // Workshop, Talk, Game, Panel, Demo, etc.

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<EventActivity> EventActivities { get; set; } = new List<EventActivity>();
}
