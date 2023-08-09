using AutoHelper.Application.Common.Interfaces;

namespace AutoHelper.Infrastructure.Services;

public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}
