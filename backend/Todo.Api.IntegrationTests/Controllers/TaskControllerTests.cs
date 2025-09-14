using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.JsonPatch;
using FluentAssertions;
using Newtonsoft.Json;
using System.Text;
using Todo.Api.Data;
using Todo.Api.DTOs;
using Todo.Api.IntegrationTests.Helpers;
using Todo.Api.IntegrationTests.SeedData;

namespace Todo.Api.IntegrationTests.Controllers
{
    public class TaskControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public TaskControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();

            var scope = factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
            TaskSeedData.Seed(context);
        }

        [Fact]
        public async Task GetTasks_ReturnsAllTasks()
        {
            var response = await _client.GetAsync("/api/task");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var paged = await response.Content.ReadFromJsonAsync<TaskDtos.PagedResponse>();
            paged.Should().NotBeNull();
            paged!.Items.Count.Should().BeGreaterThan(0);
            paged.Items.Select(t => t.Title).Should().Contain("Perform More Tests");
        }

        [Fact]
        public async Task CreateTask_ReturnsCreatedTask()
        {
            var dto = new TaskDtos.Create { Title = "Another Task" };

            var response = await _client.PostAsJsonAsync("/api/task", dto);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var created = await response.Content.ReadFromJsonAsync<TaskDtos.Response>();
            created!.Title.Should().Be("Another Task");
        }

        [Fact]
        public async Task PutTask_ShouldUpdateExistingTask()
        {
            var create = new TaskDtos.Create { Title = "Original", Tags = { "personal" } };
            var post = await _client.PostAsJsonAsync("/api/task", create);
            var created = await post.Content.ReadFromJsonAsync<TaskDtos.Response>();

            var update = new TaskDtos.Update { Title = "Updated", Tags = { "professional" } };
            var put = await _client.PutAsJsonAsync($"/api/task/{created!.Id}", update);

            put.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var got = await _client.GetAsync($"/api/task/{created.Id}");
            var updated = await got.Content.ReadFromJsonAsync<TaskDtos.Response>();

            updated.Should().NotBeNull();
            updated!.Title.Should().Be("Updated");
            updated.Tags.Should().Contain("professional").And.NotContain("personal");
        }

        [Fact]
        public async Task PatchTask_ShouldApplyPartialUpdate()
        {
            var create = new TaskDtos.Create { Title = "PatchMe" };
            var post = await _client.PostAsJsonAsync("/api/task", create);
            var created = await post.Content.ReadFromJsonAsync<TaskDtos.Response>();

            var jpd = new JsonPatchDocument<TaskDtos.Update>();
            jpd.Replace(t => t.Completed, true);
            var body = new StringContent(JsonConvert.SerializeObject(jpd), Encoding.UTF8, "application/json-patch+json");
            var patch = await _client.PatchAsync($"/api/task/{created!.Id}", body);

            patch.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var got = await _client.GetAsync($"/api/task/{created.Id}");
            var updated = await got.Content.ReadFromJsonAsync<TaskDtos.Response>();

            updated.Should().NotBeNull();
            updated!.Title.Should().Be("PatchMe");
            updated.Completed.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteTask_ShouldRemoveTask()
        {
            var create = new TaskDtos.Create { Title = "DeleteMe", Tags = { "Temporary" } };
            var post = await _client.PostAsJsonAsync("/api/task", create);
            var created = await post.Content.ReadFromJsonAsync<TaskDtos.Response>();

            var delete = await _client.DeleteAsync($"/api/task/{created!.Id}");

            delete.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var got = await _client.GetAsync($"/api/task/{created.Id}");
            got.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}