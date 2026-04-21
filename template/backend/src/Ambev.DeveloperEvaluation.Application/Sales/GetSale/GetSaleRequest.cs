using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public class GetSaleRequest : IRequest<GetSaleResponse>
{
    public Guid Id { get; set; }
}