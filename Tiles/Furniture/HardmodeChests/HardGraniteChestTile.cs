﻿using Aequus.Content.WorldGeneration;
using Aequus.Items.Placeable.Furniture.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.Furniture.HardmodeChests
{
    public class HardGraniteChestTile : BaseChest
    {
        public override void SetStaticDefaults()
        {
            HardmodeChestBoost.CountsAsChest[Type] = new(TileID.Containers, ChestType.Gold);
            ChestType.IsGenericUndergroundChest.Add(new TileKey(Type));
            base.SetStaticDefaults();
            DustType = DustID.t_Frozen;
            ItemDrop = ModContent.ItemType<HardGraniteChest>();
            AddMapEntry(new Color(100, 255, 255), CreateMapEntryName());
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            DrawBasicGlowmask(i, j, spriteBatch);
        }
    }
}