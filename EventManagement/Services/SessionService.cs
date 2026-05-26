using EventManagement.Data.Models;

namespace EventManagement.Services;

/// <summary>
/// Holds the currently logged-in user for the Blazor Server circuit (scoped per connection).
/// </summary>
public class SessionService
{
    public User? CurrentUser { get; private set; }

    public bool IsAuthenticated => CurrentUser != null;
    public bool IsAdmin => CurrentUser?.IsAdmin == true;

    public event Action? OnChange;

    public void SetUser(User user)
    {
        CurrentUser = user;
        NotifyStateChanged();
    }

    public void Logout()
    {
        CurrentUser = null;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
