using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

public static class SaleTestData
{
    private static readonly Faker<Sale> SaleFaker = new Faker<Sale>()
        .CustomInstantiator(f => new Sale(
            f.Random.AlphaNumeric(10),
            Guid.NewGuid(),
            f.Person.FullName,
            Guid.NewGuid(),
            f.Company.CompanyName()
        ));

    public static Sale GenerateValidSale()
    {
        return SaleFaker.Generate();
    }

    public static Guid GenerateProductId()
    {
        return Guid.NewGuid();
    }

    public static string GenerateProductName()
    {
        return new Faker().Commerce.ProductName();
    }

    public static decimal GenerateUnitPrice()
    {
        return new Faker().Random.Decimal(1, 100);
    }

    public static int GenerateValidQuantity()
    {
        return new Faker().Random.Int(1, 20);
    }
}