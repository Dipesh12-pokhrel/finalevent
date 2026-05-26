using System.ComponentModel.DataAnnotations;

namespace EventManagement.Data.Models;

public enum RegistrationStatus
{
    Pending = 0,
    Confirmed = 1,
    Cancelled = 2
}

public class EventRegistration
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int EventId { get; set; }
    public Event Event { get; set; } = null!;

    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    public RegistrationStatus Status { get; set; } = RegistrationStatus.Confirmed;

    [MaxLength(500)]
    public string? Notes { get; set; }
}
