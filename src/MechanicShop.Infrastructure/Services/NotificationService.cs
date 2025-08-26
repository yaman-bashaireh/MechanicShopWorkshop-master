﻿using MechanicShop.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Infrastructure.Services;

public sealed class NotificationService(ILogger<NotificationService> logger) : INotificationService
{
    private const string Message = "Your vehicle service is complete. You may collect it from the shop at your earliest convenience.";

    public async Task SendEmailAsync(string to, CancellationToken cancellationToken = default)
    {
        var at = to.IndexOf('@');
        var maskedEmail = at > 1
            ? to[0] + new string('*', at - 2) + to[at - 1] + to[at..]
            : "*****";

        logger.LogInformation("[Email] To: {Email} | Message: {Message}", maskedEmail, Message);

        // Simulated email send
        await Task.CompletedTask;
    }

    public async Task SendSmsAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        var masked = phoneNumber.Length >= 4
            ? new string('*', phoneNumber.Length - 4) + phoneNumber[^4..]
            : "****";

        logger.LogInformation("[SMS] To: {Phone} | Message: {Message}", masked, Message);

        // Simulated SMS send
        await Task.CompletedTask;
    }
}