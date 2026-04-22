using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale
{
    public class GetSaleHandler : IRequestHandler<GetSaleRequest, GetSaleResponse>
    {
        private readonly ISaleRepository _repository;
        private readonly ILogger<GetSaleHandler> _logger;

        public GetSaleHandler(ISaleRepository repository, ILogger<GetSaleHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<GetSaleResponse> Handle(GetSaleRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching sale with id {SaleId}", request.Id);

            var sale = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (sale == null)
            {
                _logger.LogWarning("Sale {SaleId} not found", request.Id);
                throw new KeyNotFoundException("Sale not found");
            }

            _logger.LogInformation(
                "Sale retrieved successfully | SaleId: {SaleId} | Customer: {CustomerName} | Total: {TotalAmount}",
                sale.Id,
                sale.CustomerName,
                sale.TotalAmount
            );

            return new GetSaleResponse
            {
                Id = sale.Id,
                SaleNumber = sale.SaleNumber,
                CustomerName = sale.CustomerName,
                BranchName = sale.BranchName,
                TotalAmount = sale.TotalAmount,
                Status = sale.Status.ToString(),
                Items = sale.Items.Select(i => new SaleItemResponse
                {
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    TotalAmount = i.TotalAmount,
                    Discount = i.Discount
                }).ToList()
            };
        }
    }
}