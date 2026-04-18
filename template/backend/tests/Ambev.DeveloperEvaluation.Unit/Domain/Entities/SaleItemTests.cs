using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

/// <summary>
/// Contains unit tests for the SaleItem entity.
/// Tests cover discount rules, quantity limits and total calculation.
/// </summary>
public class SaleItemTests
{
    /// <summary>
    /// Tests that no discount is applied when quantity is less than 4.
    /// </summary>
    [Fact(DisplayName = "Should not apply discount when quantity is less than 4")]
    public void Given_QuantityLessThan4_When_Created_Then_ShouldNotApplyDiscount()
    {
        // Arrange
        var item = SaleItemTestData.GenerateItemWithQuantity(3, 10);

        // Act
        var discount = item.Discount;

        // Assert
        Assert.Equal(0, discount);
        Assert.Equal(30, item.TotalAmount);
    }

    /// <summary>
    /// Tests that 10% discount is applied for quantities between 4 and 9.
    /// </summary>
    [Fact(DisplayName = "Should apply 10% discount when quantity is between 4 and 9")]
    public void Given_QuantityBetween4And9_When_Created_Then_ShouldApply10PercentDiscount()
    {
        // Arrange
        var item = SaleItemTestData.GenerateItemWithQuantity(5, 10);

        // Act
        var discount = item.Discount;

        // Assert
        Assert.Equal(0.1m, discount);
        Assert.Equal(45, item.TotalAmount);
    }

    /// <summary>
    /// Tests that 20% discount is applied for quantities between 10 and 20.
    /// </summary>
    [Fact(DisplayName = "Should apply 20% discount when quantity is between 10 and 20")]
    public void Given_QuantityBetween10And20_When_Created_Then_ShouldApply20PercentDiscount()
    {
        // Arrange
        var item = SaleItemTestData.GenerateItemWithQuantity(10, 10);

        // Act
        var discount = item.Discount;

        // Assert
        Assert.Equal(0.2m, discount);
        Assert.Equal(80, item.TotalAmount);
    }

    /// <summary>
    /// Tests that an exception is thrown when quantity exceeds 20.
    /// </summary>
    [Fact(DisplayName = "Should throw exception when quantity is above 20")]
    public void Given_QuantityAbove20_When_Created_Then_ShouldThrowException()
    {
        // Arrange
        var quantity = SaleItemTestData.GenerateInvalidQuantityAboveLimit();

        // Act
        var act = () => SaleItemTestData.GenerateItemWithQuantity(quantity);

        // Assert
        Assert.Throws<Exception>(act);
    }

    /// <summary>
    /// Tests that an exception is thrown when quantity is zero or negative.
    /// </summary>
    [Fact(DisplayName = "Should throw exception when quantity is zero or negative")]
    public void Given_InvalidQuantity_When_Created_Then_ShouldThrowException()
    {
        // Arrange
        var quantity = SaleItemTestData.GenerateInvalidQuantityBelowZero();

        // Act
        var act = () => SaleItemTestData.GenerateItemWithQuantity(quantity);

        // Assert
        Assert.Throws<Exception>(act);
    }

    /// <summary>
    /// Tests that increasing quantity updates discount and total correctly.
    /// </summary>
    [Fact(DisplayName = "Should increase quantity and recalculate discount and total")]
    public void Given_ExistingItem_When_IncreaseQuantity_Then_ShouldUpdateValues()
    {
        // Arrange
        var item = SaleItemTestData.GenerateItemWithQuantity(3, 10);

        // Act
        item.IncreaseQuantity(2); // total = 5

        // Assert
        Assert.Equal(5, item.Quantity);
        Assert.Equal(0.1m, item.Discount);
        Assert.Equal(45, item.TotalAmount);
    }

    /// <summary>
    /// Tests that increasing quantity beyond 20 throws exception.
    /// </summary>
    [Fact(DisplayName = "Should throw exception when increasing quantity above 20")]
    public void Given_ExistingItem_When_IncreaseQuantityAboveLimit_Then_ShouldThrowException()
    {
        // Arrange
        var item = SaleItemTestData.GenerateItemWithQuantity(18, 10);

        // Act
        var act = () => item.IncreaseQuantity(5);

        // Assert
        Assert.Throws<Exception>(act);
    }
}