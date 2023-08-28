using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Domain.Entities.Deprecated;
using AutoHelper.Domain.Events;
using MediatR;

namespace AutoHelper.Application.TodoItems.Commands.DeleteTodoItem;

public record DeleteTodoItemCommand(Guid Id) : IRequest;

public class DeleteTodoItemCommandHandler : IRequestHandler<DeleteTodoItemCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteTodoItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteTodoItemCommand request, CancellationToken cancellationToken)
    {
        //var entity = await _context.TodoItems
        //    .FindAsync(new object[] { request.Id }, cancellationToken);

        //if (entity == null)
        //{
        //    throw new NotFoundException(nameof(TodoItem), request.Id);
        //}

        //_context.TodoItems.Remove(entity);

        //entity.AddDomainEvent(new TodoItemDeletedEvent(entity));

        //await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
