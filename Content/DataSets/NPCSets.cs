﻿using Aequus.Common.NPCs.Bestiary;
using Aequus.Core.DataSets;
using Newtonsoft.Json;
using System.Collections.Generic;
using Terraria.GameContent.Bestiary;

namespace Aequus.Content.DataSets;

public partial class NPCSets : DataSet {
    [JsonProperty]
    public static HashSet<NPCEntry> OccultistIgnore { get; private set; } = new();

    [JsonProperty]
    public static HashSet<NPCEntry> IsCorrupt { get; private set; } = new();
    [JsonProperty]
    public static HashSet<NPCEntry> IsCrimson { get; private set; } = new();
    [JsonProperty]
    public static HashSet<NPCEntry> IsHallow { get; private set; } = new();
    [JsonProperty]
    public static HashSet<NPCEntry> FromPillarEvent { get; private set; } = new();

    [JsonProperty]
    public static Dictionary<NPCEntry, bool> NameTagOverride { get; private set; } = new();

    [JsonProperty]
    public static HashSet<NPCEntry> StunnableByTypeId { get; private set; } = new();
    [JsonProperty]
    public static HashSet<NPCAIEntry> StunnableByAI { get; private set; } = new();

    /// <summary>
    /// Used for Royal Gel's Crown of Blood combination.
    /// </summary>
    [JsonProperty]
    public static HashSet<NPCEntry> FriendablePreHardmodeSlime { get; private set; } = new();

    /// <summary>
    /// Used for Volatile Gelatin's Crown of Blood combination.
    /// </summary>
    [JsonProperty]
    public static HashSet<NPCEntry> FriendableHardmodeSlime { get; private set; } = new();

    /// <summary>
    /// Enemies in this set cannot become friendly. This usually contains bosses, or other special NPCs like worm segments.
    /// </summary>
    [JsonProperty]
    public static HashSet<NPCEntry> Unfriendable { get; private set; } = new();

    /// <summary>
    /// NPCs in this set deal 'heat' contact damage. This damage can be resisted using the Frost Potion.
    /// </summary>
    [JsonProperty]
    public static HashSet<NPCEntry> DealsHeatDamage { get; private set; } = new();

    /// <summary>
    /// NPCs in this set cannot become Elites. This usually contains bosses.
    /// </summary>
    [JsonProperty]
    public static HashSet<NPCEntry> PrefixBlacklist { get; private set; } = new();

    [JsonProperty]
    public static HashSet<NPCEntry> PushableByTypeId { get; private set; } = new();
    [JsonProperty]
    public static HashSet<NPCAIEntry> PushableByAI { get; private set; } = new();

    #region Bestiary
    [JsonIgnore]
    public static List<IBestiaryInfoElement> CorruptionTags { get; private set; } = new();
    [JsonIgnore]
    public static List<IBestiaryInfoElement> CrimsonTags { get; private set; } = new();
    [JsonIgnore]
    public static List<IBestiaryInfoElement> HallowTags { get; private set; } = new();
    [JsonIgnore]
    public static List<IBestiaryInfoElement> PillarTags { get; private set; } = new();
    [JsonIgnore]
    public static List<IBestiaryInfoElement> UnderworldTags { get; private set; } = new();

    private void LoadBestiaryElementTypes() {
        CorruptionTags.AddRange(new IBestiaryInfoElement[] {
            BestiaryBuilder.Corruption,
            BestiaryBuilder.UndergroundCorruption,
            BestiaryBuilder.CorruptionDesert,
            BestiaryBuilder.CorruptionUndergroundDesert,
            BestiaryBuilder.CorruptionIce,
        });
        CrimsonTags.AddRange(new IBestiaryInfoElement[] {
            BestiaryBuilder.Crimson,
            BestiaryBuilder.UndergroundCrimson,
            BestiaryBuilder.CrimsonDesert,
            BestiaryBuilder.CrimsonUndergroundDesert,
            BestiaryBuilder.CrimsonIce,
        });
        HallowTags.AddRange(new IBestiaryInfoElement[] {
            BestiaryBuilder.Hallow,
            BestiaryBuilder.UndergroundHallow,
            BestiaryBuilder.HallowDesert,
            BestiaryBuilder.HallowUndergroundDesert,
            BestiaryBuilder.HallowIce,
        });
        PillarTags.AddRange(new IBestiaryInfoElement[] {
            BestiaryBuilder.SolarPillar,
            BestiaryBuilder.VortexPillar,
            BestiaryBuilder.NebulaPillar,
            BestiaryBuilder.StardustPillar,
        });
        UnderworldTags.AddRange(new IBestiaryInfoElement[] {
            BestiaryBuilder.Underworld,
        });
    }

    private bool ContainsBestiaryTags(List<IBestiaryInfoElement> tags, List<IBestiaryInfoElement> searchTags) {
        return tags != null && tags.Exists(t => t != null && searchTags.Exists(searchTag => t.GetType() == searchTag.GetType()));
    }
    #endregion

    #region Loading
    public override void SetStaticDefaults() {
        LoadBestiaryElementTypes();

        // Make all of these NPCs immune to the vanilla "Slow" debuff.
        NPCID.Sets.SpecificDebuffImmunity[NPCID.HallowBoss][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.CultistBoss][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.BloodEelBody][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.BloodEelTail][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.BoneSerpentBody][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.BoneSerpentTail][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.CultistDragonBody1][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.CultistDragonBody2][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.CultistDragonBody3][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.CultistDragonBody4][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.CultistDragonTail][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.DevourerBody][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.DevourerTail][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.DiggerBody][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.DiggerTail][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.DuneSplicerBody][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.DuneSplicerTail][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.EaterofWorldsBody][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.EaterofWorldsTail][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.GiantWormBody][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.GiantWormTail][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.LeechBody][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.LeechTail][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.SeekerBody][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.SeekerTail][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.SolarCrawltipedeBody][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.SolarCrawltipedeTail][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.StardustWormBody][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.StardustWormTail][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.TombCrawlerBody][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.TombCrawlerTail][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.WyvernBody][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.WyvernBody2][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.WyvernBody3][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.WyvernLegs][BuffID.Slow] = true;
        NPCID.Sets.SpecificDebuffImmunity[NPCID.WyvernTail][BuffID.Slow] = true;
    }

    public override void AddRecipes() {
        for (int i = NPCID.NegativeIDCount + 1; i < NPCLoader.NPCCount; i++) {
            var bestiaryEntry = Main.BestiaryDB.FindEntryByNPCID(i);
            if (bestiaryEntry == null || bestiaryEntry.Info == null) {
                continue;
            }

            var info = bestiaryEntry.Info;
            if (ContainsBestiaryTags(info, CorruptionTags)) {
                IsCorrupt.Add((NPCEntry)i);
            }
            if (ContainsBestiaryTags(info, CrimsonTags)) {
                IsCrimson.Add((NPCEntry)i);
            }
            if (ContainsBestiaryTags(info, HallowTags)) {
                IsHallow.Add((NPCEntry)i);
            }
            if (ContainsBestiaryTags(info, PillarTags)) {
                FromPillarEvent.Add((NPCEntry)i);
            }
            if (ContainsBestiaryTags(info, UnderworldTags)) {
                OccultistIgnore.Add((NPCEntry)i);
            }
        }
    }
    #endregion

    #region Methods
    public bool IsUnholy(int npcId) {
        return IsCorrupt.Contains(npcId) || IsCrimson.Contains(npcId);
    }
    public bool IsHoly(int npcId) {
        return IsHallow.Contains(npcId);
    }
    #endregion
}