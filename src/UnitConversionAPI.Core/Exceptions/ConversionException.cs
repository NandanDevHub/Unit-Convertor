namespace UnitConversionAPI.Core.Exceptions;

public class ConversionException : Exception
{
    public ConversionException(string message) : base(message) { }
}

public class UnitNotFoundException : ConversionException
{
    public string UnitId { get; }

    public UnitNotFoundException(string unitId)
        : base($"Unit '{unitId}' is not recognized. Use GET /api/units to see available units.")
    {
        UnitId = unitId;
    }
}

public class IncompatibleUnitsException : ConversionException
{
    public IncompatibleUnitsException(string fromCategory, string toCategory, string fromUnit, string toUnit)
        : base($"Cannot convert '{fromUnit}' ({fromCategory}) to '{toUnit}' ({toCategory}). Both units must belong to the same category.")
    {
    }
}
