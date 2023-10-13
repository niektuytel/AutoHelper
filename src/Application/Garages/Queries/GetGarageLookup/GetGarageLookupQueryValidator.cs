using AutoHelper.Application.Garages.Queries.GetGaragesLookups;
using AutoHelper.Application.TodoItems.Queries.GetTodoItemsWithPagination;
using FluentValidation;

namespace AutoHelper.Application.Garages.Queries.GetGarageLookup;

public class GetGarageLookupQueryValidator : AbstractValidator<GetGarageLookupQuery>
{
    public GetGarageLookupQueryValidator()
    {
        RuleFor(x => x.Identifier)
            .NotEmpty()
            .WithMessage("Identifier is required.");

    }
}
