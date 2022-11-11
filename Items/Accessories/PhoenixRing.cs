﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories
{
    public class PhoenixRing : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory(20, 14);
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 2, silver: 50);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var aequus = player.Aequus();
            aequus.accBoneRing++;
            aequus.accBoneBurningRing++;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BoneRing>()
                .AddIngredient(ItemID.MagmaStone)
                .AddIngredient(ItemID.Bone, 30)
                .AddTile(TileID.TinkerersWorkbench)
                .TryRegisterBefore(ItemID.NecroHelmet);
        }
    }
}