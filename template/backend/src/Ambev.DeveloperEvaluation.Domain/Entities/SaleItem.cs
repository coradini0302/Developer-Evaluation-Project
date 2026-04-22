public class SaleItem
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    /// <summary>Discount rate: 0, 0.1 or 0.2.</summary>
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
        var newQty = Quantity + quantity;
        if (newQty > 20)
            throw new InvalidOperationException($"Cannot exceed 20 units per item (requested: {newQty})");

        Quantity = newQty;
        Recalculate();
    }

    private void SetQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero");

        if (quantity > 20)
            throw new InvalidOperationException($"Cannot exceed 20 units per item (requested: {quantity})");

        Quantity = quantity;
    }

    private void Recalculate()
    {
        var total = Quantity * UnitPrice;
        Discount = CalculateDiscountRate(Quantity);
        TotalAmount = total - total * Discount;
    }

    private static decimal CalculateDiscountRate(int quantity)
    {
        if (quantity >= 10) return 0.2m;
        if (quantity >= 4)  return 0.1m;
        return 0m;
    }
}
