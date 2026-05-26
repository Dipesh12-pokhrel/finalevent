using System.ComponentModel.DataAnnotations;

namespace EventManagement.Data.Models;

public class Venue
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(400)]
    public string Address { get; set; } = string.Empty;

    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    public int Capacity { get; set; } = 100;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<EventVenue> EventVenues { get; set; } = new List<EventVenue>();
}
