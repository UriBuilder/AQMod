﻿using AequusRemake.DataSets.Structures.DropRulesChest;
using AequusRemake.Systems.Fishing;
using Terraria.GameContent.ItemDropRules;

namespace AequusRemake.Core.Structures.Conversion;

public interface IConvertDropRules {
    IItemDropRule ToItemDropRule();
    IChestLootRule ToChestDropRule();
    IFishDropRule ToFishDropRule() { return null; }
}
