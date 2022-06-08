﻿using Aequus.Graphics;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Boss
{
    public abstract class AequusBoss : ModNPC
    {
        public const int ACTION_GOODBYE = -1;
        public const int ACTION_INIT = 0;
        public const int ACTION_INTRO = 1;

        public int Action { get => (int)NPC.ai[0]; set => NPC.ai[0] = value; }
        public float ActionTimer { get => NPC.ai[1]; set => NPC.ai[1] = value; }

        public override void SetStaticDefaults()
        {
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            SnowgraveCorpse.NPCBlacklist.Add(Type);
        }

        public void ClearAI()
        {
            NPC.ai[0] = 0f;
            NPC.ai[1] = 0f;
            NPC.ai[2] = 0f;
            NPC.ai[3] = 0f;
        }

        public void ClearLocalAI()
        {
            NPC.localAI[0] = 0f;
            NPC.localAI[1] = 0f;
            NPC.localAI[2] = 0f;
            NPC.localAI[3] = 0f;
        }

        public void SetCamera(string context = null, Vector2? position = null, CameraPriority priority = CameraPriority.BossDefeat, float speed = 12f, int hold = 60)
        {
            ModContent.GetInstance<GameCamera>().SetTarget(context ?? Name, position ?? NPC.Center, priority, speed, hold);
        }
        public void Flash(Vector2? position, float amt, float multiplier = 0.9f)
        {
            FlashScene.Flash.Set(position ?? NPC.Center, amt, multiplier);
        }
        public void Shake(float amt, float multiplier = 0.9f)
        {
            EffectsSystem.Shake.Set(amt, multiplier);
        }
    }
}