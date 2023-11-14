using AutoHelper.Application.Common.Mappings;
using AutoHelper.Domain.Entities.Garages;

namespace AutoHelper.Application.Garages._DTOs;

public class GarageLookupSimplefiedDto
{
    public string Identifier { get; set; } = null!;
    public string Name { get; set; } = null!;
}