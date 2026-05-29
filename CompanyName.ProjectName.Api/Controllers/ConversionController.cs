using CompanyName.ProjectName.Application.Common.Models;
using CompanyName.ProjectName.Application.Features.ConversionHistory.Commands;
using CompanyName.ProjectName.Application.Features.ConversionHistory.DTOs;
using CompanyName.ProjectName.Application.Features.ConversionHistory.Queries;

namespace CompanyName.ProjectName.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConversionController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<ConversionHistoryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var response = await mediator.Send(new GetAllConversionsQuery(page, pageSize));

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ConversionHistoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await mediator.Send(new GetConversionByIdQuery(id));

        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ConversionHistoryDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateConversionCommand command)
    {
        var response = await mediator.Send(command);

        return CreatedAtAction(nameof(GetById), new { id = response.Data!.Id }, response);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await mediator.Send(new DeleteConversionCommand(id));

        return Ok(response);
    }
}