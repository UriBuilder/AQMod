﻿using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace Aequus.Content.CrossMod.SplitSupport.Photography.Envelopes;
public class EnvelopeGlimmer : BaseEnvelope {
    public override void ModifyItemLoot(ItemLoot itemLoot) {
        base.ModifyItemLoot(itemLoot);
        AddPreHardmodeBasic(itemLoot);
        itemLoot.Add(ItemDropRule.OneFromOptions(1, new int[] {
           ModContent.ItemType<Items.Weapons.Melee.SuperStarSword.SuperStarSword>(),
           ModContent.ItemType<Items.Weapons.Magic.Nightfall>(),
           ModContent.ItemType<Items.Weapons.Summon.Minion.StariteStaff>(),
           ModContent.ItemType<Items.Accessories.Combat.HyperCrystal>(),
        }));
        itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Misc.CelesitalEightBall>(), 4));
        itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Materials.Glimmer.StariteMaterial>(), minimumDropped: 3, maximumDropped: 10));
    }
}