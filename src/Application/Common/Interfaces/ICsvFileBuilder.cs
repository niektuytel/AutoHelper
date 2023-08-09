using AutoHelper.Application.TodoLists.Queries.ExportTodos;

namespace AutoHelper.Application.Common.Interfaces;

public interface ICsvFileBuilder
{
    byte[] BuildTodoItemsFile(IEnumerable<TodoItemRecord> records);
}
