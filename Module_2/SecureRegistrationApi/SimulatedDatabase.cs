public class User
{
    public int Id { get; set; } = 0;
    public required string Name { get; set; } = string.Empty;
    public required string Email { get; set; } = string.Empty;
    public required string Password { get; set; } = string.Empty;
}

public class UserDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}

public class LoginDto
{
    public string Name { get; set; }
    public string Password {get; set; }
}

public static class SimulatedDatabase
{
    public static List<User> Users { get; } = new List<User>();

    public static void InsertUser(User user)
    {
        Users.Add(user);
    }

    public static User? FindUserByEmail(string email)
    {
        return Users.Find(u => u.Email == email);
    }

    public static List<User> GetAllUsers() => Users;
}
