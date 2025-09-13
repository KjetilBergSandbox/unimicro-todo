using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Todo.Api.DTOs;
using Todo.Api.Mappers;
using Todo.Api.Data;

namespace Todo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {

        private readonly TodoDbContext _context;

        public TaskController(TodoDbContext context) { _context = context; }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskDtos.Response>>> GetTasks()
        {
            var tasks = await _context.Tasks.ToListAsync();
            return tasks.Select(TaskMapper.ToResponseDto).ToList();
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
