using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Todo.Api.DTOs;
using Todo.Api.Mappers;
using Todo.Api.Data;
using Todo.Api.Services;

namespace Todo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {

        private readonly TodoDbContext _context;
        private readonly ITaskQueryEngine _queryEngine;

        public TaskController(TodoDbContext context, ITaskQueryEngine queryEngine)
        {
            _context = context;
            _queryEngine = queryEngine;
        }

        [HttpGet]
        public async Task<ActionResult<TaskDtos.PagedResponse>> GetTasks(
            [FromQuery] string? query,
            [FromQuery] List<string>? tags,
            [FromQuery] int limit = 25,
            [FromQuery] int offset = 0)
        {
            const int MaxLimit = 100;
            limit = Math.Clamp(limit, 0, MaxLimit);
            offset = Math.Max(0, offset);

            var dbQuery = _context.Tasks.AsQueryable();
            dbQuery = _queryEngine.ApplyFilters(_context.Tasks.AsQueryable(), tags);
            var candidates = await dbQuery.ToListAsync();

            if (!string.IsNullOrWhiteSpace(query)) candidates = _queryEngine.ApplyOrdering(candidates, query).ToList();

            var paged = candidates.Skip(offset).Take(limit).ToList();

            var next = offset + limit < candidates.Count ? Url.Action(nameof(GetTasks), new { query, tags, limit, offset = offset + limit }) : null;
            var prev = offset > 0 ? Url.Action(nameof(GetTasks), new { query, tags, limit, offset = Math.Max(0, offset - limit) }) : null;

            return new TaskDtos.PagedResponse
            {
                Items = paged.Select(TaskMapper.ToResponseDto).ToList(),
                Total = candidates.Count,
                Limit = limit,
                Offset = offset,
                Next = next,
                Prev = prev
            };
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskDtos.Response>> GetTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound(TaskNotFound(id));

            return TaskMapper.ToResponseDto(task);
        }

        [HttpPost]
        public async Task<ActionResult<TaskDtos.Response>> CreateTask([FromBody] TaskDtos.Create dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = TaskMapper.ToEntity(dto);
            _context.Tasks.Add(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTask), new { id = entity.Id }, TaskMapper.ToResponseDto(entity));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskDtos.Update dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound(TaskNotFound(id));

            TaskMapper.UpdateFromDto(task, dto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchTask(int id, [FromBody] JsonPatchDocument<TaskDtos.Update> patch)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound(TaskNotFound(id));

            var template = TaskMapper.ToUpdateDto(task);

            patch.ApplyTo(template, ModelState);

            if (!TryValidateModel(template)) return ValidationProblem(ModelState);

            bool changed = TaskMapper.UpdateFromDto(task, template);
            if (changed) await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound(TaskNotFound(id));

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private static string TaskNotFound(int id) => $"Task with id {id} not found.";

    }
}
