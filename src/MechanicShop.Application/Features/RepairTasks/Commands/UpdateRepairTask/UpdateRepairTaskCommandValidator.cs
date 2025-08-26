﻿using FluentValidation;

namespace MechanicShop.Application.Features.RepairTasks.Commands.UpdateRepairTask;

public class UpdateRepairTaskCommandValidator : AbstractValidator<UpdateRepairTaskCommand>
{
    public UpdateRepairTaskCommandValidator()
    {
        RuleFor(x => x.RepairTaskId)
            .NotEmpty().WithMessage("Repair task ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Task name is required.")
            .MaximumLength(100);

        RuleFor(x => x.LaborCost)
            .InclusiveBetween(1, 10_000)
            .WithMessage("Labor cost must be between 1 and 10,000.");

        RuleFor(x => x.EstimatedDurationInMins)
            .IsInEnum()
            .WithMessage("Invalid duration selected.");

        RuleFor(x => x.Parts)
            .NotNull()
            .Must(p => p.Count > 0)
            .WithMessage("At least one part is required.");

        RuleForEach(x => x.Parts)
            .SetValidator(new UpdateRepairTaskPartCommandValidator());
    }
}