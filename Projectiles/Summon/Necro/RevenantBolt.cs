﻿using Aequus.Buffs.Necro;
using Aequus.Content;
using Aequus.Graphics;
using Aequus.Particles;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Summon.Necro
{
    public class RevenantBolt : ZombieBolt
    {
        public override string Texture => AequusHelpers.GetPath<ZombieBolt>();

        public override float Tier => 2f;

        public override void SetStaticDefaults()
        {
            this.SetTrail(10);
            PushableEntities.AddProj(Type);
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.scale = 0.8f;
            Projectile.alpha = 10;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
            Projectile.alpha = 250;
            Projectile.scale = 1.1f;
            Projectile.DamageType = NecromancyDamageClass.Instance;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(100, 222, 255, 255 - Projectile.alpha);
        }

        public override void AI()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                var center = Projectile.Center;
                foreach (var v in AequusHelpers.CircularVector(3, Main.GlobalTimeWrappedHourly * 5f))
                {
                    if (Main.rand.NextBool(3))
                        EffectsSystem.ParticlesBehindProjs.Add(new BloomParticle(center + v * 6f, Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * -0.125f, new Color(110, 160, 255, 100), Color.Blue.UseA(0) * 0.1f, 1.1f, 0.35f, Main.rand.NextFloat(MathHelper.TwoPi)));
                }
            }

            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 10;
                if (Projectile.alpha < 0)
                {
                    Projectile.alpha = 0;
                }
            }

            int target = Projectile.FindTargetWithLineOfSight(400f);
            if (target != -1)
            {
                float speed = Projectile.velocity.Length();
                Projectile.velocity = Vector2.Normalize(Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.npc[target].Center) * speed, 0.125f)) * speed;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            NecromancyDebuff.ReduceDamageForDebuffApplication<RevenantDebuff>(Tier, target, ref damage);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
            NecromancyDebuff.ApplyDebuff<RevenantDebuff>(target, 600, Projectile.owner);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            primColor = new Color(40, 100, 255, 100) * Projectile.Opacity;
            primScale = 6f;

            var texture = TextureAssets.Projectile[Type].Value;
            var frame = Projectile.Frame();
            var origin = frame.Size() / 2f;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, frame, new Color(10, 40, 255, 100) * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(lightColor) * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale * 0.5f, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            var center = Projectile.Center;
            for (int i = 0; i < 4; i++)
            {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.MagicMirror, newColor: new Color(255, 255, 255, 0));
                d.velocity *= 0.2f;
                d.velocity += (d.position - center) / 8f;
                d.scale += Main.rand.NextFloat(-0.5f, 0f);
                d.fadeIn = d.scale + Main.rand.NextFloat(0.2f, 0.5f);
            }
            for (int i = 0; i < 12; i++)
            {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), newColor: new Color(222, 222, 255, 150));
                d.velocity *= 0.2f;
                d.velocity += (d.position - center) / 8f;
            }
        }
    }
}