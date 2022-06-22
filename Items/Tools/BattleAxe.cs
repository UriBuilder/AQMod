﻿using Aequus.Projectiles.Melee.Swords;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Aequus.Items.Tools
{
    public class BattleAxe : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToDopeSword<BattleAxeProj>(32);
            Item.useTime /= 2;
            Item.SetWeaponValues(24, 5f, 20);
            Item.width = 30;
            Item.height = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.axe = 15;
            Item.tileBoost = 1;
            Item.value = Item.sellPrice(gold: 1);
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;
            Item.scale = 1.1f;
            Item.reuseDelay = 4;
        }

        public override int ChoosePrefix(UnifiedRandom rand)
        {
            return AequusHelpers.RollSwordPrefix(Item, rand);
        }

        public override bool CanShoot(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] <= 0;
        }
    }
}