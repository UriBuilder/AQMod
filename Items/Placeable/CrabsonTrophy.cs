﻿using Aequus.Tiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable
{
    public class CrabsonTrophy : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<WallPaintings>(), WallPaintings.Crabson);
            Item.maxStack = 99;
            Item.value = 50000;
            Item.rare = ItemRarityID.Blue;
        }
    }
}