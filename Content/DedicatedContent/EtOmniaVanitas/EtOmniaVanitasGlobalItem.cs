﻿using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.DedicatedContent.EtOmniaVanitas;

public class EtOmniaVanitasGlobalItem : GlobalItem {
    public override void ModifyItemLoot(Item item, ItemLoot itemLoot) {
        if (!ItemID.Sets.BossBag[item.type]) {
            return;
        }

        itemLoot.Add(ItemDropRule.Common(EtOmniaVanitasLoader.Tier1.Type, EtOmniaVanitas.DropChance));
    }
}