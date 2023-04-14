﻿using Aequus.Content.Biomes.CrabCrevice.Tiles;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.Nature {
    public class SeaPickle : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SeaPickleTile>());
            Item.value = Item.buyPrice(silver: 2);
        }
    }
}