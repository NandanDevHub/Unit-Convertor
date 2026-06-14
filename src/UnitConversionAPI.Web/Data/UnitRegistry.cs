using UnitConversionAPI.Core.Interfaces;
using UnitConversionAPI.Core.Models;

namespace UnitConversionAPI.Web.Data;

public class UnitRegistry : IUnitRegistry
{
    private readonly Dictionary<string, UnitDefinition> _units;

    public UnitRegistry()
    {
        _units = BuildRegistry()
            .ToDictionary(u => u.Id, StringComparer.OrdinalIgnoreCase);
    }

    public UnitDefinition? Find(string unitId) =>
        _units.GetValueOrDefault(unitId);

    public IEnumerable<UnitDefinition> GetAll() => _units.Values;

    public IEnumerable<UnitDefinition> GetByCategory(string category) =>
        _units.Values.Where(u => u.Category.Equals(category, StringComparison.OrdinalIgnoreCase));

    public IEnumerable<string> GetCategories() =>
        _units.Values.Select(u => u.Category).Distinct(StringComparer.OrdinalIgnoreCase).Order();

    private static IEnumerable<UnitDefinition> BuildRegistry()
    {
        // --- Length (base: meter) ---
        yield return Linear("meter",       "Meter",       "m",   "length", 1.0);
        yield return Linear("kilometer",   "Kilometer",   "km",  "length", 1_000.0);
        yield return Linear("centimeter",  "Centimeter",  "cm",  "length", 0.01);
        yield return Linear("millimeter",  "Millimeter",  "mm",  "length", 0.001);
        yield return Linear("micrometer",  "Micrometer",  "μm",  "length", 0.000_001);
        yield return Linear("inch",        "Inch",        "in",  "length", 0.0254);
        yield return Linear("foot",        "Foot",        "ft",  "length", 0.3048);
        yield return Linear("yard",        "Yard",        "yd",  "length", 0.9144);
        yield return Linear("mile",        "Mile",        "mi",  "length", 1_609.344);
        yield return Linear("nautical-mile", "Nautical Mile", "nmi", "length", 1_852.0);

        // --- Temperature (base: Celsius, non-linear) ---
        yield return new UnitDefinition
        {
            Id = "celsius",    Name = "Celsius",    Symbol = "°C", Category = "temperature",
            ToBase   = v => v,
            FromBase = v => v
        };
        yield return new UnitDefinition
        {
            Id = "fahrenheit", Name = "Fahrenheit", Symbol = "°F", Category = "temperature",
            ToBase   = v => (v - 32.0) * 5.0 / 9.0,
            FromBase = v => v * 9.0 / 5.0 + 32.0
        };
        yield return new UnitDefinition
        {
            Id = "kelvin",     Name = "Kelvin",     Symbol = "K",  Category = "temperature",
            ToBase   = v => v - 273.15,
            FromBase = v => v + 273.15
        };
        yield return new UnitDefinition
        {
            Id = "rankine",    Name = "Rankine",    Symbol = "°R", Category = "temperature",
            ToBase   = v => (v - 491.67) * 5.0 / 9.0,
            FromBase = v => (v + 273.15) * 9.0 / 5.0
        };

        // --- Weight / Mass (base: kilogram) ---
        yield return Linear("kilogram",   "Kilogram",   "kg",  "weight", 1.0);
        yield return Linear("gram",       "Gram",       "g",   "weight", 0.001);
        yield return Linear("milligram",  "Milligram",  "mg",  "weight", 0.000_001);
        yield return Linear("metric-ton", "Metric Ton", "t",   "weight", 1_000.0);
        yield return Linear("pound",      "Pound",      "lb",  "weight", 0.453_592_37);
        yield return Linear("ounce",      "Ounce",      "oz",  "weight", 0.028_349_523_125);
        yield return Linear("stone",      "Stone",      "st",  "weight", 6.350_293_18);
        yield return Linear("short-ton",  "Short Ton",  "ton", "weight", 907.184_74);

        // --- Area (base: square meter) ---
        yield return Linear("square-meter",      "Square Meter",      "m²",  "area", 1.0);
        yield return Linear("square-kilometer",  "Square Kilometer",  "km²", "area", 1_000_000.0);
        yield return Linear("square-centimeter", "Square Centimeter", "cm²", "area", 0.0001);
        yield return Linear("square-foot",       "Square Foot",       "ft²", "area", 0.092_903_04);
        yield return Linear("square-inch",       "Square Inch",       "in²", "area", 0.000_645_16);
        yield return Linear("square-yard",       "Square Yard",       "yd²", "area", 0.836_127_36);
        yield return Linear("square-mile",       "Square Mile",       "mi²", "area", 2_589_988.110_336);
        yield return Linear("acre",              "Acre",              "ac",  "area", 4_046.856_422_4);
        yield return Linear("hectare",           "Hectare",           "ha",  "area", 10_000.0);

        // --- Volume (base: liter) ---
        yield return Linear("liter",        "Liter",          "L",   "volume", 1.0);
        yield return Linear("milliliter",   "Milliliter",     "mL",  "volume", 0.001);
        yield return Linear("cubic-meter",  "Cubic Meter",    "m³",  "volume", 1_000.0);
        yield return Linear("cubic-centimeter", "Cubic Centimeter", "cm³", "volume", 0.001);
        yield return Linear("gallon-us",    "Gallon (US)",    "gal", "volume", 3.785_411_784);
        yield return Linear("gallon-uk",    "Gallon (UK)",    "gal (UK)", "volume", 4.546_09);
        yield return Linear("fluid-ounce",  "Fluid Ounce (US)", "fl oz", "volume", 0.029_573_529_56);
        yield return Linear("pint-us",      "Pint (US)",      "pt",  "volume", 0.473_176_473);
        yield return Linear("cup",          "Cup (US)",       "cup", "volume", 0.236_588_236_5);
        yield return Linear("tablespoon",   "Tablespoon",     "tbsp","volume", 0.014_786_764_78);
        yield return Linear("teaspoon",     "Teaspoon",       "tsp", "volume", 0.004_928_921_593);

        // --- Speed (base: meter per second) ---
        yield return Linear("meter-per-second",  "Meter per Second",  "m/s",  "speed", 1.0);
        yield return Linear("kilometer-per-hour","Kilometer per Hour","km/h", "speed", 1.0 / 3.6);
        yield return Linear("mile-per-hour",     "Mile per Hour",     "mph",  "speed", 0.44704);
        yield return Linear("knot",              "Knot",              "kn",   "speed", 0.514_444);
        yield return Linear("foot-per-second",   "Foot per Second",   "ft/s", "speed", 0.3048);

        // --- Digital Storage (base: byte) ---
        yield return Linear("byte",      "Byte",      "B",   "storage", 1.0);
        yield return Linear("kilobyte",  "Kilobyte",  "KB",  "storage", 1_024.0);
        yield return Linear("megabyte",  "Megabyte",  "MB",  "storage", 1_048_576.0);
        yield return Linear("gigabyte",  "Gigabyte",  "GB",  "storage", 1_073_741_824.0);
        yield return Linear("terabyte",  "Terabyte",  "TB",  "storage", 1_099_511_627_776.0);
        yield return Linear("bit",       "Bit",       "bit", "storage", 0.125);
        yield return Linear("kilobit",   "Kilobit",   "Kbit","storage", 128.0);
        yield return Linear("megabit",   "Megabit",   "Mbit","storage", 131_072.0);
    }

    private static UnitDefinition Linear(string id, string name, string symbol, string category, double factorToBase) =>
        new()
        {
            Id       = id,
            Name     = name,
            Symbol   = symbol,
            Category = category,
            ToBase   = value => value * factorToBase,
            FromBase = value => value / factorToBase
        };
}
