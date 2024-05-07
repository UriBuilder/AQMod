﻿using Aequus.Common;
using tModLoaderExtended.GlowMasks;

namespace Aequus.Old.Content.Weapons.Melee.UltimateSword;

[AutoloadGlowMask]
public class UltimateSword : ModItem {
    public override void SetDefaults() {
        Item.LazyCustomSwordDefaults<UltimateSwordProj>(24);
        Item.SetWeaponValues(50, 4.5f, 11);
        Item.width = 30;
        Item.height = 30;
        Item.scale = 1f;
        Item.rare = Commons.Rare.OmegaStariteLoot;
        Item.value = Commons.Cost.OmegaStariteLoot;
        Item.autoReuse = true;
    }

    public override bool? UseItem(Player player) {
        return null;
    }

    public override bool MeleePrefix() {
        return true;
    }

    public override void HoldItem(Player player) {
        player.AddBuff(ModContent.BuffType<UltimateSwordBuff>(), 1, quiet: true);
    }

    public override void AddRecipes() {
        //CreateRecipe()
        //    .AddIngredient<StariteMaterial>(20)
        //    .AddIngredient<CosmicEnergy>()
        //    .AddTile(TileID.Anvils)
        //    .TryRegisterAfter(ItemID.NightsEdge);
    }
}