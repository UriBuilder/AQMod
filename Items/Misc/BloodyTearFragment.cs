﻿using Aequus.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc
{
    public class BloodyTearFragment : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityMaterials[Type] = ItemSortingPriority.Materials.DemoniteOre;
            SacrificeTotal = 25;
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 2);
        }

        public override void AddRecipes()
        {
            Recipe.Create(ItemID.BloodMoonStarter)
                .AddIngredient(Type, 4)
                .AddTile(TileID.Anvils)
                .TryRegisterAfter(ItemID.SuspiciousLookingEye);
            Recipe.Create(ItemID.BloodMoonMonolith)
                .AddIngredient(Type, 4)
                .AddTile(TileID.Anvils)
                .TryRegisterAfter(ItemID.VoidMonolith);
            Recipe.Create(ItemID.BloodyMachete)
                .AddIngredient(Type, 12)
                .AddTile(TileID.Anvils)
                .Register();
            Recipe.Create(ItemID.BloodRainBow)
                .AddIngredient(Type, 12)
                .AddTile(TileID.Anvils)
                .Register();
            Recipe.Create(ItemID.VampireFrogStaff)
                .AddIngredient(Type, 12)
                .AddTile(TileID.Anvils)
                .Register();
            Recipe.Create(ItemID.BloodFishingRod)
                .AddIngredient(Type, 12)
                .AddTile(TileID.Anvils)
                .Register();
            Recipe.Create(ItemID.BloodHamaxe)
                .AddIngredient(Type, 12)
                .AddIngredient(ItemID.SoulofLight, 8)
                .AddTile(TileID.Anvils)
                .Register();
            Recipe.Create(ItemID.SharpTears)
                .AddIngredient(Type, 12)
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}