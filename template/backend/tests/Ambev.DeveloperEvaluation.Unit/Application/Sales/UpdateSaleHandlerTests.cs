using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class UpdateSaleHandlerTests
{
    private readonly Mock<ISaleRepository> _repositoryMock = new();
    private readonly Mock<ILogger<UpdateSaleHandler>> _loggerMock = new();

    private UpdateSaleHandler CreateHandler() =>
        new(_repositoryMock.Object, _loggerMock.Object);

    private static Sale CreateActiveSale()
    {
        var sale = new Sale("SALE-001", Guid.NewGuid(), "Original Customer", Guid.NewGuid(), "Original Branch");
        sale.AddItem(Guid.NewGuid(), "Product A", 2, 10.00m);
        return sale;
    }

    [Fact(DisplayName = "Should update sale successfully when sale exists and is active")]
    public async Task Given_ValidCommand_When_Handle_Then_ShouldUpdateSale()
    {
        // Arrange
        var sale = CreateActiveSale();
        _repositoryMock
            .Setup(r => r.GetByIdAsync(sale.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);

        var command = new UpdateSaleCommand
        {
            Id = sale.Id,
            CustomerName = "Updated Customer",
            BranchName = "Updated Branch",
            Items =
            [
                new UpdateSaleItemCommand
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Product B",
                    Quantity = 5,
                    UnitPrice = 20.00m
                }
            ]
        };

        // Act
        var handler = CreateHandler();
        await handler.Handle(command, CancellationToken.None);

        // Assert
        sale.CustomerName.Should().Be("Updated Customer");
        sale.BranchName.Should().Be("Updated Branch");
        sale.Items.Should().HaveCount(1);
        sale.Items.First().ProductName.Should().Be("Product B");
        sale.Items.First().Quantity.Should().Be(5);

        _repositoryMock.Verify(r =>
            r.UpdateAsync(sale, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Should throw KeyNotFoundException when sale does not exist")]
    public async Task Given_NonExistentId_When_Handle_Then_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Sale?)null);

        var command = new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            CustomerName = "Any",
            BranchName = "Any",
            Items = []
        };

        // Act
        var handler = CreateHandler();
        Func<Task> act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
        _repositoryMock.Verify(r =>
            r.UpdateAsync(It.IsAny<Sale>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Should throw InvalidOperationException when sale is cancelled")]
    public async Task Given_CancelledSale_When_Handle_Then_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var sale = CreateActiveSale();
        sale.Cancel();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(sale.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);

        var command = new UpdateSaleCommand
        {
            Id = sale.Id,
            CustomerName = "New Customer",
            BranchName = "New Branch",
            Items = []
        };

        // Act
        var handler = CreateHandler();
        Func<Task> act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Cannot update a cancelled sale");
        _repositoryMock.Verify(r =>
            r.UpdateAsync(It.IsAny<Sale>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Should recalculate totals and discounts after item replacement")]
    public async Task Given_NewItems_When_Handle_Then_ShouldRecalculateTotals()
    {
        // Arrange
        var sale = CreateActiveSale();
        _repositoryMock
            .Setup(r => r.GetByIdAsync(sale.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sale);

        var command = new UpdateSaleCommand
        {
            Id = sale.Id,
            CustomerName = sale.CustomerName,
            BranchName = sale.BranchName,
            Items =
            [
                new UpdateSaleItemCommand
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Discounted Product",
                    Quantity = 10,
                    UnitPrice = 50.00m
                }
            ]
        };

        // Act
        await CreateHandler().Handle(command, CancellationToken.None);

        // Assert — qty 10 × $50 = $500, discount rate = 20%, total = $400
        sale.Items.Should().HaveCount(1);
        sale.Items.First().Discount.Should().Be(0.2m);
        sale.TotalAmount.Should().Be(400.00m);
    }
}
