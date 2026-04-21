using Ambev.DeveloperEvaluation.Common.Pagination;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories
{
    public class SaleRepository : ISaleRepository
    {
        private readonly DefaultContext _context;

        public SaleRepository(DefaultContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Sale sale)
        {
            await _context.Sales.AddAsync(sale);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Sale sale, CancellationToken cancellationToken)
        {
            _context.Sales.Update(sale);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Sales
                .Include(s => s.Items) 
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<PaginatedList<Sale>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            var query = _context.Sales
                .Include(s => s.Items) 
                .AsNoTracking();

            var count = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(s => s.Date) // ✔️ boa prática (ordenar)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PaginatedList<Sale>(items, count, page, pageSize);
        }
    }
}