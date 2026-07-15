using Severina.Domain.ValueObjects;
using Xunit;

namespace Severina.Tests.Unit;

public class CnpjCpfTests
{
    [Theory]
    [InlineData("52998224725")]
    [InlineData("529.982.247-25")]
    [InlineData("11144477735")]
    [InlineData("111.444.777-35")]
    public void Create_ValidCpf_ReturnsValid(string cpf)
    {
        var result = CnpjCpf.Create(cpf);
        Assert.NotNull(result);
        Assert.True(result.IsCpf);
        Assert.Equal(11, result.Value.Length);
    }

    [Theory]
    [InlineData("52998224720")]
    [InlineData("11111111111")]
    [InlineData("22222222222")]
    [InlineData("00000000000")]
    public void Create_InvalidCpf_ThrowsArgumentException(string cpf)
    {
        Assert.Throws<ArgumentException>(() => CnpjCpf.Create(cpf));
    }

    [Theory]
    [InlineData("11222333000181")]
    [InlineData("11.222.333/0001-81")]
    [InlineData("11444777000161")]
    public void Create_ValidCnpj_ReturnsValid(string cnpj)
    {
        var result = CnpjCpf.Create(cnpj);
        Assert.NotNull(result);
        Assert.True(result.IsCnpj);
        Assert.Equal(14, result.Value.Length);
    }

    [Theory]
    [InlineData("11222333000100")]
    [InlineData("11111111111111")]
    [InlineData("22222222222222")]
    public void Create_InvalidCnpj_ThrowsArgumentException(string cnpj)
    {
        Assert.Throws<ArgumentException>(() => CnpjCpf.Create(cnpj));
    }

    [Fact]
    public void Create_EmptyString_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => CnpjCpf.Create(""));
        Assert.Throws<ArgumentException>(() => CnpjCpf.Create("   "));
    }

    [Fact]
    public void TryCreate_ValidCpf_ReturnsTrue()
    {
        Assert.True(CnpjCpf.TryCreate("52998224725", out var result));
        Assert.NotNull(result);
    }

    [Fact]
    public void TryCreate_InvalidCpf_ReturnsFalse()
    {
        Assert.False(CnpjCpf.TryCreate("00000000000", out var result));
        Assert.Null(result);
    }

    [Fact]
    public void Formatted_Cpf_ReturnsFormattedString()
    {
        var cnpjCpf = CnpjCpf.Create("52998224725");
        Assert.Equal("529.982.247-25", cnpjCpf.Formatted);
    }

    [Fact]
    public void Formatted_Cnpj_ReturnsFormattedString()
    {
        var cnpjCpf = CnpjCpf.Create("11222333000181");
        Assert.Equal("11.222.333/0001-81", cnpjCpf.Formatted);
    }

    [Fact]
    public void Equals_SameValue_ReturnsTrue()
    {
        var cnpjCpf1 = CnpjCpf.Create("52998224725");
        var cnpjCpf2 = CnpjCpf.Create("52998224725");
        Assert.Equal(cnpjCpf1, cnpjCpf2);
        Assert.True(cnpjCpf1 == cnpjCpf2);
    }

    [Fact]
    public void Equals_DifferentValue_ReturnsFalse()
    {
        var cnpjCpf1 = CnpjCpf.Create("52998224725");
        var cnpjCpf2 = CnpjCpf.Create("11144477735");
        Assert.NotEqual(cnpjCpf1, cnpjCpf2);
        Assert.True(cnpjCpf1 != cnpjCpf2);
    }

    [Fact]
    public void ImplicitConversion_ReturnsValue()
    {
        var cnpjCpf = CnpjCpf.Create("52998224725");
        string value = cnpjCpf;
        Assert.Equal("52998224725", value);
    }
}
