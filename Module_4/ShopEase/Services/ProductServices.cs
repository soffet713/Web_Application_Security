using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShopEase.Models;

namespace ShopEase.Services
{
    public class ProductService
    {
        // A temporary in-memory list replacing the database for frontend testing
        private readonly List<Product> _mockDatabase = new List<Product>();

        public async Task AddProductToDbAsync(Product product)
        {
            await Task.Delay(100); // Simulate network lag
            _mockDatabase.Add(product);
            Console.WriteLine($"[Mock DB] Added product: {product.Name}");
        }

        public async Task RemoveProductFromDbAsync(int productId)
        {
            await Task.Delay(100); // Simulate network lag
            _mockDatabase.RemoveAll(p => p.ProductID == productId);
            Console.WriteLine($"[Mock DB] Removed product ID: {productId}");
        }
    }
}