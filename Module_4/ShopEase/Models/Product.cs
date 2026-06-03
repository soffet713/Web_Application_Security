using System;
using System.ComponentModel.DataAnnotations;

namespace ShopEase.Models
{
    public class Product
    {
        // Properties
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\s\-\.]+$", ErrorMessage = "Special characters are not allowed to prevent XSS.")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 10000.00, ErrorMessage = "Price must be between $0.01 and $10,000.00.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        [StringLength(30, ErrorMessage = "Category cannot exceed 30 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Category can only contain letters.")]
        public string Category { get; set; } = string.Empty;

        public Product()
        {
        }

        // Constructor for easy initialization
        public Product(int id, string name, decimal price, string category)
        {
            ProductID = id;
            Name = name;
            Price = price;
            Category = category;
        }

        // Method to display product details
        public void DisplayDetails()
        {
            Console.WriteLine($"Product: {Name} | Price: ${Price:F2} | Category: {Category}");
        }
    }
}