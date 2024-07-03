﻿using Aequu2.Core.ContentGeneration;
using Aequu2.Core.Entities.Items.Components;
using Terraria.Localization;

namespace Aequu2.Old.Content.Fishing.Poppers;

[LegacyName("CursedPopper")]
public class CorruptPopper : UnifiedModBait, IModifyFishingPower {
    public static float IncreasedFishingPowerInCorruption { get; set; } = 0.3f;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(XLanguage.Percent(IncreasedFishingPowerInCorruption));

    public override void SetDefaults() {
        Item.bait = 30;
        Item.value = Item.sellPrice(silver: 10);
        Item.rare = ItemRarityID.LightRed;
        Item.width = 6;
        Item.height = 6;
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
    }

    public void ModifyFishingPower(Player player, Item fishingRod, ref float fishingLevel) {
        if (player.ZoneCorrupt) {
            fishingLevel += IncreasedFishingPowerInCorruption;
        }
    }
}