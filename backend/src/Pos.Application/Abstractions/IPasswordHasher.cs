namespace Pos.Application.Abstractions;

public interface IPasswordHasher
{
    bool Verify(string plainText, string passwordHash);
}
