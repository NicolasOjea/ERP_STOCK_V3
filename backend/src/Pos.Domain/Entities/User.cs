using Pos.Domain.Common;

namespace Pos.Domain.Entities;

public sealed class User : EntityBase
{
    private User()
    {
    }

    public User(Guid id, Guid tenantId, string username, string passwordHash, DateTimeOffset createdAtUtc, bool isActive = true)
        : base(id, tenantId, createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Username is required.", nameof(username));
        if (string.IsNullOrWhiteSpace(passwordHash)) throw new ArgumentException("PasswordHash is required.", nameof(passwordHash));

        Username = username;
        PasswordHash = passwordHash;
        IsActive = isActive;
    }

    public string Username { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
}
