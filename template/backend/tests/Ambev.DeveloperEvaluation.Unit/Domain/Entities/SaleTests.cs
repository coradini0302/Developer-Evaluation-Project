using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities
{
    /// <summary>
    /// Contains unit tests for the Sale entity.
    /// Tests cover total calculation, cancellation rules and item behavior.
    /// </summary>
    public class SaleTests
    {
        /// <summary>
        /// Tests that the total amount of a sale is correctly calculated
        /// based on its items, including applied discounts.
        /// </summary>
        [Fact(DisplayName = "Should calculate total amount using items")]
        public void Should_Calculate_Total_Amount_Using_Items()
        {
            // Arrange
            var sale = new Sale("SALE-1", Guid.NewGuid(), "Cliente", Guid.NewGuid(), "Filial");

            // Act
            sale.AddItem(Guid.NewGuid(), "Produto A", 5, 100); // 500 → 10% desconto → 450
            sale.AddItem(Guid.NewGuid(), "Produto B", 2, 50);  // 100 → sem desconto

            // Assert
            Assert.Equal(550, sale.TotalAmount);
        }

        /// <summary>
        /// Tests that adding items to a cancelled sale is not allowed.
        /// Ensures domain integrity by preventing invalid operations.
        /// </summary>
        [Fact(DisplayName = "Should not allow adding items when sale is cancelled")]
        public void Should_Not_Allow_Add_Item_When_Sale_Is_Cancelled()
        {
            // Arrange
            var sale = new Sale("SALE-1", Guid.NewGuid(), "Cliente", Guid.NewGuid(), "Filial");

            // Act
            sale.Cancel();

            // Assert
            Assert.Throws<Exception>(() =>
                sale.AddItem(Guid.NewGuid(), "Produto", 1, 100));
        }

        /// <summary>
        /// Tests that calling Cancel correctly updates the sale status.
        /// </summary>
        [Fact(DisplayName = "Should mark sale as cancelled")]
        public void Should_Mark_Sale_As_Cancelled()
        {
            // Arrange
            var sale = new Sale("SALE-1", Guid.NewGuid(), "Cliente", Guid.NewGuid(), "Filial");

            // Act
            sale.Cancel();

            // Assert
            Assert.Equal(SaleStatus.Cancelled, sale.Status);
        }

        /// <summary>
        /// Tests that cancelling an already cancelled sale throws an exception.
        /// Prevents invalid state transitions.
        /// </summary>
        [Fact(DisplayName = "Should not cancel sale twice")]
        public void Should_Not_Cancel_Sale_Twice()
        {
            // Arrange
            var sale = new Sale("SALE-1", Guid.NewGuid(), "Cliente", Guid.NewGuid(), "Filial");

            sale.Cancel();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => sale.Cancel());
        }

        /// <summary>
        /// Tests that adding the same product multiple times merges the items
        /// and recalculates the total amount correctly, including discounts.
        /// </summary>
        [Fact(DisplayName = "Should merge items and recalculate total when same product is added")]
        public void Should_Recalculate_Total_When_Same_Product_Is_Added()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var sale = new Sale("SALE-1", Guid.NewGuid(), "Cliente", Guid.NewGuid(), "Filial");

            // Act
            sale.AddItem(productId, "Produto", 2, 100); // 200
            sale.AddItem(productId, "Produto", 3, 100); // total = 5 → 10% desconto → 450

            // Assert
            Assert.Single(sale.Items);
            Assert.Equal(450, sale.TotalAmount);
        }
    }
}