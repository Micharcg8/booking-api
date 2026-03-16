using Microsoft.AspNetCore.Mvc;
using Booking.Contracts.Cities;

namespace Booking.Api.Controllers;

/// <summary>
/// City suggestions for search autocomplete (origin/destination).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CitiesController : ControllerBase
{
    private static readonly IReadOnlyList<CityDto> Cities = new List<CityDto>
    {
        new CityDto { Code = "MAD", Name = "Madrid" },
        new CityDto { Code = "BCN", Name = "Barcelona" },
        new CityDto { Code = "VAL", Name = "Valencia" },
        new CityDto { Code = "SVQ", Name = "Sevilla" },
        new CityDto { Code = "BIO", Name = "Bilbao" },
        new CityDto { Code = "PMI", Name = "Palma de Mallorca" },
        new CityDto { Code = "AGP", Name = "Málaga" },
        new CityDto { Code = "ALC", Name = "Alicante" },
        new CityDto { Code = "OVD", Name = "Asturias" },
        new CityDto { Code = "CDT", Name = "Castellón" },
        new CityDto { Code = "GRX", Name = "Granada" },
        new CityDto { Code = "XRY", Name = "Jerez" },
        new CityDto { Code = "LPA", Name = "Las Palmas" },
        new CityDto { Code = "TFN", Name = "Tenerife Norte" },
        new CityDto { Code = "TFS", Name = "Tenerife Sur" },
        new CityDto { Code = "VLC", Name = "Valencia" },
        new CityDto { Code = "VLL", Name = "Valladolid" },
        new CityDto { Code = "VGO", Name = "Vigo" },
        new CityDto { Code = "SCQ", Name = "Santiago de Compostela" },
        new CityDto { Code = "PAR", Name = "Paris" },
        new CityDto { Code = "LON", Name = "London" },
        new CityDto { Code = "ROM", Name = "Rome" },
        new CityDto { Code = "LIS", Name = "Lisbon" },
        new CityDto { Code = "AMS", Name = "Amsterdam" },
        new CityDto { Code = "BER", Name = "Berlin" },
        new CityDto { Code = "MUC", Name = "Munich" },
        new CityDto { Code = "VIE", Name = "Vienna" },
        new CityDto { Code = "BRU", Name = "Brussels" },
        new CityDto { Code = "DUB", Name = "Dublin" },
        new CityDto { Code = "NYC", Name = "New York" },
        new CityDto { Code = "MIA", Name = "Miami" },
        // México
        new CityDto { Code = "MEX", Name = "Ciudad de México" },
        new CityDto { Code = "GDL", Name = "Guadalajara" },
        new CityDto { Code = "MTY", Name = "Monterrey" },
        new CityDto { Code = "CUN", Name = "Cancún" },
        new CityDto { Code = "TIJ", Name = "Tijuana" },
        new CityDto { Code = "PVR", Name = "Puerto Vallarta" },
        new CityDto { Code = "SJD", Name = "Los Cabos" },
        new CityDto { Code = "MID", Name = "Mérida" },
        // Argentina
        new CityDto { Code = "EZE", Name = "Buenos Aires" },
        new CityDto { Code = "COR", Name = "Córdoba" },
        new CityDto { Code = "MDZ", Name = "Mendoza" },
        new CityDto { Code = "ROS", Name = "Rosario" },
        new CityDto { Code = "USH", Name = "Ushuaia" },
        new CityDto { Code = "IGR", Name = "Iguazú" },
        // Brasil
        new CityDto { Code = "GRU", Name = "São Paulo" },
        new CityDto { Code = "GIG", Name = "Río de Janeiro" },
        new CityDto { Code = "BSB", Name = "Brasília" },
        new CityDto { Code = "CNF", Name = "Belo Horizonte" },
        new CityDto { Code = "POA", Name = "Porto Alegre" },
        new CityDto { Code = "REC", Name = "Recife" },
        new CityDto { Code = "FOR", Name = "Fortaleza" },
        new CityDto { Code = "SSA", Name = "Salvador de Bahía" },
        new CityDto { Code = "CGH", Name = "São Paulo Congonhas" },
        // Chile
        new CityDto { Code = "SCL", Name = "Santiago de Chile" },
        new CityDto { Code = "PMC", Name = "Puerto Montt" },
        new CityDto { Code = "ANF", Name = "Antofagasta" },
        // Colombia
        new CityDto { Code = "BOG", Name = "Bogotá" },
        new CityDto { Code = "MDE", Name = "Medellín" },
        new CityDto { Code = "CTG", Name = "Cartagena" },
        new CityDto { Code = "CLO", Name = "Cali" },
        new CityDto { Code = "BAQ", Name = "Barranquilla" },
        // Perú
        new CityDto { Code = "LIM", Name = "Lima" },
        new CityDto { Code = "CUZ", Name = "Cusco" },
        new CityDto { Code = "AQP", Name = "Arequipa" },
        // Ecuador
        new CityDto { Code = "UIO", Name = "Quito" },
        new CityDto { Code = "GYE", Name = "Guayaquil" },
        // Venezuela
        new CityDto { Code = "CCS", Name = "Caracas" },
        new CityDto { Code = "MAR", Name = "Maracaibo" },
        // Uruguay
        new CityDto { Code = "MVD", Name = "Montevideo" },
        new CityDto { Code = "PDP", Name = "Punta del Este" },
        // Paraguay
        new CityDto { Code = "ASU", Name = "Asunción" },
        // Bolivia
        new CityDto { Code = "VVI", Name = "Santa Cruz de la Sierra" },
        new CityDto { Code = "LPB", Name = "La Paz" },
        // Centroamérica y Caribe
        new CityDto { Code = "SJO", Name = "San José (Costa Rica)" },
        new CityDto { Code = "PTY", Name = "Ciudad de Panamá" },
        new CityDto { Code = "HAV", Name = "La Habana" },
        new CityDto { Code = "VRA", Name = "Varadero" },
        new CityDto { Code = "SDQ", Name = "Santo Domingo" },
        new CityDto { Code = "PUJ", Name = "Punta Cana" },
        new CityDto { Code = "SJU", Name = "San Juan (Puerto Rico)" },
        new CityDto { Code = "GUA", Name = "Ciudad de Guatemala" },
        new CityDto { Code = "SAL", Name = "San Salvador" },
        new CityDto { Code = "TGU", Name = "Tegucigalpa" },
        new CityDto { Code = "MGA", Name = "Managua" },
    };

    /// <summary>
    /// Get city suggestions by partial name or code (min 2 characters).
    /// </summary>
    /// <param name="query">Search text (e.g. first letters of city name or code).</param>
    /// <param name="limit">Max number of results (default 10).</param>
    /// <response code="200">List of matching cities.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CityDto>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<CityDto>> Get([FromQuery] string? query = null, [FromQuery] int limit = 10)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            return Ok(Array.Empty<CityDto>());

        var q = query.Trim();
        var results = Cities
            .Where(c =>
                c.Code.StartsWith(q, StringComparison.OrdinalIgnoreCase) ||
                c.Name.StartsWith(q, StringComparison.OrdinalIgnoreCase) ||
                c.Name.Contains(q, StringComparison.OrdinalIgnoreCase))
            .Take(Math.Clamp(limit, 1, 20))
            .ToList();

        return Ok(results);
    }
}
