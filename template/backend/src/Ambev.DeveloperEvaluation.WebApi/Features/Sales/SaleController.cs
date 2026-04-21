using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSales; // 🔥 NOVO
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

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

    // 🔥 CREATE
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(CreateSaleRequest request)
    {
        var command = _mapper.Map<CreateSaleCommand>(request);

        var result = await _mediator.Send(command);

        return Ok(new CreateSaleResponse { Id = result });
    }

    // 🔥 CANCEL
    [Authorize]
    [HttpDelete("{id}/cancel")]
    public async Task<IActionResult> CancelSale(Guid id, CancellationToken cancellationToken)
    {
        var command = new CancelSaleCommand { Id = id };

        await _mediator.Send(command, cancellationToken);

        return Ok("Sale cancelled successfully");
    }

    // 🔥 GET BY ID
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var request = new GetSaleRequest { Id = id };

        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }

    // 🔥 GET ALL (PAGINAÇÃO)
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var request = new GetSalesRequest
        {
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }

    // 🔥 PADRÃO DE RESPOSTA
    protected IActionResult Ok(string message) =>
        base.Ok(new ApiResponse
        {
            Success = true,
            Message = message
        });
}