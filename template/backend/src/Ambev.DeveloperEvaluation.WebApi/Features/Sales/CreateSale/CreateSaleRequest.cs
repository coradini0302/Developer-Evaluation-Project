namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale
{
    public class CreateSaleRequest
    {
        public string SaleNumber { get; set; }

        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }

        public Guid BranchId { get; set; }
        public string BranchName { get; set; }

        public List<CreateSaleItemRequest> Items { get; set; } = new();
    }

    public class CreateSaleItemRequest
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
