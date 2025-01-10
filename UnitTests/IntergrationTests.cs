using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace UnitTests
{
    public class IntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _httpClient;

        // Constructor that receives the factory to set up the HTTP client
        public IntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _httpClient = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [SetUp]
        public void Setup()
        {
            // This method is executed before each test. You can use it to set up any necessary test environment.
        }

        [Test]
        public async Task GetAllProjects_ShouldReturnOk_WhenValidUser()
        {
            // Arrange: Set up any necessary variables or mock data.
            var userId = "validUserId"; // Replace with actual valid user ID or simulate authentication
            
            // Act: Send a GET request to the API endpoint to retrieve all projects.
            var response = await _httpClient.GetAsync($"api/projects?userId={userId}");
            
            // Assert: Verify that the response status is OK (200).
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode);

            // Optionally, you can check the response content to ensure the correct projects are returned.
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.IsNotEmpty(responseBody);  // Ensure that there is content returned
        }

        [Test]
        public async Task CreateProject_ShouldReturnBadRequest_WhenErrorCreatingContainer()
        {
            // Arrange: Prepare invalid project data that would trigger a bad request.
            var invalidProjectData = new
            {
                name = "", // Invalid project name
                description = "This is a description of the project."
            };

            var content = new StringContent(
                Newtonsoft.Json.JsonConvert.SerializeObject(invalidProjectData),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            // Act: Send a POST request to create a project with the invalid data.
            var response = await _httpClient.PostAsync("api/projects", content);

            // Assert: Verify that the response status is BadRequest (400).
            Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
