﻿using Aequus;
using Aequus.Common.Items;
using Aequus.Items.Misc.Dyes;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;

namespace Aequus.Content.Items.Materials.OmniGem;

public class OmniGem : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 25;
        ItemID.Sets.SortingPriorityMaterials[Type] = ItemSortingPriority.Materials.Amber;
    }

    public override void SetDefaults() {
        Item.DefaultToPlaceableTile(ModContent.TileType<OmniGemTile>());
        Item.rare = ItemRarityID.Green;
        Item.value = Item.sellPrice(silver: 2);
    }

    public override Color? GetAlpha(Color lightColor) {
        return Color.White;
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        var texture = TextureAssets.Item[Type].Value;

        Main.spriteBatch.End();
        spriteBatch.Begin_UI(immediate: true, useScissorRectangle: true);

        var drawData = new DrawData(texture, position, frame, itemColor.A > 0 ? itemColor : Main.inventoryBack, 0f, origin, scale, SpriteEffects.None, 0);
        var maskTexture = AequusTextures.OmniGem_Mask.Value;
        var maskFrame = maskTexture.Frame(verticalFrames: 3, frameY: 2);

        var effect = GameShaders.Armor.GetShaderFromItemId(ModContent.ItemType<HueshiftDye>());
        effect.Apply(null, drawData);

        var drawPosition = position + frame.Size() / 2f * scale - origin * scale;
        Main.spriteBatch.Draw(
            maskTexture,
            drawPosition,
            maskFrame.Frame(0, -1),
            Color.White with { A = 0, } * 0.2f,
            0f,
            maskFrame.Size() / 2f,
            scale, SpriteEffects.None, 0f);

        drawData.Draw(Main.spriteBatch);

        Main.spriteBatch.Draw(
            maskTexture,
            drawPosition,
            maskFrame,
            Color.White with { A = 0, } * 0.66f,
            0f,
            maskFrame.Size() / 2f,
            scale, SpriteEffects.None, 0f);

        Main.spriteBatch.End();
        spriteBatch.Begin_UI(immediate: false, useScissorRectangle: true);
        return false;
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
        var texture = TextureAssets.Item[Type].Value;

        Main.spriteBatch.End();
        Main.spriteBatch.Begin_World(shader: true);

        var frame = new Rectangle(0, 0, texture.Width, texture.Height);
        var drawPosition = new Vector2(
            Item.position.X - Main.screenPosition.X + frame.Width / 2 + Item.width / 2 - frame.Width / 2,
            Item.position.Y - Main.screenPosition.Y + frame.Height / 2 + Item.height - frame.Height
        );
        var origin = frame.Size() / 2f;
        var drawData = new DrawData(texture, drawPosition, frame, Item.GetAlpha(lightColor), rotation, origin, scale, SpriteEffects.None, 0);

        var effect = GameShaders.Armor.GetShaderFromItemId(ModContent.ItemType<HueshiftDye>());
        effect.Apply(null, drawData);

        var maskTexture = AequusTextures.OmniGem_Mask.Value;
        var maskFrame = maskTexture.Frame(verticalFrames: 3, frameY: 2);

        Main.spriteBatch.Draw(
            maskTexture,
            drawPosition,
            maskFrame.Frame(0, -1),
            Color.White with { A = 0, } * 0.2f,
            rotation,
            maskFrame.Size() / 2f,
            1f, SpriteEffects.None, 0f);

        drawData.Draw(Main.spriteBatch);

        Main.spriteBatch.Draw(
            maskTexture,
            drawPosition,
            maskFrame,
            Color.White with { A = 0, } * 0.66f,
            rotation,
            maskFrame.Size() / 2f,
            1f, SpriteEffects.None, 0f);

        Main.spriteBatch.End();
        Main.spriteBatch.Begin_World(shader: false);
        return false;
    }

    private void AddGemRecipes() {
        Recipe.Create(ItemID.RainbowTorch, 3)
            .AddIngredient(ItemID.Torch, 3)
            .AddIngredient(Type)
            .Register()
            .DisableDecraft();
        Recipe.Create(ItemID.Topaz, 5)
            .AddIngredient(ItemID.Amethyst, 5)
            .AddIngredient(Type)
            .AddTile(TileID.DemonAltar)
            .Register()
            .DisableDecraft();
        Recipe.Create(ItemID.Sapphire, 5)
            .AddIngredient(ItemID.Topaz, 5)
            .AddIngredient(Type)
            .AddTile(TileID.DemonAltar)
            .Register()
            .DisableDecraft();
        Recipe.Create(ItemID.Emerald, 5)
            .AddIngredient(ItemID.Sapphire, 5)
            .AddIngredient(Type)
            .AddTile(TileID.DemonAltar)
            .Register()
            .DisableDecraft();
        Recipe.Create(ItemID.Ruby, 5)
            .AddIngredient(ItemID.Emerald, 5)
            .AddIngredient(Type)
            .AddTile(TileID.DemonAltar)
            .Register()
            .DisableDecraft();
        Recipe.Create(ItemID.Diamond, 5)
            .AddIngredient(ItemID.Ruby, 5)
            .AddIngredient(Type)
            .AddTile(TileID.DemonAltar)
            .Register()
            .DisableDecraft();
    }
    private void AddEnchantedRecipes() {
        Recipe.Create(ItemID.EnchantedSword)
            .AddIngredient(ItemID.WoodenSword)
            .AddIngredient(Type, 8)
            .AddTile(TileID.Anvils)
            .TryRegisterAfter(ItemID.WoodenSword)
            .DisableDecraft();

        Recipe.Create(ItemID.Sundial)
            .AddIngredient(ItemID.SunplateBlock, 50)
            .AddIngredient(Type, 8)
            .AddTile(TileID.Anvils)
            .TryRegisterAfter(ItemID.SunplateBlock)
            .DisableDecraft();
    }

    public override void AddRecipes() {
        AddGemRecipes();
        AddEnchantedRecipes();
    }

    public static float GetGlobalTime(ulong seed) {
        return Main.GlobalTimeWrappedHourly * 2f + Utils.RandomFloat(ref seed) * 20f;
    }
    public static float GetGlobalTime(int i, int j) {
        return GetGlobalTime(Helper.TileSeed(i, j));
    }
}
