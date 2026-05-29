using CompanyName.ProjectName.Application.Common.Models;
using CompanyName.ProjectName.Application.Features.ConversionExport.Commands;
using CompanyName.ProjectName.Application.Features.ConversionExport.DTOs;
using CompanyName.ProjectName.Application.Features.ConversionExport.Queries;

namespace CompanyName.ProjectName.Api.Controllers;

[ApiController]
[Route("api/conversions")]
public class ExportController(IMediator mediator) : ControllerBase
{
    [HttpGet("exports")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ConversionExportDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var response = await mediator.Send(new GetAllExportsQuery());

        return Ok(response);
    }

    [HttpPost("export")]
    [ProducesResponseType(typeof(ApiResponse<ConversionExportDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateConversionExportCommand command)
    {
        var response = await mediator.Send(command);

        return CreatedAtAction(nameof(GetAll), response);
    }
}