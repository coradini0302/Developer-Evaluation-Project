using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class CreateSaleHandlerTests
{
    private readonly Mock<ISaleRepository> _repositoryMock = new();
    private readonly Mock<ILogger<CreateSaleHandler>> _loggerMock = new();

    private CreateSaleHandler CreateHandler() =>
        new(_repositoryMock.Object, _loggerMock.Object);

    [Fact(DisplayName = "Should create a sale successfully")]
    public async Task Given_ValidCommand_When_Handle_Then_ShouldCreateSale()
    {
        // Arrange
        var handler = CreateHandler();
        var command = new CreateSaleCommand
        {
            SaleNumber = "123",
            CustomerId = Guid.NewGuid(),
            CustomerName = "Cliente",
            BranchId = Guid.NewGuid(),
            BranchName = "Filial",
            Items = new List<CreateSaleItemCommand>()
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Sale>()), Times.Once);
        Assert.NotEqual(Guid.Empty, result);
    }

    [Fact(DisplayName = "Should create sale with items")]
    public async Task Given_Items_When_Handle_Then_ShouldAddItems()
    {
        var handler = CreateHandler();
        var command = new CreateSaleCommand
        {
            SaleNumber = "123",
            CustomerId = Guid.NewGuid(),
            CustomerName = "Cliente",
            BranchId = Guid.NewGuid(),
            BranchName = "Filial",
            Items = new List<CreateSaleItemCommand>
            {
                new CreateSaleItemCommand
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Produto",
                    Quantity = 2,
                    UnitPrice = 10
                }
            }
        };

        await handler.Handle(command, CancellationToken.None);

        _repositoryMock.Verify(r =>
            r.AddAsync(It.Is<Sale>(s => s.Items.Count == 1)),
            Times.Once);
    }

    [Fact(DisplayName = "Should create sale with multiple items")]
    public async Task Given_MultipleItems_When_Handle_Then_ShouldAddAllItems()
    {
        // Arrange
        var handler = CreateHandler();
        var command = new CreateSaleCommand
        {
            SaleNumber = "123",
            CustomerId = Guid.NewGuid(),
            CustomerName = "Cliente",
            BranchId = Guid.NewGuid(),
            BranchName = "Filial",
            Items = new List<CreateSaleItemCommand>
            {
                new CreateSaleItemCommand
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Produto 1",
                    Quantity = 2,
                    UnitPrice = 10
                },
                new CreateSaleItemCommand
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Produto 2",
                    Quantity = 3,
                    UnitPrice = 20
                }
            }
        };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r =>
            r.AddAsync(It.Is<Sale>(s => s.Items.Count == 2)),
            Times.Once);
    }

    [Fact(DisplayName = "Should merge items with same product")]
    public async Task Given_DuplicateProducts_When_Handle_Then_ShouldMergeItems()
    {
        // Arrange
        var handler = CreateHandler();
        var productId = Guid.NewGuid();
        var command = new CreateSaleCommand
        {
            SaleNumber = "123",
            CustomerId = Guid.NewGuid(),
            CustomerName = "Cliente",
            BranchId = Guid.NewGuid(),
            BranchName = "Filial",
            Items = new List<CreateSaleItemCommand>
            {
                new CreateSaleItemCommand
                {
                    ProductId = productId,
                    ProductName = "Produto",
                    Quantity = 2,
                    UnitPrice = 10
                },
                new CreateSaleItemCommand
                {
                    ProductId = productId,
                    ProductName = "Produto",
                    Quantity = 3,
                    UnitPrice = 10
                }
            }
        };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(r =>
            r.AddAsync(It.Is<Sale>(s =>
                s.Items.Count == 1 &&
                s.Items.First().Quantity == 5)),
            Times.Once);
    }
}
