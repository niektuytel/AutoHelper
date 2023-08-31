﻿namespace AutoHelper.Application.Common.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? UserName { get; }
    string? UserEmail { get; }
}
