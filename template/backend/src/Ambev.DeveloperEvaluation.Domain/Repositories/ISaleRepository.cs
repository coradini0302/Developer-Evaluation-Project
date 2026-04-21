using Ambev.DeveloperEvaluation.Common.Pagination;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

public interface ISaleRepository
{
    Task AddAsync(Sale sale);
    Task UpdateAsync(Sale sale, CancellationToken cancellationToken);
    Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PaginatedList<Sale>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken);
}