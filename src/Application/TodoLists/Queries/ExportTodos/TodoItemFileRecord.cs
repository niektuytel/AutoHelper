using AutoHelper.Application.Common.Mappings;
using AutoHelper.Domain.Entities.Deprecated;

namespace AutoHelper.Application.TodoLists.Queries.ExportTodos;

public class TodoItemRecord : IMapFrom<TodoItem>
{
    public string? Title { get; set; }

    public bool Done { get; set; }
}
