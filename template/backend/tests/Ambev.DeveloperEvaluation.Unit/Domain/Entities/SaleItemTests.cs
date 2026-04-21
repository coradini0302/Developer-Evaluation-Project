using Ambev.DeveloperEvaluation.Domain.Entities;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

/// <summary>
/// Contains unit tests for the SaleItem entity.
/// Tests cover discount rules, total calculation and validations.
/// </summary>
public class SaleItemTests
{
    /// <summary>
    /// Tests that no discount is applied when quantity is less than 5.
    /// </summary>
    [Fact(DisplayName = "Should not apply discount when quantity is less than 5")]
    public void Should_Not_Apply_Discount_When_Quantity_Is_Less_Than_5()
    {
        var item = new SaleItem(Guid.NewGuid(), "Produto", 4, 100);

        Assert.Equal(0, item.Discount);
        Assert.Equal(400, item.TotalAmount);
    }

    /// <summary>
    /// Tests that a 10% discount is applied when quantity is between 5 and 9.
    /// </summary>
    [Fact(DisplayName = "Should apply 10% discount when quantity is between 5 and 9")]
    public void Should_Apply_10_Percent_Discount_When_Quantity_Is_5()
    {
        var item = new SaleItem(Guid.NewGuid(), "Produto", 5, 100);

        Assert.Equal(50, item.Discount);
        Assert.Equal(450, item.TotalAmount);
    }

    /// <summary>
    /// Tests that a 20% discount is applied when quantity is 10 or more.
    /// </summary>
    [Fact(DisplayName = "Should apply 20% discount when quantity is 10 or more")]
    public void Should_Apply_20_Percent_Discount_When_Quantity_Is_10()
    {
        var item = new SaleItem(Guid.NewGuid(), "Produto", 10, 100);

        Assert.Equal(200, item.Discount);
        Assert.Equal(800, item.TotalAmount);
    }

    /// <summary>
    /// Tests that increasing the quantity recalculates discount and total correctly.
    /// </summary>
    [Fact(DisplayName = "Should recalculate discount when quantity increases")]
    public void Should_Recalculate_Discount_When_Quantity_Increases()
    {
        var item = new SaleItem(Guid.NewGuid(), "Produto", 4, 100);

        item.IncreaseQuantity(1); // total = 5

        Assert.Equal(50, item.Discount);
        Assert.Equal(450, item.TotalAmount);
    }

    /// <summary>
    /// Tests that increasing quantity accumulates correctly and recalculates values.
    /// </summary>
    [Fact(DisplayName = "Should accumulate quantity and recalculate values correctly")]
    public void Should_Accumulate_Quantity_And_Recalculate_Correctly()
    {
        var item = new SaleItem(Guid.NewGuid(), "Produto", 2, 100);

        item.IncreaseQuantity(3); // total = 5

        Assert.Equal(5, item.Quantity);
        Assert.Equal(50, item.Discount);
        Assert.Equal(450, item.TotalAmount);
    }

    /// <summary>
    /// Tests that creating an item with zero quantity throws an exception.
    /// </summary>
    [Fact(DisplayName = "Should throw exception when quantity is zero")]
    public void Should_Throw_Exception_When_Quantity_Is_Zero()
    {
        Assert.Throws<ArgumentException>(() =>
            new SaleItem(Guid.NewGuid(), "Produto", 0, 100));
    }

    /// <summary>
    /// Tests that creating an item with negative quantity throws an exception.
    /// </summary>
    [Fact(DisplayName = "Should throw exception when quantity is negative")]
    public void Should_Throw_Exception_When_Quantity_Is_Negative()
    {
        Assert.Throws<ArgumentException>(() =>
            new SaleItem(Guid.NewGuid(), "Produto", -1, 100));
    }
}