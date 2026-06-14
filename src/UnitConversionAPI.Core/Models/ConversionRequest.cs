namespace UnitConversionAPI.Core.Models;

public class ConversionRequest
{
    public double Value { get; init; }
    public required string From { get; init; }
    public required string To { get; init; }
}
