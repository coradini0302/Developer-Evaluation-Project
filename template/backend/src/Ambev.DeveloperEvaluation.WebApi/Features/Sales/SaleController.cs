using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class SalesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public SalesController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(CreateSaleRequest request)
    {
        var command = _mapper.Map<CreateSaleCommand>(request);

        var result = await _mediator.Send(command);

        return Ok(new CreateSaleResponse { Id = result });
    }

    //[Authorize]
    [HttpDelete("{id}/cancel")]
    public async Task<IActionResult> CancelSale(Guid id, CancellationToken cancellationToken)
    {
        var command = new CancelSaleCommand { Id = id };

        await _mediator.Send(command, cancellationToken);

        return Ok( "Sale cancelled successfully");
    }

    protected IActionResult Ok(string message) =>
        base.Ok(new ApiResponse
        {
            Success = true,
            Message = message
        });

}