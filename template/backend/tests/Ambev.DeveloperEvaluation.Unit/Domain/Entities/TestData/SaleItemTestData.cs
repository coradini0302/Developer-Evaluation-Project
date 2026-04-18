using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

public static class SaleItemTestData
{
    private static readonly Faker<SaleItem> SaleItemFaker = new Faker<SaleItem>()
        .CustomInstantiator(f => new SaleItem(
            Guid.NewGuid(),
            f.Commerce.ProductName(),
            f.Random.Int(1, 20),
            f.Random.Decimal(1, 100)
        ));

    public static SaleItem GenerateValidItem()
    {
        return SaleItemFaker.Generate();
    }

    public static SaleItem GenerateItemWithQuantity(int quantity, decimal unitPrice = 10)
    {
        return new SaleItem(
            Guid.NewGuid(),
            new Faker().Commerce.ProductName(),
            quantity,
            unitPrice
        );
    }

    public static int GenerateInvalidQuantityAboveLimit()
    {
        return new Faker().Random.Int(21, 50);
    }

    public static int GenerateInvalidQuantityBelowZero()
    {
        return new Faker().Random.Int(-10, -1);
    }
}