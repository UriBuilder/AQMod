﻿using Terraria.GameContent.ItemDropRules;

namespace Aequu2.Core.Entities.Items.DropRules;

public class ConditionManaCrystals : IItemDropRuleCondition {
    public bool CanDrop(DropAttemptInfo info) {
        return info.player.ConsumedManaCrystals < Player.ManaCrystalMax;
    }

    public bool CanShowItemDropInUI() {
        return true;
    }

    public string GetConditionDescription() {
        return null;
    }
}
