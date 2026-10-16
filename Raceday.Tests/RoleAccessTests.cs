using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace Raceday.Tests
{
    public class RoleAccessTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        public RoleAccessTests(CustomWebApplicationFactory f) => _client = f.CreateClient();

        [Fact]
        public async Task CreateEvent_WithoutLogin_Returns401()
        {
            var r = await _client.PostAsJsonAsync("/api/events", new { });
            r.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetEvents_IsPublic_Returns200()
        {
            var r = await _client.GetAsync("/api/events");
            r.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetCategories_IsPublic_Returns200()
        {
            var r = await _client.GetAsync("/api/categories");
            r.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetUsers_RequiresOrganiser_Returns401WhenNotLoggedIn()
        {
            var r = await _client.GetAsync("/api/users");
            r.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}