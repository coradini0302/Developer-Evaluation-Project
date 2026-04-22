using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, Guid>
{
    private readonly ISaleRepository _repository;
    private readonly ILogger<CreateSaleHandler> _logger;

    public CreateSaleHandler(ISaleRepository repository, ILogger<CreateSaleHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Starting creation of sale {SaleNumber} for customer {CustomerName}",
            request.SaleNumber,
            request.CustomerName
        );

        var sale = new Sale(
            request.SaleNumber,
            request.CustomerId,
            request.CustomerName,
            request.BranchId,
            request.BranchName
        );

        foreach (var item in request.Items)
        {
            _logger.LogInformation(
                "Adding item {ProductName} (Qty: {Quantity}, Price: {UnitPrice}) to sale {SaleNumber}",
                item.ProductName,
                item.Quantity,
                item.UnitPrice,
                request.SaleNumber
            );

            sale.AddItem(
                item.ProductId,
                item.ProductName,
                item.Quantity,
                item.UnitPrice
            );
        }

        await _repository.AddAsync(sale);

        // Simulação de evento de domínio
        _logger.LogInformation(
            "Event: SaleCreated | SaleId: {SaleId} | Customer: {CustomerName} | TotalAmount: {TotalAmount}",
            sale.Id,
            sale.CustomerName,
            sale.TotalAmount
        );

        return sale.Id;
    }
}