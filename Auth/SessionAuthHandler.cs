using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Raceday.Auth
{
    /// <summary>
    /// Custom authentication handler that reads the user's ID and Role from the session
    /// and turns them into a ClaimsPrincipal, enabling [Authorize(Roles = "...")] to work.
    /// </summary>
    public class SessionAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public SessionAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var userId = Context.Session.GetInt32("UserID");
            var role = Context.Session.GetString("Role");
            var email = Context.Session.GetString("Email");

            if (userId == null || string.IsNullOrEmpty(role))
                return Task.FromResult(AuthenticateResult.NoResult());

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString()),
                new Claim(ClaimTypes.Email, email ?? ""),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, "SessionAuth");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "SessionAuth");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}