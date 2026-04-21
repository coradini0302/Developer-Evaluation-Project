using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale
{
    public class GetSaleHandler : IRequestHandler<GetSaleRequest, GetSaleResponse>
    {
        private readonly ISaleRepository _repository;

        public GetSaleHandler(ISaleRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetSaleResponse> Handle(GetSaleRequest request, CancellationToken cancellationToken)
        {
            var sale = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (sale == null)
                throw new KeyNotFoundException("Sale not found");

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
