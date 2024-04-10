﻿using Aequus.Common.Systems;
using Terraria.Localization;

namespace Aequus.Content.Equipment.Informational.Calendar;

public class CalendarInfoDisplay : InfoDisplay {
    private LocalizedText Tooltip;

    public override void Load() {
    }

    public override bool Active() {
        return CalendarTile.Nearby || Main.LocalPlayer.GetModPlayer<AequusPlayer>().accInfoDayCalendar;
    }

    public override string DisplayValue(ref Color displayColor, ref Color displayShadowColor) {
        Tooltip ??= this.GetLocalization("Tooltip");
        return string.Format(Tooltip.Value, ExtendLanguage.DayOfWeek(TimeSystem.DayOfTheWeek).Value, TimeSystem.DaysPassed);
    }
}