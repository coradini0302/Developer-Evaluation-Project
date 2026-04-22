using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand>
{
    private readonly ISaleRepository _repository;
    private readonly ILogger<UpdateSaleHandler> _logger;

    public UpdateSaleHandler(ISaleRepository repository, ILogger<UpdateSaleHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating sale {SaleId}", command.Id);

        try
        {
            var sale = await _repository.GetByIdAsync(command.Id, cancellationToken);

            if (sale is null)
            {
                _logger.LogWarning("Sale {SaleId} not found", command.Id);
                throw new KeyNotFoundException($"Sale {command.Id} not found");
            }

            sale.UpdateCustomer(command.CustomerName);
            sale.UpdateBranch(command.BranchName);

            sale.ClearItems();
            foreach (var item in command.Items)
                sale.AddItem(item.ProductId, item.ProductName, item.Quantity, item.UnitPrice);

            await _repository.UpdateAsync(sale, cancellationToken);

            _logger.LogInformation("Event: SaleUpdated | SaleId: {SaleId}", sale.Id);
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot update sale {SaleId}: {Reason}", command.Id, ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating sale {SaleId}", command.Id);
            throw;
        }
    }
}
