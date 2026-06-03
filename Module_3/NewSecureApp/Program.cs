using System;
using Microsoft.AspNetCore.Identity;

public class User { public string UserName { get; set; } }

public class Program
{
    // In a real app, this hash would come from a Database
    // This is a hash for "SecretPassword123!" generated via PasswordHasher
    private static string _storedHash = "AQAAAAEAACcQAAAAEM9...examplehash...";

    public static void Main(string[] args)
    {
        Console.WriteLine("Welcome to the SecureApp Login.");
        
        Console.Write("Enter username: ");
        string username = Console.ReadLine();
        
        Console.Write("Enter password: ");
        string password = Console.ReadLine();

        // A) Basic Input Validation
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("Invalid input. Username and password are required.");
            return;
        }

        // B) Calling the Secure Verification
        if (VerifyUser(password, _storedHash))
        {
            Console.WriteLine("Login successful!");
        }
        else
        {
            Console.WriteLine("Login failed. Please try again.");
        }
    }

    public static bool VerifyUser(string inputPassword, string storedHash)
    {
        var hasher = new PasswordHasher<User>();
        var result = hasher.VerifyHashedPassword(new User(), storedHash, inputPassword);
        return result == PasswordVerificationResult.Success;
    }
}
