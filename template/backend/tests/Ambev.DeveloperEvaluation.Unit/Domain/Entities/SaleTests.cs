using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

/// <summary>
/// Contains unit tests for the Sale entity.
/// Tests cover item addition, merging, cancellation and total calculation.
/// </summary>
public class SaleTests
{
    /// <summary>
    /// Tests that a new item is added to the sale.
    /// </summary>
    [Fact(DisplayName = "Should add a new item to sale")]
    public void Given_ValidItem_When_AddItem_Then_ItemShouldBeAdded()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        // Act
        sale.AddItem(
            SaleTestData.GenerateProductId(),
            SaleTestData.GenerateProductName(),
            2,
            10
        );

        // Assert
        Assert.Single(sale.Items);
    }

    /// <summary>
    /// Tests that items with the same product are merged.
    /// </summary>
    [Fact(DisplayName = "Should merge items with same product")]
    public void Given_SameProduct_When_AddItem_Then_ShouldMergeQuantities()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var productId = SaleTestData.GenerateProductId();

        // Act
        sale.AddItem(productId, "Produto", 2, 10);
        sale.AddItem(productId, "Produto", 3, 10);

        // Assert
        var item = sale.Items.First();

        Assert.Single(sale.Items);
        Assert.Equal(5, item.Quantity);
    }

    /// <summary>
    /// Tests that adding items to a cancelled sale throws an exception.
    /// </summary>
    [Fact(DisplayName = "Should not allow adding item to cancelled sale")]
    public void Given_CancelledSale_When_AddItem_Then_ShouldThrowException()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.Cancel();

        // Act
        var act = () => sale.AddItem(
            SaleTestData.GenerateProductId(),
            "Produto",
            1,
            10
        );

        // Assert
        Assert.Throws<Exception>(act);
    }

    /// <summary>
    /// Tests that total amount is calculated correctly.
    /// </summary>
    [Fact(DisplayName = "Should calculate total amount correctly")]
    public void Given_Items_When_Added_Then_TotalAmountShouldBeCorrect()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        // Act
        sale.AddItem(Guid.NewGuid(), "Produto A", 2, 10); // 20
        sale.AddItem(Guid.NewGuid(), "Produto B", 4, 10); // 40 - 10% = 36

        // Assert
        Assert.Equal(56, sale.TotalAmount);
    }

    /// <summary>
    /// Tests that cancelling a sale updates its status.
    /// </summary>
    [Fact(DisplayName = "Should mark sale as cancelled")]
    public void Given_Sale_When_Cancelled_Then_ShouldBeMarkedAsCancelled()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        // Act
        sale.Cancel();

        // Assert
        Assert.True(sale.IsCancelled);
    }
}