using FluentAssertions;
using Todo.Api.Mappers;
using Todo.Api.DTOs;

namespace Todo.Api.Tests.Services
{
    public class TaskMapperTests
    {
        [Fact]
        public void ToEntity_ShouldMapCreateDtoToTaskCorrectly()
        {
            var dto = new TaskDtos.Create { Title = "Test", Tags = { "t" } };

            var entity = TaskMapper.ToEntity(dto);

            entity.Title.Should().Be("Test");
            entity.Tags.Should().Contain("t");
            entity.Completed.Should().BeFalse();
        }
    }
}