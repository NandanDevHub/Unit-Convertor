namespace UnitConversionAPI.Core.Models;

public class UnitDefinition
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Symbol { get; init; }
    public required string Category { get; init; }

    // Converts a value in this unit to the category's base unit
    public required Func<double, double> ToBase { get; init; }

    // Converts a value from the category's base unit to this unit
    public required Func<double, double> FromBase { get; init; }
}
