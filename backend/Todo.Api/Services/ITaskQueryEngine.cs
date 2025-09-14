namespace Todo.Api.Services
{
    public interface ITaskQueryEngine
    {
        IQueryable<Models.Task> ApplyFilters(IQueryable<Models.Task> query, List<string>? tags);

        IEnumerable<Models.Task> ApplyOrdering(IEnumerable<Models.Task> tasks, string? title);
    }
}
