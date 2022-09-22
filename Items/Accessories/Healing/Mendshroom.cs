﻿using Aequus.Buffs;
using Aequus.Items.Accessories.Summon.Sentry;
using Aequus.Projectiles.Misc;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Healing
{
    public class Mendshroom : ModItem, ItemHooks.IUpdateItemDye
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;

            SentryAccessoriesDatabase.OnAI.Add(Type, SentryAccessoriesDatabase.ApplyEquipFunctional_AI);
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.rare = ItemDefaults.RarityCrabCrevice;
            Item.value = ItemDefaults.CrabCreviceValue;
            Item.buffType = ModContent.BuffType<MendshroomBuff>();
            Item.shoot = ModContent.ProjectileType<MendshroomProj>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var aequus = player.Aequus();
            aequus.accMendshroom = Item;
            aequus.mendshroomDiameter += 280f;
            aequus.mendshroomRegen += 30;
        }

        public void UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem)
        {
            player.Aequus().cMendshroom = dyeItem.dye;
        }
    }
}