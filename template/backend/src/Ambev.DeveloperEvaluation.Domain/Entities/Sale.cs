using Ambev.DeveloperEvaluation.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{
    public class Sale
    {
        public Guid Id { get; private set; }
        public string SaleNumber { get; private set; }
        public DateTime Date { get; private set; }

        public Guid CustomerId { get; private set; }
        public string CustomerName { get; private set; }

        public Guid BranchId { get; private set; }
        public string BranchName { get; private set; }

        public SaleStatus Status { get; private set; }

        private readonly List<SaleItem> _items = new();
        public IReadOnlyCollection<SaleItem> Items => _items;

        public decimal TotalAmount
        {
            get
            {
                if (Status == SaleStatus.Cancelled)
                    return 0;

                return _items
                    .Where(x => !x.IsCancelled)
                    .Sum(x => x.TotalAmount);
            }
        }

        public Sale(
            string saleNumber,
            Guid customerId,
            string customerName,
            Guid branchId,
            string branchName)
        {
            Id = Guid.NewGuid();
            SaleNumber = saleNumber;
            Date = DateTime.UtcNow;

            CustomerId = customerId;
            CustomerName = customerName;

            BranchId = branchId;
            BranchName = branchName;

            Status = SaleStatus.Created;
        }

        public void AddItem(Guid productId, string productName, int quantity, decimal unitPrice)
        {
            if (Status == SaleStatus.Cancelled)
                throw new Exception("Cannot add items to a cancelled sale");

            var existingItem = _items
                .FirstOrDefault(x => x.ProductId == productId && !x.IsCancelled);

            if (existingItem != null)
            {
                existingItem.IncreaseQuantity(quantity);
                return;
            }

            var item = new SaleItem(productId, productName, quantity, unitPrice);
            _items.Add(item);
        }

        public void Cancel()
        {
            if (Status == SaleStatus.Cancelled)
                throw new InvalidOperationException("Sale already cancelled");

            Status = SaleStatus.Cancelled;
        }
    }
}
