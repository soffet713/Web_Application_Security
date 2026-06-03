var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<EncryptionService>();

builder.Services.AddControllers();
var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

app.MapPost("/register", (UserDto userDto, EncryptionService encryptionService) =>
{
    if (string.IsNullOrWhiteSpace(userDto.Name) ||
        string.IsNullOrWhiteSpace(userDto.Email) ||
        string.IsNullOrWhiteSpace(userDto.Password))
    {
        return Results.BadRequest(new { error = "Name, email, and password are required." });
    }
    string encryptedEmail = encryptionService.Encrypt(userDto.Email);

    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

    var user = new User
    {
        Id = SimulatedDatabase.Users.Count + 1,
        Name = userDto.Name,
        Email = encryptedEmail,
        Password = hashedPassword
    };

    SimulatedDatabase.InsertUser(user);

	Console.WriteLine($"User stored: Name={user.Name}, Email={user.Email}, Password={user.Password}");
    return Results.Created($"/users/{user.Id}", new { message = "User registered successfully." });
});

app.Run();

