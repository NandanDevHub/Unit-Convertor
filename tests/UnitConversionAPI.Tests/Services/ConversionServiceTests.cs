using FluentAssertions;
using NSubstitute;
using UnitConversionAPI.Core.Exceptions;
using UnitConversionAPI.Core.Interfaces;
using UnitConversionAPI.Core.Models;
using UnitConversionAPI.Web.Data;
using UnitConversionAPI.Web.Services;

namespace UnitConversionAPI.Tests.Services;

public class ConversionServiceTests
{
    private readonly IUnitRegistry _registry;
    private readonly ConversionService _sut;

    public ConversionServiceTests()
    {
        _registry = new UnitRegistry();
        _sut = new ConversionService(_registry);
    }

    // --- Length ---

    [Theory]
    [InlineData(1.0, "meter", "foot", 3.28083989501312)]
    [InlineData(1.0, "kilometer", "meter", 1000.0)]
    [InlineData(100.0, "centimeter", "meter", 1.0)]
    [InlineData(1.0, "mile", "kilometer", 1.609344)]
    [InlineData(1.0, "inch", "centimeter", 2.54)]
    [InlineData(1.0, "yard", "foot", 3.0)]
    public void Convert_Length_ReturnsCorrectValue(double input, string from, string to, double expected)
    {
        var result = _sut.Convert(new ConversionRequest { Value = input, From = from, To = to });

        result.OutputValue.Should().BeApproximately(expected, 1e-6);
        result.Category.Should().Be("length");
    }

    // --- Temperature ---

    [Theory]
    [InlineData(0.0,   "celsius",    "fahrenheit", 32.0)]
    [InlineData(100.0, "celsius",    "fahrenheit", 212.0)]
    [InlineData(32.0,  "fahrenheit", "celsius",    0.0)]
    [InlineData(0.0,   "celsius",    "kelvin",     273.15)]
    [InlineData(273.15,"kelvin",     "celsius",    0.0)]
    [InlineData(-40.0, "celsius",    "fahrenheit", -40.0)]  // the crossover point
    public void Convert_Temperature_ReturnsCorrectValue(double input, string from, string to, double expected)
    {
        var result = _sut.Convert(new ConversionRequest { Value = input, From = from, To = to });

        result.OutputValue.Should().BeApproximately(expected, 1e-6);
        result.Category.Should().Be("temperature");
    }

    // --- Weight ---

    [Theory]
    [InlineData(1.0,   "kilogram", "pound",   2.20462262184878)]
    [InlineData(1.0,   "pound",    "kilogram", 0.45359237)]
    [InlineData(1000.0,"gram",     "kilogram", 1.0)]
    [InlineData(1.0,   "kilogram", "ounce",   35.2739619495804)]
    public void Convert_Weight_ReturnsCorrectValue(double input, string from, string to, double expected)
    {
        var result = _sut.Convert(new ConversionRequest { Value = input, From = from, To = to });

        result.OutputValue.Should().BeApproximately(expected, 1e-6);
        result.Category.Should().Be("weight");
    }

    // --- Same unit (identity) ---

    [Theory]
    [InlineData(42.5, "meter")]
    [InlineData(100.0, "celsius")]
    [InlineData(1.0, "kilogram")]
    public void Convert_SameUnit_ReturnsSameValue(double value, string unit)
    {
        var result = _sut.Convert(new ConversionRequest { Value = value, From = unit, To = unit });

        result.OutputValue.Should().BeApproximately(value, 1e-9);
    }

    // --- Negative values ---

    [Fact]
    public void Convert_NegativeTemperature_WorksCorrectly()
    {
        var result = _sut.Convert(new ConversionRequest { Value = -40.0, From = "fahrenheit", To = "celsius" });

        result.OutputValue.Should().BeApproximately(-40.0, 1e-6);
    }

    // --- Error cases ---

    [Fact]
    public void Convert_UnknownFromUnit_ThrowsUnitNotFoundException()
    {
        var act = () => _sut.Convert(new ConversionRequest { Value = 1, From = "parsec", To = "meter" });

        act.Should().Throw<UnitNotFoundException>()
           .WithMessage("*parsec*");
    }

    [Fact]
    public void Convert_UnknownToUnit_ThrowsUnitNotFoundException()
    {
        var act = () => _sut.Convert(new ConversionRequest { Value = 1, From = "meter", To = "lightyear" });

        act.Should().Throw<UnitNotFoundException>()
           .WithMessage("*lightyear*");
    }

    [Fact]
    public void Convert_IncompatibleCategories_ThrowsIncompatibleUnitsException()
    {
        var act = () => _sut.Convert(new ConversionRequest { Value = 100, From = "celsius", To = "kilogram" });

        act.Should().Throw<IncompatibleUnitsException>();
    }

    [Fact]
    public void Convert_CaseInsensitiveUnitIds_Works()
    {
        var result = _sut.Convert(new ConversionRequest { Value = 1.0, From = "METER", To = "Foot" });

        result.OutputValue.Should().BeApproximately(3.28083989501312, 1e-6);
    }

    // --- Result shape ---

    [Fact]
    public void Convert_PopulatesAllResultFields()
    {
        var result = _sut.Convert(new ConversionRequest { Value = 1.0, From = "kilometer", To = "meter" });

        result.InputValue.Should().Be(1.0);
        result.InputUnit.Should().NotBeNullOrEmpty();
        result.InputUnitSymbol.Should().NotBeNullOrEmpty();
        result.OutputUnit.Should().NotBeNullOrEmpty();
        result.OutputUnitSymbol.Should().NotBeNullOrEmpty();
        result.Category.Should().Be("length");
    }
}
