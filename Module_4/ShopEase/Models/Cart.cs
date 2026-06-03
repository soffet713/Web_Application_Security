using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShopEase.Services;

namespace ShopEase.Models
{
    public class Cart
    {
        private readonly List<Product> _items = new List<Product>();
        private readonly ProductService _dbService = new ProductService();

        // 1. ADD THIS PROPERTY: Allows Index.razor to read the items to count or serialize them
        public IReadOnlyList<Product> Items => _items;

        // Made Async: Updates the list, then awaits the database save operation
        public async Task AddProductAsync(Product product)
        {
            _items.Add(product);
            await _dbService.AddProductToDbAsync(product);
        }

        // Made Async: Updates the list, then awaits the database removal
        public async Task RemoveProductAsync(int productId)
        {
            var itemToRemove = _items.FirstOrDefault(p => p.ProductID == productId);
            if (itemToRemove != null)
            {
                _items.Remove(itemToRemove);
            }
            await _dbService.RemoveProductFromDbAsync(productId);
        }

        // 2. ADD THIS METHOD: Reloads saved storage items back into your readonly list
        // Updated to handle clearing the cart when null or empty data is passed
        public void LoadStoredItems(List<Product>? savedItems)
        {
            _items.Clear(); // Always clear out old in-memory items first!
            
            if (savedItems != null)
            {
                _items.AddRange(savedItems); // Repopulate only if we actually have saved items
            }
        }

        public void DisplayCartItems()
        {
            Console.WriteLine("\n--- Current Cart Items ---");
            foreach (var item in _items)
            {
                item.DisplayDetails();
            }
        }

        public decimal CalculateTotal()
        {
            return _items.Sum(p => p.Price);
        }
    }
}