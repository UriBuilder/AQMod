﻿using Aequus.Common;
using Aequus.Common.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Vanity.Masks;

[AutoloadEquip(EquipType.Head)]
[AutoloadGlowMask("_Glow", "_Head_Glow")]
public class OmegaStariteMask : ModItem {
    public override void SetDefaults() {
        Item.DefaultToHeadgear(16, 16, Item.headSlot);
        Item.rare = ItemDefaults.RarityBossMasks;
        Item.vanity = true;
    }

    public override Color? GetAlpha(Color lightColor) {
        return Color.Lerp(Color.White, lightColor, 0.5f);
    }

    public override void DrawArmorColor(Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor) {
        color = Color.Lerp(Color.White, color, 0.5f);
        glowMask = GlowMasksHandler.GetID(AequusTextures.OmegaStariteMask_Head_Glow.FullPath);
        glowMaskColor = (Color.White with { A = 0 }) * (1f - shadow);
    }
}