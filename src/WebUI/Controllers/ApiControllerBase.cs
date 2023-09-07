﻿using System.Security.Claims;
using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace AutoHelper.WebUI.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender? _mediator;

    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

}
