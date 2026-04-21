public class SaleItem
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    public decimal Discount { get; private set; }
    public decimal TotalAmount { get; private set; }

    public bool IsCancelled { get; private set; }

    public SaleItem(Guid productId, string productName, int quantity, decimal unitPrice)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        ProductName = productName;

        SetQuantity(quantity);
        UnitPrice = unitPrice;

        Recalculate();
    }

    public void IncreaseQuantity(int quantity)
    {
        Quantity += quantity;
        Recalculate();
    }

    private void SetQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero");

        Quantity = quantity;
    }

    private void Recalculate()
    {
        var total = Quantity * UnitPrice;

        Discount = CalculateDiscount(total, Quantity);

        TotalAmount = total - Discount;
    }

    private decimal CalculateDiscount(decimal total, int quantity)
    {
        if (quantity >= 10)
            return total * 0.2m;

        if (quantity >= 5)
            return total * 0.1m;

        return 0;
    }
}