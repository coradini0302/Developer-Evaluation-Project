using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleCommand : IRequest<Guid>
{
    public string SaleNumber { get; set; }

    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; }

    public Guid BranchId { get; set; }
    public string BranchName { get; set; }

    public List<CreateSaleItemCommand> Items { get; set; } = new();
}

public class CreateSaleItemCommand
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}