using Todo.Api.DTOs;

namespace Todo.Api.Mappers
{
    public static class TaskMapper
    {
        public static Models.Task ToEntity(TaskDtos.Create dto)
        {
            return new Models.Task
            {
                Title = dto.Title,
                DueDate = dto.DueDate,
                Tags = dto.Tags,
                Completed = false,
                CreatedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Updates a Task entity from an Update DTO if any differences exist.
        /// Only updates the entity and sets <see cref="Models.Task.UpdatedAt"/>
        /// if at least one property has actually changed.
        /// </summary>
        /// <param name="task">The Task entity to be updated.</param>
        /// <param name="dto">The Update DTO containing new values for the Task.</param>
        /// <returns>
        /// <c>true</c> if any property was updated and <see cref="Models.Task.UpdatedAt"/> was changed;
        /// <c>false</c> if no changes were applied (all properties matched the DTO).
        /// </returns>
        public static bool UpdateFromDto(Models.Task task, TaskDtos.Update dto)
        {
            bool changed = false;
            changed |= task.Title != dto.Title;
            changed |= task.Completed != dto.Completed;
            changed |= dto.DueDate != dto.DueDate;
            changed |= !Enumerable.SequenceEqual(dto.Tags, task.Tags);

            task.Title = dto.Title;
            task.Completed = dto.Completed;
            task.DueDate = dto.DueDate;
            task.Tags = dto.Tags;
            if (changed) task.UpdatedAt = DateTime.UtcNow;
            return changed;
        }

        public static TaskDtos.Update ToUpdateDto(Models.Task task)
        {
            return new TaskDtos.Update
            {
                Title = task.Title,
                Completed = task.Completed,
                DueDate = task.DueDate,
                Tags = new List<string>(task.Tags),
            };
        }

        public static TaskDtos.Response ToResponseDto(Models.Task task)
        {
            return new TaskDtos.Response
            {
                Id = task.Id,
                Title = task.Title,
                Completed = task.Completed,
                DueDate = task.DueDate,
                Tags = task.Tags,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt
            };
        }
    }
}
