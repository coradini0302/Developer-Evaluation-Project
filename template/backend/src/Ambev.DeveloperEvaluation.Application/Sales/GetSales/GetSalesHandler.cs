using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales;

public class GetSalesHandler : IRequestHandler<GetSalesRequest, GetSalesResponse>
{
    private readonly ISaleRepository _repository;

    public GetSalesHandler(ISaleRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetSalesResponse> Handle(GetSalesRequest request, CancellationToken cancellationToken)
    {
        var paged = await _repository.GetAllAsync(request.Page, request.PageSize, cancellationToken);

        return new GetSalesResponse
        {
            Items = paged.Select(s => new SaleSummary
            {
                Id = s.Id,
                SaleNumber = s.SaleNumber,
                CustomerName = s.CustomerName,
                BranchName = s.BranchName,
                TotalAmount = s.TotalAmount,
                Status = s.Status.ToString()
            }).ToList(),
            CurrentPage = paged.CurrentPage,
            TotalPages = paged.TotalPages,
            PageSize = paged.PageSize,
            TotalCount = paged.TotalCount,
            HasPrevious = paged.HasPrevious,
            HasNext = paged.HasNext
        };
    }
}
