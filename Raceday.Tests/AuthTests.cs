using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Raceday.Controllers;
using Xunit;

namespace Raceday.Tests
{
    public class AuthTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        public AuthTests(CustomWebApplicationFactory f) => _client = f.CreateClient();

        [Fact]
        public async Task Register_WithValidData_Returns201()
        {
            var dto = new AuthController.RegisterDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = $"test{Guid.NewGuid()}@raceday.co.za",
                Password = "Test123!",
                Role = "Organiser"
            };
            var r = await _client.PostAsJsonAsync("/api/auth/register", dto);
            r.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task Register_WithInvalidRole_Returns400()
        {
            var dto = new AuthController.RegisterDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = $"test{Guid.NewGuid()}@raceday.co.za",
                Password = "Test123!",
                Role = "Admin"
            };
            var r = await _client.PostAsJsonAsync("/api/auth/register", dto);
            r.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Login_WithWrongPassword_Returns401()
        {
            var dto = new AuthController.LoginDto
            { Email = "nope@raceday.co.za", Password = "wrong" };
            var r = await _client.PostAsJsonAsync("/api/auth/login", dto);
            r.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Login_WithCorrectCredentials_Returns200()
        {
            var email = $"login{Guid.NewGuid()}@raceday.co.za";
            await _client.PostAsJsonAsync("/api/auth/register", new AuthController.RegisterDto
            {
                FirstName = "Login",
                LastName = "Test",
                Email = email,
                Password = "Test123!",
                Role = "Participant"
            });
            var r = await _client.PostAsJsonAsync("/api/auth/login",
                new AuthController.LoginDto { Email = email, Password = "Test123!" });
            r.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}