﻿using Aequus.Common.DataSets;
using Aequus.Common.Preferences;
using Aequus.Items.Materials;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Content.Monsters.Mimics;

public class MimicsGlobalNPC : GlobalNPC, IPreExtractBestiaryItemDrops {
    public static readonly float IceMimicSpawnrate = 0.022f;
    public static readonly float ShadowMimicSpawnrate = 0.022f;

    public static readonly float MimicUGSpawnrate = 0.002f;
    public static readonly float MimicCavernsSpawnrate = 0.01f;

    public override void SetDefaults(NPC npc) {
        if ((npc.type != NPCID.Mimic && npc.type != NPCID.IceMimic) || Main.remixWorld || !GameplayConfig.Instance.EarlyMimics) {
            return;
        }

        npc.damage = 30;
        npc.defense = 12;
        npc.lifeMax = 300;
        npc.value = Item.buyPrice(gold: 2);
    }

    public override void OnSpawn(NPC npc, IEntitySource source) {
        if (source is not EntitySource_SpawnNPC) {
            return;
        }

        if (npc.type == NPCID.IceMimic) {
            if (Main.hardMode && GameplayConfig.Instance.AdamantiteMimics && !Main.rand.NextBool(3)) {
                npc.Transform(ModContent.NPCType<FrostMimic>());
            }
        }
        else if (npc.type == NPCID.Mimic) {
            if (GameplayConfig.Instance.ShadowMimics && NPC.downedBoss3 && npc.position.Y / 16f > Main.UnderworldLayer) {
                npc.Transform(ModContent.NPCType<ShadowMimic>());
            }
            if (Main.hardMode && GameplayConfig.Instance.AdamantiteMimics && !Main.rand.NextBool(3)) {
                if (npc.position.Y > Main.worldSurface * 16f && npc.position.Y < Main.UnderworldLayer * 16f) {
                    npc.Transform(ModContent.NPCType<AdamantiteMimic>());
                }
            }
        }
    }

    public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo) {
        if (!GameplayConfig.Instance.EarlyMimics || Main.hardMode || Main.remixWorld || spawnInfo.SpawnTileY < ((int)Main.worldSurface + 100)) {
            return;
        }

        Tile tile = Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY];
        if (GameplayConfig.Instance.DungeonMimics && spawnInfo.SpawnTileY > Main.worldSurface && Main.wallDungeon[tile.WallType]) {
            pool[ModContent.NPCType<DungeonChestMimic>()] = DungeonChestMimic.Spawnrate;
        }

        if (!spawnInfo.Allowed()) {
            return;
        }

        if (TileID.Sets.IcesSnow[tile.TileType]) {
            pool[NPCID.IceMimic] = IceMimicSpawnrate;
            return;
        }

        if (spawnInfo.SpawnTileY >= Main.UnderworldLayer) {
            // Prevent mimics spawning in the Underworld if Skeletron hasn't been defeated.
            if (GameplayConfig.Instance.ShadowMimics && NPC.downedBoss3) {
                pool[NPCID.Mimic] = ShadowMimicSpawnrate;
            }
        }
        else {
            pool[NPCID.Mimic] = spawnInfo.SpawnTileY >= Main.rockLayer ? MimicCavernsSpawnrate : MimicUGSpawnrate;
        }
    }

    void IPreExtractBestiaryItemDrops.PreExtractBestiaryItemDrops(BestiaryDatabase bestiaryDatabase, ItemDropDatabase database) {
        if (!GameplayConfig.Instance.EarlyMimics) {
            return;
        }

        MimicLootOverride(database);

        IceLootOverride(database);
    }

    static void MimicLootOverride(ItemDropDatabase database) {
        NPCLoot rules = GetCleanRules(NPCID.Mimic, database);

        LeadingConditionRule parent = new LeadingConditionRule(new Conditions.NotRemixSeed());

        // Primary Drops
        int[] chestItems = [.. ItemSets.GoldChestPrimaryLoot];
        rules.AddConditionRule(new Conditions.NotRemixSeed(), new OneFromOptionsDropRule(1, 1, chestItems));

        rules.Add(parent);
    }

    static void IceLootOverride(ItemDropDatabase database) {
        NPCLoot rules = GetCleanRules(NPCID.IceMimic, database);

        LeadingConditionRule parent = new LeadingConditionRule(new Conditions.NotRemixSeed());

        // Primary drops
        int[] chestItems = [.. ItemSets.IceChestPrimaryLoot];
        parent.OnSuccess(ItemDropRule.OneFromOptions(1, chestItems));

        // Secondary drops
        parent.OnSuccess(ItemDropRule.Common(ItemID.Fish, chanceDenominator: 10))
            .OnFailedRoll(ItemDropRule.Common(ModContent.ItemType<FrozenTechnology>(), chanceDenominator: 2, minimumDropped: 1, maximumDropped: 2));

        rules.Add(parent);
    }

    static NPCLoot GetCleanRules(int npcId, ItemDropDatabase database) {
        NPCLoot rules = Helper.GetNPCLoot(npcId, database);

        // Repair the original mimic drop rules only if the Remix Seed is active.
        LeadingConditionRule overrideRule = new LeadingConditionRule(new Conditions.RemixSeed());
        foreach (IItemDropRule rule in rules.Get(includeGlobalDrops: false)) {
            overrideRule.OnSuccess(rule);
        }
        rules.Clear();

        rules.Add(overrideRule);

        return rules;
    }
}
