﻿using Terraria;
using Terraria.ModLoader;

namespace Aequus.Tiles.Paintings.Canvas3x3 {
    public class GoreNestPainting2 : ModItem {
        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<WallPaintings3x3>(), WallPaintings3x3.GoreNest2Painting);
            Item.value = Item.buyPrice(gold: 1);
        }
    }
}