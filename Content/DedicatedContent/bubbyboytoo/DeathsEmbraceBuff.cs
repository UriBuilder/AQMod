﻿using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Content.DedicatedContent.bubbyboytoo;

public class DeathsEmbraceBuff : ModBuff {
    public override void Update(Player player, ref int buffIndex) {
        player.GetDamage(DamageClass.Generic) += 0.15f;
        player.GetCritChance(DamageClass.Generic) += 0.15f;
        if (player.buffTime[buffIndex] < 2 && Main.myPlayer == player.whoAmI) {
            player.KillMe(new PlayerDeathReason() { SourceCustomReason = Language.GetTextValue("Mods.Aequus.Player.DeathMessage.DeathsEmbrace", player.name), },
                player.statLife, 0);
        }
    }

    public override bool RightClick(int buffIndex) {
        return false;
    }
}