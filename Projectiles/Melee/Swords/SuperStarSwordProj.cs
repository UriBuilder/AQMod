﻿using Aequus.Buffs;
using Aequus.Buffs.Debuffs;
using Aequus.Common.Net.Sounds;
using Aequus.Projectiles.Base;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Melee.Swords {
    public class SuperStarSwordProj : HeldSlashingSwordProjectile
    {
        private bool _spawnedProjectile;

        public static Color[] DustColors => new Color[] { new Color(10, 60, 255, 0), new Color(10, 255, 255, 0), new Color(100, 180, 255, 0), };

        public override string Texture => AequusTextures.SuperStarSword.Path;

        public override bool ShouldUpdatePlayerDirection() {
            return AnimProgress > 0.1f && AnimProgress < 0.3f;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 66;
            Projectile.height = 66;
            Projectile.noEnchantmentVisuals = true;
            swordHeight = 50;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override bool? CanDamage()
        {
            return (AnimProgress > 0.05f && AnimProgress < 0.5f) ? null : false;
        }

        public override void AI()
        {
            base.AI();
            if (Main.player[Projectile.owner].itemAnimation <= 1)
            {
                Main.player[Projectile.owner].Aequus().itemCombo = (ushort)(combo == 0 ? 64 : 0);
            }
            if (!playedSound && AnimProgress > 0.4f)
            {
                playedSound = true;
                SoundEngine.PlaySound(AequusSounds.swordSwoosh with { Pitch = 0.2f }, Projectile.Center);
            }

            if (AnimProgress < 0.3f)
            {
                var car = DustColors;
                int amt = !Aequus.HQ ? 1 : Main.rand.Next(2) + 1;
                for (int i = 0; i < amt; i++)
                {
                    var velocity = AngleVector.RotatedBy(MathHelper.PiOver2 * -swingDirection) * Main.rand.NextFloat(2f, 8f);
                    var d = Dust.NewDustPerfect(Main.player[Projectile.owner].Center + AngleVector * Main.rand.NextFloat(10f, 70f * Projectile.scale), DustID.SilverFlame, velocity, newColor: Helper.LerpBetween(car, Main.rand.NextFloat(3f)).UseA(0));
                    d.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                    d.scale *= Projectile.scale;
                    d.fadeIn = d.scale + 0.1f;
                    d.noGravity = true;
                    if (Projectile.numUpdates == -1)
                    {
                        AequusPlayer.SpawnEnchantmentDusts(Main.player[Projectile.owner].Center + AngleVector * Main.rand.NextFloat(10f, 70f * Projectile.scale), velocity, Main.player[Projectile.owner]);
                    }
                }
            }
        }

        public override void UpdateSwing(float progress, float interpolatedSwingProgress)
        {
            if (interpolatedSwingProgress >= 0.5f && !_spawnedProjectile)
            {
                if (Main.player[Projectile.owner].Aequus().LifeRatio > 0.9f && Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Main.player[Projectile.owner].GetSource_HeldItem(), Main.player[Projectile.owner].Center + BaseAngleVector * 30f,
                        BaseAngleVector * Projectile.velocity.Length() * 9f,
                        ModContent.ProjectileType<SuperStarSwordSlash>(), (int)(Projectile.damage * 0.75f), Projectile.knockBack / 4f, Projectile.owner);
                }
                _spawnedProjectile = true;
            }
        }

        public override Vector2 GetOffsetVector(float progress)
        {
            return BaseAngleVector.RotatedBy((progress * (MathHelper.Pi * 1.25f) - MathHelper.PiOver2 * 1.25f) * -swingDirection);
        }

        public override float SwingProgress(float progress)
        {
            return Math.Max((float)Math.Sqrt(Math.Sqrt(Math.Sqrt(SwingProgressAequus(progress)))), MathHelper.Lerp(progress, 1f, progress));
        }

        public override float GetScale(float progress)
        {
            float scale = base.GetScale(progress);
            if (progress > 0.1f && progress < 0.9f)
            {
                return scale + 0.5f * (float)Math.Pow(Math.Sin((progress - 0.1f) / 0.9f * MathHelper.Pi), 2f);
            }
            return scale;
        }

        public override float GetVisualOuter(float progress, float swingProgress)
        {
            if (progress > 0.6f)
            {
                float p = 1f - (1f - progress) / 0.4f;
                Projectile.alpha = (int)(p * 255);
                return -8f * p;
            }
            return 0f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            AequusBuff.ApplyBuff<BlueFire>(target, 240, out bool canPlaySound);
            if (canPlaySound)
            {
                ModContent.GetInstance<BlueFireDebuffSound>().Play(target.Center);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var center = Main.player[Projectile.owner].Center;
            var drawColor = Projectile.GetAlpha(lightColor) * Projectile.Opacity;

            GetSwordDrawInfo(out var texture, out var handPosition, out var frame, out var rotationOffset, out var origin, out var effects);
            DrawSword(texture, handPosition, frame, drawColor, rotationOffset, origin, effects);

            float size = texture.Size().Length();
            if (AnimProgress < 0.4f) {
                float swishProgress = AnimProgress / 0.4f;
                float intensity = (float)Math.Sin((float)Math.Pow(swishProgress, 2f) * MathHelper.Pi);

                var swish = AequusTextures.Swish.Value;
                var swishOrigin = swish.Size() / 2f;
                var swishColor = new Color(10, 30, 100, 10) * intensity * intensity * Projectile.Opacity;
                float r = BaseAngleVector.ToRotation() + (swishProgress * 2f - 1f) * -swingDirection * 0.4f;
                var swishLocation = Main.player[Projectile.owner].Center - Main.screenPosition;
                Main.EntitySpriteDraw(swish, swishLocation + r.ToRotationVector2() * (size - 40f + 30f * swishProgress) * baseSwordScale, null, swishColor, r + MathHelper.PiOver2, swishOrigin, 1f, effects, 0);
                Main.EntitySpriteDraw(swish, swishLocation + r.ToRotationVector2() * (size - 55f + 30f * swishProgress) * baseSwordScale, null, swishColor * 0.4f, r + MathHelper.PiOver2, swishOrigin, new Vector2(1.2f, 1.75f), effects, 0);
            }
            return false;
        }
    }
}
