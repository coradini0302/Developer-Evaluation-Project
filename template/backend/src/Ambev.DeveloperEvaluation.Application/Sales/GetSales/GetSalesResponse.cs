namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales;

public class GetSalesResponse
{
    public List<SaleSummary> Items { get; set; } = [];
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public bool HasPrevious { get; set; }
    public bool HasNext { get; set; }
}

public class SaleSummary
{
    public Guid Id { get; set; }
    public string SaleNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
}
