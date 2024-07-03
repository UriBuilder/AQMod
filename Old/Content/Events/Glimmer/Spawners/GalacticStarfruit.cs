﻿using Aequu2.DataSets;
using Aequu2.Old.Content.Bosses.OmegaStarite;
using System.Linq;
using Terraria.Audio;

namespace Aequu2.Old.Content.Events.Glimmer.Spawners;

public class GalacticStarfruit : ModItem {
    public override void SetStaticDefaults() {
        ItemSets.SortingPriorityBossSpawns[Type] = ItemSets.SortingPriorityBossSpawns[ItemID.BloodySpine];
        Item.ResearchUnlockCount = 3;
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.SuspiciousLookingEye);
        Item.width = 20;
        Item.height = 20;
        Item.rare = ItemRarityID.Green;
        Item.consumable = true;
        Item.maxStack = Item.CommonMaxStack;
        Item.value = Item.buyPrice(gold: 2);
    }

    public override bool CanUseItem(Player player) {
        return !Main.dayTime && !GlimmerZone.EventTechnicallyActive && !NPC.AnyNPCs(ModContent.NPCType<OmegaStarite>());
    }

    public override bool? UseItem(Player player) {
        SoundEngine.PlaySound(SoundID.Roar, player.position);
        if (Main.myPlayer == player.whoAmI) {
            GlimmerSystem.BeginEvent();
        }
        return GlimmerZone.EventTechnicallyActive;
    }

    public override void AddRecipes() {
        foreach (int bar in ItemTypeVariantDataSet.DemoniteBar.Where(i => i.ValidEntry)) {
            CreateRecipe()
                .AddIngredient(ItemID.FallenStar, 5)
                .AddIngredient(bar, 1)
                .AddTile(TileID.DemonAltar)
                .Register()
                .SortAfterFirstRecipesOf(ItemID.DeerThing);
        }
    }
}