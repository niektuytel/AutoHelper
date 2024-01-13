using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Domain.Entities.Conversations;
using AutoHelper.Domain.Entities.Conversations.Enums;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;
using MediatR;
using AutoHelper.Application.Conversations._DTOs;
using Hangfire;
using AutoHelper.Application.Common.Extensions;
using AutoHelper.Application.Conversations.Commands.SendMessage;
using System.Text.Json.Serialization;

namespace AutoHelper.Application.Conversations.Commands.StartConversationItems;

public record StartConversationItemsCommand: IQueueRequest<Unit>
{
    public StartConversationItemsCommand(List<Guid> conversationIds)
    {
        ConversationIds = conversationIds;
    }

    public List<Guid> ConversationIds { get; set; }

    [JsonIgnore]
    internal IEnumerable<ConversationItem> ConversationItems { get; set; } = null!;

    [JsonIgnore]
    public IQueueService QueueingService { get; set; } = null!;
}

public class StartGarageConversationBatchCommandHandler : IRequestHandler<StartConversationItemsCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly ISender _mediator;

    public StartGarageConversationBatchCommandHandler(IApplicationDbContext context, ISender mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    public async Task<Unit> Handle(StartConversationItemsCommand request, CancellationToken cancellationToken)
    {
        foreach (var conversation in request.ConversationItems)
        {
            var message = conversation.Messages
                .OrderBy(item => item.LastModified)
                .LastOrDefault();

            if (message == null)
            {
                continue;
            }

            var messageCommand = new SendMessageCommand(message.Id);
            var result = await _mediator.Send(messageCommand, cancellationToken);

            request.QueueingService.LogInformation(result);
        }

        return Unit.Value;
    }

}