using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    // Inject our services instead of creating them with 'new'
    private readonly EncryptionService _encryptionService;

    public UserController(EncryptionService encryptionService)
    {
        _encryptionService = encryptionService;
    }

    [HttpPost("register")]
    public IActionResult Register(UserDto userDto)
    {
        if (string.IsNullOrWhiteSpace(userDto.Name) || 
            string.IsNullOrWhiteSpace(userDto.Email) || 
            string.IsNullOrWhiteSpace(userDto.Password))
        {
            return BadRequest(new { error = "All fields are required." });
        }

        // 1. Encrypt sensitive data (Email)
        string encryptedEmail = _encryptionService.Encrypt(userDto.Email);
        
        // 2. Generate Salt, then Hash the password (using BCrypt)
        string salt = BCrypt.Net.BCrypt.GenerateSalt();
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(userDto.Password, salt);

        var user = new User
        {
            // Use the list inside our simulated database
            Id = SimulatedDatabase.Users.Count + 1,
            Name = userDto.Name,
            Email = encryptedEmail,
            Password = hashedPassword,
        };

        SimulatedDatabase.InsertUser(user);
        return Created($"/users/{user.Id}", new { message = "User registered successfully." });
    }

    [HttpPost("login")] // Changed from "Post" to "login" for clarity
    public IActionResult Login(LoginDto loginDto)
    {
        // Look in our simulated database
        var user = SimulatedDatabase.Users.FirstOrDefault(u => u.Name == loginDto.Name);
        
        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
        {
            return Unauthorized(new { error = "Invalid credentials." });
        }
        
        return Ok(new { message = "Login successful." });
    }
}
