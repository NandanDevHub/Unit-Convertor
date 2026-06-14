using UnitConversionAPI.Core.Models;

namespace UnitConversionAPI.Core.Interfaces;

public interface IConversionService
{
    ConversionResult Convert(ConversionRequest request);
}
