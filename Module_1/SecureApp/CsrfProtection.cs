using System;
using System.Security.Cryptography;

public class CsrfProtection
{
    public string GenerateToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    }

    public bool ValidateToken(string token, string sessionToken)
    {
        return token == sessionToken;
    }
}