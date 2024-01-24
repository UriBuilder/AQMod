﻿namespace Aequus.Old.Content.Events.DemonSiege.Spawners;

public class UnholyCore : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 3;
        DemonSiegeSystem.RegisterSacrifice(new SacrificeData(Type, Type, EventTier.PreHardmode));
    }

    public override void SetDefaults() {
        Item.width = 20;
        Item.height = 20;
        Item.consumable = true;
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ItemRarityID.Green;
        Item.value = Item.buyPrice(gold: 1);
    }
}