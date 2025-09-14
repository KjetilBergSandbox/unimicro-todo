using FluentAssertions;
using Todo.Api.Services;

namespace Todo.Api.Tests.Services
{
    public class TaskQueryEngineTests
    {
        private readonly TaskQueryEngine _engine = new();

        [Fact]
        public void ApplyFilters_ShouldReturnOnlyTasksWithMatchingTags()
        {
            var tasks = new List<Models.Task>
            {
                new() { Title = "A", Tags = new List<string>{"a","b"} },
                new() { Title = "B", Tags = new List<string>{"b"} },
                new() { Title = "C", Tags = new List<string>{"c"} }
            };

            var queryable = tasks.AsQueryable();

            var result = _engine.ApplyFilters(queryable, new List<string> { "b" });

            result.Should().HaveCount(2);
            result.Should().OnlyContain(t => t.Tags.Contains("b"));
        }

        [Fact]
        public void ApplyOrdering_ShouldSortTasksByFuzzySimilarity()
        {
            var tasks = new List<Models.Task>
            {
                new() { Title = "apricot" },
                new() { Title = "banana" },
                new() { Title = "apple" }
            };

            var result = _engine.ApplyOrdering(tasks, "apple");

            result.First().Title.Should().Be("apple");
            result.Last().Title.Should().Be("banana");
        }
    }
}