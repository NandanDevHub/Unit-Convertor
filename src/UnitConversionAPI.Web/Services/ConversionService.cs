using UnitConversionAPI.Core.Exceptions;
using UnitConversionAPI.Core.Interfaces;
using UnitConversionAPI.Core.Models;

namespace UnitConversionAPI.Web.Services;

public class ConversionService : IConversionService
{
    private readonly IUnitRegistry _registry;

    public ConversionService(IUnitRegistry registry)
    {
        _registry = registry;
    }

    public ConversionResult Convert(ConversionRequest request)
    {
        var fromUnit = _registry.Find(request.From)
            ?? throw new UnitNotFoundException(request.From);

        var toUnit = _registry.Find(request.To)
            ?? throw new UnitNotFoundException(request.To);

        if (!fromUnit.Category.Equals(toUnit.Category, StringComparison.OrdinalIgnoreCase))
            throw new IncompatibleUnitsException(fromUnit.Category, toUnit.Category, fromUnit.Name, toUnit.Name);

        double baseValue = fromUnit.ToBase(request.Value);
        double result = toUnit.FromBase(baseValue);

        return new ConversionResult
        {
            InputValue       = request.Value,
            InputUnit        = fromUnit.Name,
            InputUnitSymbol  = fromUnit.Symbol,
            OutputValue      = result,
            OutputUnit       = toUnit.Name,
            OutputUnitSymbol = toUnit.Symbol,
            Category         = fromUnit.Category
        };
    }
}
