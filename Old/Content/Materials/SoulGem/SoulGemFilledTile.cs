﻿using Terraria.GameContent;

namespace Aequus.Old.Content.Materials.SoulGem;

public class SoulGemFilledTile : SoulGemTile {
    public override string Texture => AequusTextures.SoulGemTile.Path;
    protected override int Item => ModContent.ItemType<SoulGemFilled>();
    protected override Color MapColor => new Color(40, 140, 180);

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
        r = 0.075f;
        g = 0.22f;
        b = 0.5f;
    }

    public override void PostDraw(int i, int j, SpriteBatch spriteBatch) {
        spriteBatch.Draw(
            TextureAssets.Tile[Type].Value,
            new Vector2(i * 16f, j * 16f) - Main.screenPosition + TileHelper.DrawOffset,
            new Rectangle(Main.tile[i, j].TileFrameX, Main.tile[i, j].TileFrameY, 16, 16),
            Color.White with { A = 0 } * Helper.Oscillate(Main.GlobalTimeWrappedHourly * 2.5f, 0.1f, 0.33f),
            0f,
            Vector2.Zero,
            1f, SpriteEffects.None, 0f);
    }
}