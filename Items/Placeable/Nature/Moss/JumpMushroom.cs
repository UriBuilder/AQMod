﻿using Aequus.Tiles.Moss;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.Nature.Moss
{
    public class JumpMushroom : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<JumpMushroomTile>());
        }
    }
}