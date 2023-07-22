﻿using Corelibs.Basic.DDD;
using Manabu.Entities.Shared;

namespace Manabu.Entities.Plans;

public record PlanAimId(string Value) : EntityId(Value);

public class PlanAim : Aim<PlanId, PlanAimId>, IAggregateRoot<PlanAimId>
{
    public const string DefaultCollectionName = "planAims";

    public PlanAim(
        PlanId planId,
        DateTime startedTime) : base(planId, startedTime) {}
}
