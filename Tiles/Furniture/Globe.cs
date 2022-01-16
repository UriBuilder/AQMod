﻿using AQMod.Common.WorldGeneration;
using AQMod.Items.Placeable.Furniture;
using AQMod.Items.Tools.Map;
using AQMod.Tiles.TileEntities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AQMod.Tiles.Furniture
{
    public class Globe : ModTile
    {
        public static bool GenGlobeTemple(int x, int y)
        {
            if (!AQWorldGen.ActiveAndSolid(x, y) && AQWorldGen.ActiveAndSolid(x, y + 1) && Main.tile[x, y].wall == WallID.None)
            {
                Main.tile[x, y + 1].type = TileID.GrayBrick;
                Main.tile[x, y + 1].halfBrick(halfBrick: false);
                Main.tile[x, y + 1].slope(slope: 0);
                Framing.GetTileSafely(x + 1, y + 1).active(active: true);
                Main.tile[x + 1, y + 1].type = TileID.GrayBrick;
                Main.tile[x + 1, y + 1].halfBrick(halfBrick: false);
                Main.tile[x + 1, y + 1].slope(slope: 0);
                Framing.GetTileSafely(x - 1, y + 1).active(active: true);
                Main.tile[x - 1, y + 1].type = TileID.GrayBrick;
                Main.tile[x - 1, y + 1].halfBrick(halfBrick: false);
                Main.tile[x - 1, y + 1].slope(slope: 0);
                Framing.GetTileSafely(x + 2, y + 1).active(active: true);
                Main.tile[x + 2, y + 1].type = TileID.GrayBrick;
                Main.tile[x + 2, y + 1].halfBrick(halfBrick: false);
                Main.tile[x + 2, y + 1].slope(slope: 0);
                Framing.GetTileSafely(x - 2, y + 1).active(active: true);
                Main.tile[x - 2, y + 1].type = TileID.GrayBrick;
                Main.tile[x - 2, y + 1].halfBrick(halfBrick: false);
                Main.tile[x - 2, y + 1].slope(slope: 0);
                int height = WorldGen.genRand.Next(3) + 5;
                for (int i = 0; i < height; i++)
                {
                    Framing.GetTileSafely(x + 1, y - i).active(active: false);
                    Main.tile[x + 1, y - i].wall = WallID.Stone;
                    Framing.GetTileSafely(x, y - i).active(active: false);
                    Main.tile[x, y - i].wall = WallID.Stone;
                    Framing.GetTileSafely(x - 1, y - i).active(active: false);
                    Main.tile[x - 1, y - i].wall = WallID.Stone;
                    Framing.GetTileSafely(x - 2, y - i).active(active: true);
                    Main.tile[x - 2, y - i].type = TileID.WoodenBeam;
                    Framing.GetTileSafely(x + 2, y - i).active(active: true);
                    Main.tile[x + 2, y - i].type = TileID.WoodenBeam;
                }
                for (int i = 0; i < 5; i++)
                {
                    Framing.GetTileSafely(x - 2 + i, y + 2).active(active: true);
                    Main.tile[x - 2 + i, y + 2].type = TileID.GrayBrick;
                    Framing.GetTileSafely(x - 2 + i, y - height).active(active: true);
                    Main.tile[x - 2 + i, y - height].type = TileID.GrayBrick;
                }
                for (int i = 0; i < 3; i++)
                {
                    Framing.GetTileSafely(x - 1 + i, y - height - 1).active(active: true);
                    Main.tile[x - 1 + i, y - height - 1].type = TileID.GrayBrick;
                }
                WorldGen.PlaceTile(x, y, TileID.Tables);
                WorldGen.PlaceTile(x + WorldGen.genRand.Next(2), y - 2, ModContent.TileType<Globe>());
                return true;
            }
            return false;
        }

        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.AnchorInvalidTiles = new[] { (int)TileID.MagicalIceBlock, };
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<TEGlobe>().Hook_AfterPlacement, -1, 0, true);
            TileObjectData.addTile(Type);
            dustType = DustID.Stone;
            disableSmartCursor = true;
            AddMapEntry(new Color(180, 180, 180), Lang.GetItemName(ModContent.ItemType<GlobeItem>()));
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 32, 32, ModContent.ItemType<GlobeItem>());
            int index = ModContent.GetInstance<TEGlobe>().Find(i, j);
            if (index == -1)
            {
                var g = (TEGlobe)TileEntity.ByID[index];
                foreach (var text in g.LegacyMarkers)
                {
                    switch (text)
                    {
                        case "CosmicMarker":
                            Item.NewItem(i * 16, j * 16, 32, 32, ModContent.ItemType<CosmicTelescope>());
                            break;
                        case "DungeonMarker":
                            Item.NewItem(i * 16, j * 16, 32, 32, ModContent.ItemType<DungeonMap>());
                            break;
                        case "LihzahrdMarker":
                            Item.NewItem(i * 16, j * 16, 32, 32, ModContent.ItemType<LihzahrdMap>());
                            break;
                        case "RetroMarker":
                            Item.NewItem(i * 16, j * 16, 32, 32, ModContent.ItemType<RetroGoggles>());
                            break;
                    }
                }
                return;
            }
            ModContent.GetInstance<TEGlobe>().Kill(i, j);
        }
    }
}