﻿using Aequus.DataSets;
using Aequus.DataSets.Structures.DropRulesChest;
using Aequus.DataSets.Structures.Enums;

namespace Aequus.Core.Utilities;

public static class ExtendChestLoot {
    public static void RegisterCommon(this ChestLootDatabase database, ChestPool type, int item, int minStack = 1, int maxStack = 1, int chanceDemoninator = 1, int chanceNumerator = 1, params Condition[] conditions) {
        database.Register(type, new CommonChestRule(item, minStack, maxStack, chanceDemoninator, chanceNumerator, conditions));
    }

    public static void RegisterIndexed(this ChestLootDatabase database, ChestPool type, params IChestLootRule[] rules) {
        database.Register(type, new IndexedChestRule(rules));
    }

    public static void RegisterOneFromOptions(this ChestLootDatabase database, ChestPool type, params IChestLootRule[] rules) {
        database.Register(type, new OneFromOptionsChestRule(rules));
    }

    public static IChestLootRule OnSucceed(this IChestLootRule parentRule, IChestLootRule rule) {
        parentRule.ChainedRules.Add(new OnSuccessChestChain(rule));
        return parentRule;
    }

    public static IChestLootRule OnFailure(this IChestLootRule parentRule, IChestLootRule rule) {
        parentRule.ChainedRules.Add(new OnFailChestChain(rule));
        return parentRule;
    }

    public static bool CanDropWithConditions(this IChestLootRule rule, in ChestLootInfo info) {
        return rule.CanDrop(in info) && (rule.Conditions?.IsMet() == false ? false : true);
    }

    public static void ClearSelfAndChains(this IChestLootRule rule) {
        rule.Reset();
        foreach (var c in rule.ChainedRules) {
            c.RuleToChain?.ClearSelfAndChains();
        }
    }
}