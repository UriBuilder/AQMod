﻿using Aequus.Common.Tiles;

namespace Aequus.Content.Biomes.PollutedOcean.Tiles.PolymerSands;

[LegacyName("SedimentaryRockTile", "SedimentaryRock")]
public class PolymerSandstone : MultiMergeTile {
    public override void Load() {
        Mod.AddContent(new InstancedTileItem(this).WithRecipe((item) => {
            item.CreateRecipe()
                .AddIngredient(PolymerSand.Item)
                .AddIngredient(ItemID.StoneBlock)
                .AddTile(TileID.Solidifier)
                .Register();
        }));
    }

    public override void SetStaticDefaults() {
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;
        AddMerge(TileID.Sand);
        AddMerge(TileID.HardenedSand);

        TileID.Sets.ChecksForMerge[Type] = true;
        TileID.Sets.Conversion.Sandstone[Type] = true;
        AddMapEntry(new(160, 149, 97));
        DustType = DustID.Sand;
        HitSound = SoundID.Tink;
        MineResist = 1.5f;

        PollutedOceanSystem.BiomeTiles.Add(Type);
    }

    public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
}