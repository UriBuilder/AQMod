﻿using Aequus.Tiles.CraftingStation;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.Furniture.CraftingStation
{
    public class RecyclingMachine : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<RecyclingMachineTile>());
            Item.maxStack = 9999;
            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Blue;
        }
    }
}