﻿using Aequus.Common.Structures.Conditions;
using System.Collections.Generic;

namespace Aequus.Systems.Chests.DropRules;

/// <summary>Usually used in conjunction with <see cref="ReplaceItemChestRule"/> to pull all of the item slots backwards after deleting the item.</summary>
public class RemoveItemChestRule : IChestLootRule {
    public List<IChestLootChain> ChainedRules { get; set; } = new();
    public ConditionCollection Conditions { get; set; } = null;

    public ChestLootResult AddItem(in ChestLootInfo info) {
        info.Add.RemoveItem(in info);
        return ChestLootResult.Success;
    }
}
