using Todo.Api.Data;

namespace Todo.Api.IntegrationTests.SeedData
{
    public static class TaskSeedData
    {
        public static void Seed(TodoDbContext context)
        {
            if (context.Tasks.Any()) return;

            var tasks = new List<Models.Task>
            {
                new () { Title = "Add Testing Data to DB", Tags = { "professional", "testing" }, Completed = true, CreatedAt = DateTime.UtcNow },
                new () { Title = "Perform More Tests", Tags = { "professional", "testing" }, Completed = false, CreatedAt = DateTime.UtcNow },
                new () { Title = "Make Dinner (Maybe)", Tags = { "personal", "cooking" }, Completed = false, CreatedAt = DateTime.UtcNow }
            };

            context.Tasks.AddRange(tasks);
            context.SaveChanges();
        }
    }
}