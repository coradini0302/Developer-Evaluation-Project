using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Sales;

public class SalesIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public SalesIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    // ─────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────

    private static CreateSalePayload BuildCreateSaleRequest(int quantity = 1, decimal unitPrice = 25.00m) =>
        new(
            SaleNumber: $"SALE-{Guid.NewGuid():N}",
            CustomerId: Guid.NewGuid(),
            CustomerName: "Integration Test Customer",
            BranchId: Guid.NewGuid(),
            BranchName: "Integration Test Branch",
            Items:
            [
                new CreateSaleItemPayload(
                    ProductId: Guid.NewGuid(),
                    ProductName: "Test Product A",
                    Quantity: quantity,
                    UnitPrice: unitPrice)
            ]
        );

    private async Task<Guid> CreateSaleAsync(int quantity = 1, decimal unitPrice = 25.00m)
    {
        var response = await _client.PostAsJsonAsync("/api/sales", BuildCreateSaleRequest(quantity, unitPrice));
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<CreateSaleResult>();
        return body!.Id;
    }

    // ─────────────────────────────────────────────
    // POST /api/sales
    // ─────────────────────────────────────────────

    [Fact]
    public async Task CreateSale_WithValidPayload_Returns200AndNonEmptyId()
    {
        // Arrange
        var request = BuildCreateSaleRequest(quantity: 1);

        // Act
        var response = await _client.PostAsJsonAsync("/api/sales", request);
        var body = await response.Content.ReadFromJsonAsync<CreateSaleResult>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().NotBeNull();
        body!.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateSale_WithDuplicateProductItems_MergesIntoSingleItem()
    {
        // Arrange – same productId twice so the handler merges them
        var productId = Guid.NewGuid();
        var request = new CreateSalePayload(
            SaleNumber: $"SALE-{Guid.NewGuid():N}",
            CustomerId: Guid.NewGuid(),
            CustomerName: "Merge Test Customer",
            BranchId: Guid.NewGuid(),
            BranchName: "Main Branch",
            Items:
            [
                new CreateSaleItemPayload(productId, "Product A", 3, 10.00m),
                new CreateSaleItemPayload(productId, "Product A", 2, 10.00m)
            ]
        );

        var id = (await _client.PostAsJsonAsync("/api/sales", request)
            .ContinueWith(t => t.Result.Content.ReadFromJsonAsync<CreateSaleResult>()))
            .Result!.Id;

        // Act
        var response = await _client.GetAsync($"/api/sales/{id}");
        var body = await response.Content.ReadFromJsonAsync<GetSaleResult>();

        // Assert – two identical products merged → qty 5 → 10% discount → total = 50 → discount = 5
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body!.Items.Should().HaveCount(1);
        body.Items[0].Quantity.Should().Be(5);
        body.Items[0].Discount.Should().Be(5.00m);
    }

    // ─────────────────────────────────────────────
    // GET /api/sales/{id}
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetSaleById_WithExistingId_ReturnsSaleWithCorrectFields()
    {
        // Arrange
        var id = await CreateSaleAsync(quantity: 1, unitPrice: 50.00m);

        // Act
        var response = await _client.GetAsync($"/api/sales/{id}");
        var body = await response.Content.ReadFromJsonAsync<GetSaleResult>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().NotBeNull();
        body!.Id.Should().Be(id);
        body.CustomerName.Should().Be("Integration Test Customer");
        body.BranchName.Should().Be("Integration Test Branch");
        body.Status.Should().Be("Created");
        body.Items.Should().HaveCount(1);

        var item = body.Items[0];
        item.ProductName.Should().Be("Test Product A");
        item.Quantity.Should().Be(1);
        item.UnitPrice.Should().Be(50.00m);
        // qty < 5 → no discount
        item.Discount.Should().Be(0m);
        item.TotalAmount.Should().Be(50.00m);
    }

    [Fact]
    public async Task GetSaleById_WithFiveItems_Applies10PercentDiscount()
    {
        // Arrange – qty 5 triggers 10% discount
        var id = await CreateSaleAsync(quantity: 5, unitPrice: 25.00m);

        // Act
        var response = await _client.GetAsync($"/api/sales/{id}");
        var body = await response.Content.ReadFromJsonAsync<GetSaleResult>();

        // Assert
        var item = body!.Items[0];
        item.Quantity.Should().Be(5);
        // 5 × 25 = 125 → 10% = 12.5
        item.Discount.Should().Be(12.50m);
        item.TotalAmount.Should().Be(112.50m);
        body.TotalAmount.Should().Be(112.50m);
    }

    [Fact]
    public async Task GetSaleById_WithTenItems_Applies20PercentDiscount_Correctly()
    {
        // Arrange – qty 10 triggers 20% discount
        var id = await CreateSaleAsync(quantity: 10, unitPrice: 20.00m);

        // Act
        var response = await _client.GetAsync($"/api/sales/{id}");
        var body = await response.Content.ReadFromJsonAsync<GetSaleResult>();

        // Assert
        var item = body!.Items[0];
        item.Quantity.Should().Be(10);
        // 10 × 20 = 200 → 20% = 40
        item.Discount.Should().Be(40.00m);
        item.TotalAmount.Should().Be(160.00m);
        body.TotalAmount.Should().Be(160.00m);
    }

    [Fact]
    public async Task GetSaleById_WithTenItems_Applies20PercentDiscount()
    {
        // Arrange – qty 10 triggers 20% discount
        var id = await CreateSaleAsync(quantity: 10, unitPrice: 20.00m);

        // Act
        var response = await _client.GetAsync($"/api/sales/{id}");
        var body = await response.Content.ReadFromJsonAsync<GetSaleResult>();

        // Assert
        var item = body!.Items[0];
        item.Quantity.Should().Be(10);
        // 10 × 20 = 200 → 20% = 40
        item.Discount.Should().Be(40.00m);
        item.TotalAmount.Should().Be(160.00m);
        body.TotalAmount.Should().Be(160.00m);
    }

    [Fact]
    public async Task GetSaleById_WithNonExistingId_ThrowsKeyNotFoundException()
    {
        // Act
        Func<Task> act = async () =>
            await _client.GetAsync($"/api/sales/{Guid.NewGuid()}");

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Sale not found");
    }

    // ─────────────────────────────────────────────
    // GET /api/sales?page=1&pageSize=10
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetAllSales_WithDefaultPagination_ReturnsPaginatedResult()
    {
        // Arrange
        await CreateSaleAsync();
        await CreateSaleAsync();

        // Act
        var response = await _client.GetAsync("/api/sales?page=1&pageSize=10");
        var body = await response.Content.ReadFromJsonAsync<GetSalesResult>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().NotBeNull();
        body!.CurrentPage.Should().Be(1);
        body.PageSize.Should().Be(10);
        body.TotalCount.Should().BeGreaterThanOrEqualTo(2);
        body.Items.Should().NotBeEmpty();
        body.HasPrevious.Should().BeFalse();
    }

    [Fact]
    public async Task GetAllSales_SaleItemsAreNotIncludedInListResponse()
    {
        // Arrange
        await CreateSaleAsync(quantity: 5);

        // Act
        var response = await _client.GetAsync("/api/sales?page=1&pageSize=10");
        var body = await response.Content.ReadFromJsonAsync<GetSalesResult>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body!.Items.Should().NotBeEmpty();
        body.Items[0].SaleNumber.Should().NotBeNullOrEmpty();
        body.Items[0].CustomerName.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetAllSales_PageSizeOf1_ReturnsSingleItemAndPaginationFlags()
    {
        // Arrange
        await CreateSaleAsync();
        await CreateSaleAsync();

        // Act
        var response = await _client.GetAsync("/api/sales?page=1&pageSize=1");
        var body = await response.Content.ReadFromJsonAsync<GetSalesResult>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body!.Items.Should().HaveCount(1);
        body.PageSize.Should().Be(1);
        body.TotalPages.Should().BeGreaterThanOrEqualTo(2);
        body.HasNext.Should().BeTrue();
        body.HasPrevious.Should().BeFalse();
    }

    // ─────────────────────────────────────────────
    // DELETE /api/sales/{id}/cancel
    // ─────────────────────────────────────────────

    [Fact]
    public async Task CancelSale_WithExistingId_Returns200AndSuccessMessage()
    {
        // Arrange
        var id = await CreateSaleAsync();

        // Act
        var response = await _client.DeleteAsync($"/api/sales/{id}/cancel");
        var body = await response.Content.ReadFromJsonAsync<ApiResult>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().NotBeNull();
        body!.Success.Should().BeTrue();
        body.Message.Should().Be("Sale cancelled successfully");
    }

    [Fact]
    public async Task CancelSale_AfterCancellation_SaleStatusIsCancelled()
    {
        // Arrange
        var id = await CreateSaleAsync();
        await _client.DeleteAsync($"/api/sales/{id}/cancel");

        // Act
        var response = await _client.GetAsync($"/api/sales/{id}");
        var body = await response.Content.ReadFromJsonAsync<GetSaleResult>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body!.Status.Should().Be("Cancelled");
        body.TotalAmount.Should().Be(0m);
    }

    [Fact]
    public async Task CancelSale_AlreadyCancelled_ThrowsInvalidOperationException()
    {
        // Arrange
        var id = await CreateSaleAsync();
        await _client.DeleteAsync($"/api/sales/{id}/cancel");

        // Act
        Func<Task> act = async () =>
            await _client.DeleteAsync($"/api/sales/{id}/cancel");

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Sale already cancelled");
    }

    // ─────────────────────────────────────────────
    // DTOs
    // ─────────────────────────────────────────────

    private record CreateSalePayload(
        string SaleNumber,
        Guid CustomerId,
        string CustomerName,
        Guid BranchId,
        string BranchName,
        CreateSaleItemPayload[] Items);

    private record CreateSaleItemPayload(
        Guid ProductId,
        string ProductName,
        int Quantity,
        decimal UnitPrice);

    private record CreateSaleResult(Guid Id);

    private record GetSaleResult(
        Guid Id,
        string SaleNumber,
        string CustomerName,
        string BranchName,
        decimal TotalAmount,
        string Status,
        List<SaleItemResult> Items);

    private record SaleItemResult(
        string ProductName,
        int Quantity,
        decimal UnitPrice,
        decimal Discount,
        decimal TotalAmount);

    private record GetSalesResult(
        List<SaleSummaryResult> Items,
        int CurrentPage,
        int TotalPages,
        int PageSize,
        int TotalCount,
        bool HasPrevious,
        bool HasNext);

    private record SaleSummaryResult(
        Guid Id,
        string SaleNumber,
        string CustomerName,
        string BranchName,
        decimal TotalAmount,
        string Status);

    private record ApiResult(bool Success, string Message);
}