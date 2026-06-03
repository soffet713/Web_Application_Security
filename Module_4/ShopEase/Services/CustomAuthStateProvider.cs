using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

namespace ShopEase.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        // Change this boolean to 'false' to test what logged-out users see!
        private readonly bool _isUserLoggedIn = true;

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            ClaimsIdentity identity;

            if (_isUserLoggedIn)
            {
                // Simulate a securely logged-in user named "TestCustomer"
                var claims = new[] { 
                    new Claim(ClaimTypes.Name, "TestCustomer"),
                    new Claim(ClaimTypes.Role, "Customer") 
                };
                identity = new ClaimsIdentity(claims, "MockAuthenticationType");
            }
            else
            {
                // Simulate an anonymous/logged-out visitor
                identity = new ClaimsIdentity();
            }

            var user = new ClaimsPrincipal(identity);
            return Task.FromResult(new AuthenticationState(user));
        }
    }
}