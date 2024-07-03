﻿namespace Aequu2.Core.Hooks;

public partial class TerrariaHooks {
    /// <summary>Allows the Grand Reward to disable money drops when equipped by the closest player.</summary>
    private static void NPC_NPCLoot_DropMoney(On_NPC.orig_NPCLoot_DropMoney orig, NPC self, Player closestPlayer) {
#if !DEBUG
        // Prevent money drops if the Grand Reward's downside is in effect.
        if (closestPlayer.TryGetModPlayer<Aequu2Player>(out var Aequu2Player) && Aequu2Player.accGrandRewardDownside) {
            return;
        }
#endif

        orig(self, closestPlayer);
    }
}
