namespace EventManagement.Data.Models;

/// <summary>Junction table – Event ↔ Activity (many-to-many)</summary>
public class EventActivity
{
    public int EventId { get; set; }
    public Event Event { get; set; } = null!;

    public int ActivityId { get; set; }
    public Activity Activity { get; set; } = null!;
}
