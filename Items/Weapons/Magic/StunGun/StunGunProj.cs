﻿using Aequus.Common.Graphics;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Magic.StunGun;

public class StunGunProj : ModProjectile {
    public override string Texture => AequusTextures.Extra(ExtrasID.RainbowRodTrailShape);

    public override void SetStaticDefaults() {
        ProjectileID.Sets.TrailCacheLength[Type] = 100;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults() {
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.extraUpdates = 40;
        Projectile.penetrate = -1;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 2;
        Projectile.friendly = true;
        Projectile.timeLeft = 400;
        Projectile.aiStyle = -1;
    }

    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
        width = 2;
        height = 2;
        return true;
    }

    private void OnHitAnythingAtAll() {
        Projectile.friendly = false;
        Projectile.velocity = Vector2.Zero;
        Projectile.tileCollide = false;
    }

    public override void AI() {
        if (Projectile.numUpdates == -1) {
            Projectile.Opacity = 1f - (Projectile.ai[2] - 50f) / 350f;
            Projectile.friendly = false;
            Projectile.velocity = Vector2.Zero;
            Projectile.extraUpdates = 10;
        }

        if (!Projectile.tileCollide || Projectile.velocity == Vector2.Zero) {
            return;
        }

        if (Collision.WetCollision(Projectile.position, Projectile.width, Projectile.height)) {
            OnHitAnythingAtAll();
        }
        if (Projectile.ai[0] == 0f || Projectile.ai[1] == 0f) {
            Projectile.ai[0] = Projectile.velocity.X;
            Projectile.ai[1] = Projectile.velocity.Y;
        }
        Projectile.ai[2]++;
        var velocity = new Vector2(Projectile.ai[0], Projectile.ai[1]);
        if (Projectile.ai[2] > 6f) {
            Projectile.velocity = velocity.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f));
        }
        if (Projectile.friendly) {
            int target = Projectile.FindTargetWithLineOfSight(100f);
            if (target != -1) {
                velocity = Vector2.Lerp(velocity, Projectile.DirectionTo(Main.npc[target].Center) * 16f, 0.3f);
                Projectile.ai[0] = velocity.X;
                Projectile.ai[1] = velocity.Y;
            }
        }
        Projectile.ai[0] *= 0.98f;
        Projectile.ai[1] *= 0.98f;
        if (Projectile.friendly && Main.rand.NextBool(Math.Max(Projectile.MaxUpdates / 15, 1))) {
            var d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Square(-2f, 2f), DustID.Electric, Scale: 0.75f);
            d.velocity *= Main.rand.NextFloat(0.1f, 0.2f);
            d.velocity += Projectile.velocity * Main.rand.NextFloat(0f, 0.2f);
            d.noGravity = true;
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity) {
        OnHitAnythingAtAll();
        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        OnHitAnythingAtAll();
        target.AddBuff(ModContent.BuffType<StunGunDebuff>(), 300);
        Projectile.Center = target.Center;
    }

    public override bool PreDraw(ref Color lightColor) {
        float opacity = Projectile.Opacity;
        AequusDrawing.DrawBasicVertexLine(TextureAssets.MagicPixel.Value, Projectile.oldPos, Projectile.oldRot, 
            (p) => Color.Cyan with { A = 0 } * opacity * p,
            (p) => MathF.Sin(p * MathHelper.Pi) * (1f + 3f * opacity),
            -Main.screenPosition + Projectile.Size / 2f
        );
        return false;
    }
}