using UnitConversionAPI.Core.Models;

namespace UnitConversionAPI.Core.Interfaces;

public interface IUnitRegistry
{
    UnitDefinition? Find(string unitId);
    IEnumerable<UnitDefinition> GetAll();
    IEnumerable<UnitDefinition> GetByCategory(string category);
    IEnumerable<string> GetCategories();
}
