using Microsoft.EntityFrameworkCore;

namespace Todo.Api.Data
{
    public class TodoDbContext : DbContext
    {
        public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options) { }

        public DbSet<Models.Task> Tasks { get; set; }
    }
}
