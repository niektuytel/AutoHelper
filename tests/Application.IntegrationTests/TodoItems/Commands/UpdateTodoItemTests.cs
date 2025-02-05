﻿using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.TodoItems.Commands.CreateTodoItem;
using AutoHelper.Application.TodoItems.Commands.UpdateTodoItem;
using AutoHelper.Application.TodoLists.Commands.CreateTodoList;
using AutoHelper.Domain.Entities.Deprecated;
using FluentAssertions;
using NUnit.Framework;

namespace AutoHelper.Application.IntegrationTests.TodoItems.Commands;

using static Testing;

public class UpdateTodoItemTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireValidTodoItemId()
    {
        var command = new UpdateTodoItemCommand { Id = Guid.Empty, Title = "New Title" };
        await FluentActions.Invoking(() => SendAsync(command)).Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task ShouldUpdateTodoItem()
    {
        //var userId = await RunAsDefaultUserAsync();

        //var listId = await SendAsync(new CreateTodoListCommand
        //{
        //    Title = "New List"
        //});

        //var itemId = await SendAsync(new CreateTodoItemCommand
        //{
        //    ListId = listId,
        //    Title = "New Item"
        //});

        //var command = new UpdateTodoItemCommand
        //{
        //    Id = itemId,
        //    Title = "Updated Item Title"
        //};

        //await SendAsync(command);

        //var item = await FindAsync<TodoItem>(itemId);

        //item.Should().NotBeNull();
        //item!.Title.Should().Be(command.Title);
        //item.LastModifiedBy.Should().NotBeNull();
        //item.LastModifiedBy.Should().Be(userId);
        //item.LastModified.Should().NotBeNull();
        //item.LastModified.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}
