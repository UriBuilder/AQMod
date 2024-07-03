﻿using Aequu2.DataSets;

namespace Aequu2.Old.Content.Items.Materials.SoulGem;

public class SoulGemGlobalNPC : GlobalNPC {
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation) {
        return !NPCDataSet.CannotGrantSoulGems.Contains(entity.type);
    }

    public override bool SpecialOnKill(NPC npc) {
        if (Main.netMode != NetmodeID.Server && npc.playerInteraction[Main.myPlayer]) {
            SoulGem.TryFillSoulGems(Main.LocalPlayer);
        }
        return false;
    }
}
