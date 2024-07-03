﻿namespace Aequu2.Old.Content.Items.Accessories.OnHitDebuffs;

[AutoloadEquip(EquipType.HandsOn)]
public class PhoenixRing : ModItem {
    public override void SetDefaults() {
        Item.DefaultToAccessory(20, 14);
        Item.rare = ItemRarityID.LightRed;
        Item.value = Item.sellPrice(gold: 3);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.GetModPlayer<Aequu2Player>().accBoneRing++;
        player.magmaStone = true;
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient<BoneRing>()
            .AddIngredient(ItemID.MagmaStone)
            .AddTile(TileID.TinkerersWorkbench)
            .Register()
            .SortAfterFirstRecipesOf(ItemID.FireGauntlet);
    }
}