namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public class GetSaleResponse
{
    public Guid Id { get; set; }
    public string SaleNumber { get; set; }
    public string CustomerName { get; set; }
    public string BranchName { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; }
    public List<SaleItemResponse> Items { get; set; } = [];
}

public class SaleItemResponse
{
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal Discount { get; set; }
}