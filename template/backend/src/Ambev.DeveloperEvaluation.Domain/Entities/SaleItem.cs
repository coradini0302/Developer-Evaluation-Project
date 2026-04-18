using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{
    public class SaleItem
    {
        public Guid Id { get; private set; }

        public Guid ProductId { get; private set; }
        public string ProductName { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }

        public decimal Discount { get; private set; } // %
        public decimal TotalAmount { get; private set; }

        public bool IsCancelled { get; private set; }

        public SaleItem(Guid productId, string productName, int quantity, decimal unitPrice)
        {
            ProductId = productId;
            ProductName = productName;
            UnitPrice = unitPrice;

            SetQuantity(quantity);
        }

        private void SetQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new Exception("Quantity must be greater than zero");

            if (quantity > 20)
                throw new Exception("Cannot sell more than 20 identical items");

            Quantity = quantity;

            ApplyDiscount();
            CalculateTotal();
        }

        private void ApplyDiscount()
        {
            if (Quantity >= 10)
                Discount = 0.2m;
            else if (Quantity >= 4)
                Discount = 0.1m;
            else
                Discount = 0;
        }

        private void CalculateTotal()
        {
            var total = Quantity * UnitPrice;
            TotalAmount = total - (total * Discount);
        }

        public void IncreaseQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new Exception("Quantity must be greater than zero");

            var newQuantity = Quantity + quantity;

            if (newQuantity > 20)
                throw new Exception("Cannot sell more than 20 identical items");

            Quantity = newQuantity;

            ApplyDiscount();
            CalculateTotal();
        }
    }
}
