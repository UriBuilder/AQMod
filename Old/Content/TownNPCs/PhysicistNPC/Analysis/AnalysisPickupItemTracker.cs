﻿namespace Aequu2.Old.Content.TownNPCs.PhysicistNPC.Analysis;

public class AnalysisPickupItemTracker : GlobalItem {
    public override bool OnPickup(Item item, Player player) {
        if (item.value > 0 && !item.IsACoin && !item.questItem && !item.vanity) {
            int rare = item.OriginalRarity;
            if (!AnalysisSystem.IgnoreRarities.Contains(rare)) {
                if (Main.netMode == NetmodeID.MultiplayerClient) {
                    ExtendedMod.GetPacket<AnalysisItemPickupPacket>().Send(player, item);
                }
                else {
                    AnalysisSystem.HandleItemPickup(player, item);
                }

                //Main.NewText($"{item.value} | {rare}:{Aequu2Text.GetRarityNameValue(rare)}");
                //Main.NewText($"{AnalysisSystem.quest.itemValue} | {AnalysisSystem.quest.itemRarity}:{Aequu2Text.GetRarityNameValue(AnalysisSystem.quest.itemRarity)} | {AnalysisSystem.quest.itemValue}", Main.DiscoColor);
                //Main.NewText($"{AnalysisSystem.RareTracker[rare].highestValueObtained}", Main.DiscoColor);
            }
        }
        return true;
    }
}