using CateringPlanet.Application.Abstractions;
using CateringPlanet.Application.DTOs;

namespace CateringPlanet.Tests.Fakes;

/// <summary>
/// In-memory <see cref="IAccountService"/> fake used by unit tests.
/// Tracks loyalty-points movements via <see cref="PointsCalls"/>.
/// </summary>
public class FakeAccountService : IAccountService
{
    public Dictionary<string, UserDto> Users { get; } = new();

    public Dictionary<string, int> Points { get; } = new();

    public List<(string UserId, int Delta)> PointsCalls { get; } = new();

    public Task<UserDto?> GetByIdAsync(string userId)
        => Task.FromResult(Users.TryGetValue(userId, out var user) ? user : null);

    public Task<UserDto?> GetByEmailAsync(string email)
        => Task.FromResult(Users.Values.FirstOrDefault(u => u.Email == email));

    public Task<IEnumerable<UserDto>> GetAllAsync() => Task.FromResult(Users.Values.AsEnumerable());

    public Task<(bool Succeeded, string? Error)> RegisterAsync(string email, string password, string firstName, string lastName, string role, string? phone = null, string? address = null)
        => Task.FromResult<(bool Succeeded, string? Error)>((false, null));

    public Task<(bool Succeeded, string? Error)> LoginAsync(string email, string password, bool rememberMe)
        => Task.FromResult<(bool Succeeded, string? Error)>((false, null));

    public Task<UserDto?> ValidateCredentialsAsync(string email, string password)
        => Task.FromResult<UserDto?>(null);

    public Task<IEnumerable<string>> GetRolesAsync(string userId)
        => Task.FromResult(Enumerable.Empty<string>());

    public Task LogoutAsync() => Task.CompletedTask;

    public Task<(bool Succeeded, string? Error)> UpdateProfileAsync(string userId, string firstName, string lastName, string phone, string address)
        => Task.FromResult<(bool Succeeded, string? Error)>((false, null));

    public Task<bool> IsInRoleAsync(string userId, string role) => Task.FromResult(false);

    public Task<int> GetPointsAsync(string userId)
        => Task.FromResult(Points.TryGetValue(userId, out var balance) ? balance : 0);

    public Task AddPointsAsync(string userId, int delta)
    {
        Points[userId] = (Points.TryGetValue(userId, out var balance) ? balance : 0) + delta;
        PointsCalls.Add((userId, delta));
        return Task.CompletedTask;
    }
}
