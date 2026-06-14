using Microsoft.AspNetCore.Mvc;
using UnitConversionAPI.Core.Interfaces;
using UnitConversionAPI.Core.Models;

namespace UnitConversionAPI.Web.Controllers;

[ApiController]
[Route("api")]
[Produces("application/json")]
public class ConversionController : ControllerBase
{
    private readonly IConversionService _conversionService;
    private readonly IUnitRegistry _unitRegistry;

    public ConversionController(IConversionService conversionService, IUnitRegistry unitRegistry)
    {
        _conversionService = conversionService;
        _unitRegistry = unitRegistry;
    }

    /// <summary>
    /// Converts a numeric value from one unit to another.
    /// </summary>
    [HttpGet("convert")]
    [ProducesResponseType(typeof(ConversionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Convert(
        [FromQuery] double value,
        [FromQuery] string from,
        [FromQuery] string to)
    {
        if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
            return BadRequest(new { error = "Query parameters 'from' and 'to' are required." });

        var result = _conversionService.Convert(new ConversionRequest
        {
            Value = value,
            From  = from.Trim(),
            To    = to.Trim()
        });

        return Ok(result);
    }

    /// <summary>
    /// Returns all supported unit categories along with their units.
    /// </summary>
    [HttpGet("units")]
    [ProducesResponseType(typeof(IEnumerable<CategoryUnits>), StatusCodes.Status200OK)]
    public IActionResult GetUnits()
    {
        var grouped = _unitRegistry.GetAll()
            .GroupBy(u => u.Category, StringComparer.OrdinalIgnoreCase)
            .OrderBy(g => g.Key)
            .Select(g => new CategoryUnits
            {
                Category = g.Key,
                Units = g.OrderBy(u => u.Name)
                         .Select(u => new UnitSummary { Id = u.Id, Name = u.Name, Symbol = u.Symbol })
            });

        return Ok(grouped);
    }

    /// <summary>
    /// Returns all units for a specific category.
    /// </summary>
    [HttpGet("units/{category}")]
    [ProducesResponseType(typeof(IEnumerable<UnitSummary>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetUnitsByCategory(string category)
    {
        var units = _unitRegistry.GetByCategory(category)
            .OrderBy(u => u.Name)
            .Select(u => new UnitSummary { Id = u.Id, Name = u.Name, Symbol = u.Symbol })
            .ToList();

        if (units.Count == 0)
            return NotFound(new { error = $"No units found for category '{category}'." });

        return Ok(units);
    }
}
