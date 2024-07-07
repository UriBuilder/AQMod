﻿using AequusRemake.Core.Entities.Projectiles;
using AequusRemake.DataSets;

namespace AequusRemake.Content.Items.Accessories.EventPrevention;

public class EventDeactivatorPlayer : ModPlayer {
    public bool accDisableBloodMoon;
    public bool accDisableEclipse;
    public bool accDisableGlimmer;
    public bool accDisableFrostMoon;
    public bool accDisablePumpkinMoon;

    public override void PreUpdate() {
        CheckFlagOverrides();
    }

    public override bool PreItemCheck() {
        UndoPlayerFlagOverrides();
        return true;
    }

    public override void PostItemCheck() {
        CheckFlagOverrides();
    }

    public override void PostUpdate() {
        UndoPlayerFlagOverrides();
    }

    private void CheckFlagOverrides() {
        if (accDisableBloodMoon) {
            Commons.Refs.BloodMoon.OverrideWith(false);
        }
        if (accDisableEclipse) {
            Commons.Refs.Eclipse.OverrideWith(false);
        }
        if (accDisablePumpkinMoon) {
            Commons.Refs.PumpkinMoon.OverrideWith(false);
        }
        if (accDisableFrostMoon) {
            Commons.Refs.FrostMoon.OverrideWith(false);
        }
    }

    public static void CheckPlayerFlagOverrides(Player player) {
        if (player.TryGetModPlayer(out EventDeactivatorPlayer eventDeactivator)) {
            eventDeactivator.CheckFlagOverrides();
        }
    }

    public static void UndoPlayerFlagOverrides() {
        Commons.Refs.BloodMoon.RestoreOriginalValue();
        Commons.Refs.Eclipse.RestoreOriginalValue();
        Commons.Refs.PumpkinMoon.RestoreOriginalValue();
        Commons.Refs.FrostMoon.RestoreOriginalValue();
    }

    public override void ResetEffects() {
        accDisableBloodMoon = false;
        accDisableGlimmer = false;
        accDisableEclipse = false;
        accDisablePumpkinMoon = false;
        accDisableFrostMoon = false;
    }

    public override void PostUpdateEquips() {
        if (accDisableBloodMoon) {
            foreach (int npc in NPCDataSet.FromBloodMoon) {
                Player.npcTypeNoAggro[npc] = true;
            }
        }
        if (accDisableGlimmer) {
            foreach (int npc in NPCDataSet.FromGlimmer) {
                Player.npcTypeNoAggro[npc] = true;
            }
        }
        if (accDisableEclipse) {
            foreach (int npc in NPCDataSet.FromEclipse) {
                Player.npcTypeNoAggro[npc] = true;
            }
        }
    }

    public override bool CanHitNPC(NPC target) {
        return (!accDisableBloodMoon || !NPCDataSet.FromBloodMoon.Contains(target.netID))
            && (!accDisableGlimmer || !NPCDataSet.FromGlimmer.Contains(target.netID))
            && (!accDisableEclipse || !NPCDataSet.FromEclipse.Contains(target.netID));
    }

    public override bool CanBeHitByProjectile(Projectile proj) {
        return proj.TryGetGlobalProjectile(out ProjectileSource source) && source.HasNPCOwner
            ? CanHitNPC(Main.npc[source.parentNPCIndex])
            : true;
    }
}
