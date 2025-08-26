using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Workorders.Enums;
using MechanicShop.Infrastructure.Settings;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MechanicShop.Infrastructure.Services;

public class WorkOrderPolicy(IOptions<AppSettings> options, IAppDbContext context) : IWorkOrderPolicy
{
    private readonly AppSettings _appSettings = options.Value;
    private readonly IAppDbContext _context = context;

    public async Task<Result<Success>> CheckSpotAvailabilityAsync(Spot spot, DateTimeOffset startAt, DateTimeOffset endAt, Guid? excludeWorkOrderId = null, CancellationToken ct = default)
    {
        var isOccupied = await _context.WorkOrders.AnyAsync(
            a =>
            a.Spot == spot &&
            a.StartAtUtc < endAt &&
            a.EndAtUtc > startAt &&
            (!excludeWorkOrderId.HasValue || a.Id != excludeWorkOrderId.Value),
            ct);

        return isOccupied
             ? Error.Conflict("MechanicShop_Spot_Full", "The selected time slot is unavailable for the requested services.")
             : Result.Success;
    }

    public Result<Success> ValidateMinimumRequirement(DateTimeOffset startAt, DateTimeOffset endAt)
    {
        if ((endAt - startAt) < TimeSpan.FromMinutes(_appSettings.MinimumAppointmentDurationInMinutes))
        {
            return Error.Conflict(
                "WorkOrder_TooShort",
                $"WorkOrder duration must be at least {_appSettings.MinimumAppointmentDurationInMinutes} minutes.");
        }

        return Result.Success;
    }

    public async Task<bool> IsLaborOccupied(Guid laborId, Guid excludedWorkOrderId, DateTimeOffset startAt, DateTimeOffset endAt)
    {
        return await _context.WorkOrders.AnyAsync(a =>
            a.LaborId == laborId &&
            a.Id != excludedWorkOrderId &&
            a.StartAtUtc < endAt &&
            a.EndAtUtc > startAt);
    }

    public bool IsOutsideOperatingHours(DateTimeOffset startAt, TimeSpan duration)
    {
        var opening = startAt.Date.Add(_appSettings.OpeningTime.ToTimeSpan());
        var closing = startAt.Date.Add(_appSettings.ClosingTime.ToTimeSpan());
        var endAt = startAt + duration;

        return startAt < opening || endAt > closing;
    }

    public async Task<bool> IsVehicleAlreadyScheduled(
        Guid vehicleId,
        DateTimeOffset startAt,
        DateTimeOffset endAt,
        Guid? excludedWorkOrderId = null)
    {
        var overlapping = await _context.WorkOrders
    .Where(a =>
        a.VehicleId == vehicleId &&
        (excludedWorkOrderId == null || a.Id != excludedWorkOrderId) &&
        a.StartAtUtc < endAt &&
        a.EndAtUtc > startAt)
    .ToListAsync();

        return await _context.WorkOrders.AnyAsync(a =>
            a.VehicleId == vehicleId &&
            (excludedWorkOrderId == null || a.Id != excludedWorkOrderId) &&
            a.StartAtUtc < endAt &&
            a.EndAtUtc > startAt);
    }
}