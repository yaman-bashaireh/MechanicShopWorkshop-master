﻿using Asp.Versioning;

using MechanicShop.Contracts.Responses;
using MechanicShop.Infrastructure.Settings;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace MechanicShop.Api.Controllers;

[Route("api/settings")]
[ApiVersionNeutral]
public sealed class SettingsController(IOptions<AppSettings> options) : ApiController
{
    private readonly AppSettings _settings = options.Value;

    [HttpGet("operating-hours")]
    [ProducesResponseType(typeof(OperatingHoursResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Gets the application's operating hours.")]
    [EndpointDescription("Returns the current configured opening and closing times.")]
    [EndpointName("GetOperatingHours")]
    public IActionResult GetOperatingHours()
    {
        return Ok(new OperatingHoursResponse(_settings.OpeningTime, _settings.ClosingTime));
    }
}