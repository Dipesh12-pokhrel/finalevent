namespace EventManagement.Data.Models;

/// <summary>Junction table – Event ↔ Venue (many-to-many)</summary>
public class EventVenue
{
    public int EventId { get; set; }
    public Event Event { get; set; } = null!;

    public int VenueId { get; set; }
    public Venue Venue { get; set; } = null!;
}
