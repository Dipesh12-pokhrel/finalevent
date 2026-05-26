using System.ComponentModel.DataAnnotations;

namespace EventManagement.Data.Models;

public class User
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required, MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public bool IsAdmin { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<EventRegistration> Registrations { get; set; } = new List<EventRegistration>();
}
