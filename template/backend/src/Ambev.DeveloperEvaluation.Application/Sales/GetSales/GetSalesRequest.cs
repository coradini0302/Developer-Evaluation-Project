using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales;

public class GetSalesRequest : IRequest<GetSalesResponse>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
