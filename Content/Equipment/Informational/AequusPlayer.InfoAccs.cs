﻿using Aequus.Content.Equipment.Informational.Monocle;
using Aequus.Core.CodeGeneration;

namespace Aequus;

public partial class AequusPlayer {
    [ResetEffects]
    public bool accInfoDayCalendar;

    [ResetEffects]
    public bool accInfoDPSMeterDebuff;

    [ResetEffects]
    public bool accMonocle;
    [ResetEffects]
    public bool accShimmerMonocle;

    public bool ShowMonocle => accMonocle && ModContent.GetInstance<MonocleBuilderToggle>().CurrentState == 0;
    public bool ShowShimmerMonocle => accShimmerMonocle && ModContent.GetInstance<ShimmerMonocleBuilderToggle>().CurrentState == 0;
}