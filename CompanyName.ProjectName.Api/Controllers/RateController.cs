using CompanyName.ProjectName.Application.Common.Models;
using CompanyName.ProjectName.Application.Features.ExchangeRate.DTOs;
using CompanyName.ProjectName.Application.Features.ExchangeRate.Queries;

namespace CompanyName.ProjectName.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RateController(IMediator mediator) : ControllerBase
{
    [HttpGet("{baseCurrency}")]
    [ProducesResponseType(typeof(ApiResponse<ExchangeRateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllRates(string baseCurrency)
    {
        var response = await mediator.Send(new GetAllRatesQuery(baseCurrency));

        return Ok(response);
    }

    [HttpGet("{fromCurrency}/{toCurrency}")]
    [ProducesResponseType(typeof(ApiResponse<ExchangeRatePairDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetRatePair(string fromCurrency, string toCurrency)
    {
        var response = await mediator.Send(new GetRatePairQuery(fromCurrency, toCurrency));

        return Ok(response);
    }
}