namespace UnitConversionAPI.Core.Models;

public class UnitSummary
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Symbol { get; init; }
}

public class CategoryUnits
{
    public required string Category { get; init; }
    public required IEnumerable<UnitSummary> Units { get; init; }
}
