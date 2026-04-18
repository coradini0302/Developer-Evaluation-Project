using Ambev.DeveloperEvaluation.Application.Sales;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Moq;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class CreateSaleHandlerTests
{
    [Fact(DisplayName = "Should create a sale successfully")]
    public async Task Given_ValidCommand_When_Handle_Then_ShouldCreateSale()
    {
        // Arrange
        var repositoryMock = new Mock<ISaleRepository>();

        var handler = new CreateSaleHandler(repositoryMock.Object);

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
        repositoryMock.Verify(r => r.AddAsync(It.IsAny<Sale>()), Times.Once);
        Assert.NotEqual(Guid.Empty, result);
    }

    [Fact(DisplayName = "Should create sale with items")]
    public async Task Given_Items_When_Handle_Then_ShouldAddItems()
    {
        var repositoryMock = new Mock<ISaleRepository>();

        var handler = new CreateSaleHandler(repositoryMock.Object);

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

        repositoryMock.Verify(r =>
            r.AddAsync(It.Is<Sale>(s => s.Items.Count == 1)),
            Times.Once);
    }

    [Fact(DisplayName = "Should create sale with multiple items")]
    public async Task Given_MultipleItems_When_Handle_Then_ShouldAddAllItems()
    {
        // Arrange
        var repositoryMock = new Mock<ISaleRepository>();

        var handler = new CreateSaleHandler(repositoryMock.Object);

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
        repositoryMock.Verify(r =>
            r.AddAsync(It.Is<Sale>(s => s.Items.Count == 2)),
            Times.Once);
        }

    [Fact(DisplayName = "Should merge items with same product")]
    public async Task Given_DuplicateProducts_When_Handle_Then_ShouldMergeItems()
    {
        // Arrange
        var repositoryMock = new Mock<ISaleRepository>();

        var handler = new CreateSaleHandler(repositoryMock.Object);

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
        repositoryMock.Verify(r =>
            r.AddAsync(It.Is<Sale>(s =>
                s.Items.Count == 1 &&
                s.Items.First().Quantity == 5)),
            Times.Once);
    }
}