using Severina.Infrastructure.Services;
using Xunit;

namespace Severina.Tests.Unit;

public class PasswordServiceTests
{
    private readonly PasswordService _service = new();

    [Theory]
    [InlineData("Test123!")]
    [InlineData("MyP@ssw0rd")]
    [InlineData("StrongP@ss1")]
    public void HashPassword_ReturnsNonEmptyHash(string password)
    {
        var hash = _service.HashPassword(password);
        Assert.False(string.IsNullOrEmpty(hash));
        Assert.NotEqual(password, hash);
    }

    [Fact]
    public void VerifyPassword_CorrectPassword_ReturnsTrue()
    {
        var password = "Test123!";
        var hash = _service.HashPassword(password);
        Assert.True(_service.VerifyPassword(password, hash));
    }

    [Fact]
    public void VerifyPassword_WrongPassword_ReturnsFalse()
    {
        var hash = _service.HashPassword("Test123!");
        Assert.False(_service.VerifyPassword("WrongPassword!", hash));
    }

    [Fact]
    public void HashPassword_DifferentHashesForSamePassword()
    {
        var password = "Test123!";
        var hash1 = _service.HashPassword(password);
        var hash2 = _service.HashPassword(password);
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void VerifyPassword_SameHashVerifiesDifferentPasswords_ReturnsFalse()
    {
        var hash = _service.HashPassword("Test123!");
        Assert.False(_service.VerifyPassword("Test1234!", hash));
        Assert.False(_service.VerifyPassword("test123!", hash));
        Assert.False(_service.VerifyPassword("Test123", hash));
    }
}
