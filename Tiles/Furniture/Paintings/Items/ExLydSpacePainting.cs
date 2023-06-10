﻿using Aequus.NPCs.Town.PhysicistNPC.Analysis;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Tiles.Furniture.Paintings.Items {
    public class ExLydSpacePainting : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
            AnalysisSystem.IgnoreItem.Add(Type);
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<WallPaintings>(), WallPaintings.ExLydSpacePainting);
            Item.value = Item.buyPrice(gold: 2);
        }
    }
}