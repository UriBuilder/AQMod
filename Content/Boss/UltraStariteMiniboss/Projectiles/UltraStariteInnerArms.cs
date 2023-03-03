﻿using Aequus.Projectiles.Monster;
using Microsoft.Xna.Framework;
using Terraria;

namespace Aequus.Content.Boss.UltraStariteMiniboss.Projectiles
{
    public class UltraStariteInnerArms : EnemyAttachedProjBase
    {
        public override string Texture => Aequus.BlankTexture;

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.hide = true;
            Projectile.penetrate = -1;
        }

        protected override bool CheckAttachmentConditions(NPC npc)
        {
            return (int)npc.ai[0] != -1 && npc.ModNPC is UltraStarite;
        }

        protected override void AIAttached(NPC npc)
        {
            Projectile.position += (npc.rotation + MathHelper.TwoPi / 5f * Projectile.ai[1] - MathHelper.PiOver2 + MathHelper.TwoPi / 10f).ToRotationVector2() * (npc.height * npc.scale + npc.ai[3] + 50f);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (target.townNPC || target.life < 5)
                damage = (int)(damage * 0.1f);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            Main.npc[AttachedNPC].ModNPC.OnHitPlayer(target, damage, crit); // janky magic :trollface:
        }
    }
}