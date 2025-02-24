﻿using Aequus;
using Aequus.Buffs.Debuffs;
using Aequus.Common.Buffs;
using Aequus.Common.DataSets;
using Aequus.Common.Graphics.Primitives;
using Aequus.Common.Items.DropRules;
using Aequus.Common.NPCs;
using Aequus.Common.Particles;
using Aequus.Common.Utilities;
using Aequus.Content.CursorDyes.Items;
using Aequus.Content.Events.GlimmerEvent;
using Aequus.Items.Materials.Glimmer;
using Aequus.Items.Potions.NeutronYogurt;
using Aequus.NPCs.Monsters.Glimmer.UltraStarite.Projectiles;
using Aequus.Particles;
using Aequus.Particles.Dusts;
using Aequus.Tiles.Furniture.Boss.Relics;
using Aequus.Tiles.Furniture.Boss.Trophies;
using Aequus.Tiles.Monoliths.CosmicMonolith;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.Utilities;

namespace Aequus.NPCs.Monsters.Glimmer.UltraStarite;
[AutoloadBossHead]
public class UltraStarite : ModNPC {
    public const float BossProgression = 4.61f;

    public const int STATE_DEATHRAY_TRANSITION_END = 5;
    public const int STATE_CHASE = 4;
    public const int STATE_DEATHRAY = 3;
    public const int STATE_SPINNY = 2;
    public const int STATE_FLYUP = 1;
    public const int STATE_IDLE = 0;
    public const int STATE_GOODBYE = -1;
    public const int STATE_DEAD = -2;

    public int State { get => (int)NPC.ai[0]; set => NPC.ai[0] = value; }
    public float ArmsLength { get => NPC.ai[3]; set => NPC.ai[3] = value; }

    public float[] oldArmsLength;
    public TrailRenderer armTrail;
    public TrailRenderer armTrailSmoke;

    public override void SetStaticDefaults() {
        Main.npcFrameCount[Type] = 3;
        NPCID.Sets.TrailingMode[Type] = 7;
        NPCID.Sets.TrailCacheLength[Type] = 15;
        ItemID.Sets.KillsToBanner[BannerItem] = 10;
        AequusBuff.SetImmune(Type, Starite.DefaultBuffImmunities());
        NPCID.Sets.NPCBestiaryDrawOffset[Type] = new() {
            Scale = 0.6f,
            Position = new(1f, 0f)
        };
        NPCID.Sets.DontDoHardmodeScaling[Type] = true;
        NPCID.Sets.CantTakeLunchMoney[Type] = true;
        NPCSets.ElitePrefixBlacklist.Add(Type);
        SnowgraveCorpse.NPCBlacklist.Add(Type);
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot) {
        this.CreateLoot(npcLoot)
            .AddRelic<UltraStariteRelic>()
            .Add(new GuaranteedFlawlesslyRule(ModContent.ItemType<UltraStariteTrophy>(), 10))
            .Add<StariteMaterial>(chance: 1, stack: (8, 15))
            .Add<CosmicMonolith>(chance: 4, stack: 1)
            .Add<ManaCursor>(chance: 4, stack: 1)
            .Add(ItemID.Megaphone, chance: 50, stack: 1)
            .Add<NeutronYogurt>(chance: 1, stack: (1, 2));
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        this.CreateEntry(database, bestiaryEntry)
            .QuickUnlock();
    }

    public override void SetDefaults() {
        NPC.width = 120;
        NPC.height = 120;
        NPC.lifeMax = 1500;
        NPC.damage = 50;
        NPC.defense = 20;
        NPC.HitSound = SoundID.NPCHit5;
        NPC.DeathSound = SoundID.NPCDeath55;
        NPC.aiStyle = -1;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.knockBackResist = 0f;
        NPC.value = Item.buyPrice(silver: 50);
        NPC.npcSlots = 3f;

        this.SetBiome<GlimmerZone>();

        oldArmsLength = new float[NPCID.Sets.TrailCacheLength[Type]];
    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: balance -> balance (bossAdjustment is different, see the docs for details) */
    {
        NPC.lifeMax = (int)(NPC.lifeMax * 0.85f * numPlayers);
        NPC.damage = (int)(NPC.damage * 0.9f);
    }

    public override void HitEffect(NPC.HitInfo hit) {
        float x = NPC.velocity.X.Abs() * hit.HitDirection;
        if (NPC.life <= 0) {
            if (Main.netMode != NetmodeID.Server && State == STATE_DEAD) {
                ScreenShake.SetShake(35, multiplier: 0.9f, where: NPC.Center);
                ScreenFlash.Flash.Set(NPC.Center, 0.2f);
            }
            for (int i = 0; i < 50; i++) {
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.MagicMirror);
                Main.dust[d].velocity.X += x;
                Main.dust[d].velocity.Y = -Main.rand.NextFloat(2f, 6f);
            }
            for (int i = 0; i < 70; i++) {
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 57 + Main.rand.Next(2));
                Main.dust[d].velocity.X += x;
                Main.dust[d].velocity.Y = -Main.rand.NextFloat(2f, 6f);
            }
            for (int i = 0; i < 16; i++) {
                Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center, new Vector2(Main.rand.NextFloat(-5f, 5f) + x, Main.rand.NextFloat(-5f, 5f)), 16 + Main.rand.Next(2));
            }
        }
        else {
            for (int i = 0; i < 7; i++) {
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.MagicMirror);
                Main.dust[d].velocity.X += x;
                Main.dust[d].velocity.Y = -Main.rand.NextFloat(5f, 12f);
            }
            int d1 = Dust.NewDust(NPC.position, NPC.width, NPC.height, 57 + Main.rand.Next(2));
            Main.dust[d1].velocity.X += x;
            Main.dust[d1].velocity.Y = -Main.rand.NextFloat(2f, 6f);
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(Main.rand.NextFloat(-4f, 4f) + x * 0.75f, Main.rand.NextFloat(-4f, 4f)), 16 + Main.rand.Next(2));
        }
    }

    private bool PlayerCheck() {
        NPC.TargetClosest(faceTarget: false);
        if (!NPC.HasValidTarget || Main.player[NPC.target].dead || NPC.Distance(Main.player[NPC.target]) > 2000f) {
            NPC.ai[0] = -1f;
            return false;
        }
        else {
            return true;
        }
    }

    public override void AI() {
        //if (AequusHelpers.debugKey)
        //{
        //    if (State != STATE_DEAD)
        //    {
        //        NPC.localAI[1] = NPC.ai[0];
        //        NPC.localAI[2] = Main.GlobalTimeWrappedHourly;
        //        State = STATE_DEAD;
        //        NPC.ai[2] = 0f;
        //        NPC.velocity *= 0.5f;
        //        NPC.dontTakeDamage = true;
        //        NPC.life = NPC.lifeMax;
        //    }
        //}
        if (State == STATE_DEAD) {
            if (NPC.localAI[0] == 0) {
                NPC.localAI[0] = Main.rand.Next(100);
            }
            NPC.velocity *= 0.98f;
            NPC.rotation += 0.06f * (1f + NPC.ai[2] / 60f);
            if (NPC.ai[2] > 0f)
                NPC.ai[2] = 0f;
            NPC.ai[2] -= 1f - NPC.ai[2] / 120f;
            for (int i = 0; i < Main.rand.Next(2, 14); i++) {
                var d = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Unit() * NPC.ai[2] * Main.rand.NextFloat(0.2f, 1f) * 5f, ModContent.DustType<MonoDust>(), newColor: Color.Lerp(new Color(255, 20, 100), new Color(255, 150, 250), Math.Min(Main.rand.NextFloat(1f) - NPC.ai[2] / 60f, 1f)).UseA(0));
                d.velocity *= 0.2f;
                d.velocity += (NPC.Center - d.position) / 8f;
                d.scale = Main.rand.NextFloat(0.3f, 2f);
                d.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            }
            if (NPC.ai[2] < -60f) {
                for (int i = 0; i < 200; i++) {
                    var d = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Unit() * 80f * Main.rand.NextFloat(0.01f, 1f), ModContent.DustType<MonoDust>(), newColor: Color.Lerp(new Color(255, 20, 100), new Color(255, 150, 250), Main.rand.NextFloat(1f)).UseA(0));
                    d.velocity *= 0.2f;
                    d.velocity += (d.position - NPC.Center) / 2f;
                    d.scale = Main.rand.NextFloat(0.3f, 2.5f);
                    d.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                }
                NPC.life = -33333;
                NPC.HitEffect();
                NPC.checkDead();
            }
            return;
        }

        if (Main.dayTime && !Main.remixWorld) {
            NPC.Aequus().noOnKill = true;
        }

        if (Main.rand.NextBool(8)) {
            var d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Enchanted_Pink);
            d.velocity = (d.position - NPC.Center) / 8f;
        }
        if (Main.rand.NextBool(10)) {
            var g = Gore.NewGoreDirect(NPC.GetSource_FromThis(), NPC.position + new Vector2(Main.rand.Next(NPC.width - 4), Main.rand.Next(NPC.height - 4)), new Vector2(Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f)), 16);
            g.velocity = (g.position - NPC.Center) / 8f;
            g.scale *= 0.6f;
        }
        Lighting.AddLight(NPC.Center, new Vector3(1.2f, 1.2f, 0.5f));
        Vector2 center = NPC.Center;
        if (NPC.ai[0] == -1f) {
            NPC.noTileCollide = true;
            NPC.velocity.X *= 0.95f;
            if (NPC.velocity.Y > 0f)
                NPC.velocity.Y *= 0.96f;
            NPC.velocity.Y -= 0.075f;

            NPC.timeLeft = Math.Min(NPC.timeLeft, 100);
            NPC.rotation += NPC.velocity.Length() * 0.0157f;
            return;
        }

        Player player = Main.player[NPC.target];
        Vector2 plrCenter = player.Center;
        oldArmsLength[0] = NPC.ai[3];
        Helper.UpdateCacheList(oldArmsLength);
        switch (State) {
            case STATE_IDLE: {
                    NPC.TargetClosest(faceTarget: false);
                    if (NPC.HasValidTarget) {
                        if (Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height) || NPC.life < NPC.lifeMax) {
                            NPC.ai[0] = STATE_FLYUP;
                            NPC.ai[1] = 0f;
                            for (int i = 0; i < 5; i++) {
                                int damage = Main.expertMode ? 45 : 75;
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), center, new Vector2(0f, 0f), ModContent.ProjectileType<UltraStariteOuterArms>(), damage, 1f, Main.myPlayer, NPC.whoAmI + 1, i);
                            }
                            NPC.netUpdate = true;
                        }
                        else {
                            NPC.ai[1]++;
                            if (NPC.ai[1] >= 1200f) {
                                NPC.timeLeft = 0;
                                NPC.ai[0] = -1f;
                            }
                            NPC.velocity *= 0.96f;
                            return;
                        }
                    }
                    else {
                        if (Main.player[NPC.target].dead) {
                            NPC.ai[0] = -1f;
                            NPC.ai[1] = -0f;
                            NPC.netUpdate = true;
                        }
                        NPC.ai[1]++;
                        if (NPC.ai[1] >= 1200f) {
                            NPC.timeLeft = 0;
                            NPC.ai[0] = -1f;
                            NPC.netUpdate = true;
                        }
                        NPC.velocity *= 0.96f;
                        return;
                    }
                }
                break;

            case STATE_FLYUP: {
                    NPC.ai[1]++;
                    NPC.velocity.Y -= 0.45f;
                    if (NPC.ai[1] > 20f && PlayerCheck()) {
                        State = 2;
                        NPC.ai[1] = 0f;
                    }
                }
                break;

            case STATE_SPINNY: {
                    NPC.velocity *= 0.9f;
                    NPC.ai[1]++;
                    if (NPC.ai[1] >= 90f) {
                        if ((int)NPC.ai[1] == 90f)
                            WrapRotations();
                        float multiplier = 1f;
                        if (NPC.ai[1] > 100f)
                            multiplier = 1f - (NPC.ai[1] - 100f) / 40f;
                        NPC.rotation += MathHelper.TwoPi / 25f * multiplier;
                        if (NPC.ai[1] > 140f) {
                            NPC.ai[0] = 3f;
                            NPC.ai[1] = 0f;
                            ArmsLength = 0f;
                        }
                        else {
                            float x = (NPC.ai[1] - 90f) / 50f * MathHelper.TwoPi;
                            ArmsLength = (float)Math.Sin(x) * 175f;
                            if (ArmsLength < 0f)
                                ArmsLength *= 0.1f;
                        }
                    }
                    else {
                        NPC.rotation += 0.02f;
                    }
                }
                break;

            case STATE_DEATHRAY: {
                    NPC.velocity *= 0.9f;
                    if (NPC.ai[1] == 0f) {
                        int armID = 0;
                        float closestArm = float.MaxValue;
                        WrapRotations();
                        for (int i = 0; i < 5; i++) {
                            var comparisonPoint = center + GetArmRotation(i).ToRotationVector2() * 180f;
                            float d = Vector2.Distance(comparisonPoint, plrCenter);
                            if (d < closestArm) {
                                closestArm = d;
                                armID = i;
                            }
                        }
                        NPC.rotation += MathHelper.TwoPi / 5f * armID;
                    }
                    NPC.ai[1]++;
                    if (NPC.ai[1] > 500f) {
                        State = 5;
                        NPC.ai[1] = 0f;
                    }
                    if (NPC.ai[1] < 200f) {
                        var comparisonPoint = center + GetArmRotation(0).ToRotationVector2() * 180f;
                        NPC.ai[2] = Math.Sign(comparisonPoint.X - plrCenter.X);
                        NPC.rotation = NPC.rotation.AngleLerp(NPC.DirectionTo(plrCenter).ToRotation(), (1f - NPC.ai[1] / 200f) * 0.05f);
                    }
                    if ((int)NPC.ai[1] == 197 && Helper.ShouldDoEffects(NPC.Center)) {
                        ScreenFlash.Flash.Set(NPC.Center, 0.8f, 0.93f);
                        ScreenShake.SetShake(50f, 0.95f, NPC.Center);
                    }
                    if ((int)NPC.ai[1] == 200) {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, ModContent.ProjectileType<UltraStariteDeathray>(), 40, 1f, Main.myPlayer, NPC.whoAmI + 1f);
                        SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen.WithPitch(0.5f), NPC.Center);
                    }
                    NPC.rotation += (Main.expertMode ? 0.02f : 0.009f) * NPC.ai[2];
                }
                break;

            case STATE_CHASE: {
                    if (!PlayerCheck()) {
                        return;
                    }
                    NPC.ai[1]++;
                    if (NPC.ai[1] < 50f) {
                        NPC.velocity *= 0.96f;
                    }
                    else {
                        float wantedDistance = 250f;
                        var difference = Main.player[NPC.target].Center - NPC.Center;
                        if (difference.Length() > wantedDistance) {
                            NPC.velocity = Vector2.Lerp(NPC.velocity, difference / 90f, 0.05f);
                        }
                        else {
                            NPC.velocity *= 0.98f;
                            NPC.ai[1] += 4;
                        }
                    }
                    if (NPC.ai[1] > 300f) {
                        State = STATE_SPINNY;
                        NPC.ai[1] = 0f;
                    }
                    NPC.rotation += 0.01f + NPC.velocity.Length() * 0.01f;
                }
                break;

            case STATE_DEATHRAY_TRANSITION_END: {
                    NPC.velocity *= 0.8f;
                    NPC.ai[1]++;
                    if (NPC.ai[1] > 90f) {
                        State = STATE_CHASE;
                        NPC.ai[1] = 0f;
                    }
                }
                break;
        }
        if (NPC.velocity.Length() < 1.5f) {
            if (center.Y + 400f < plrCenter.Y)
                NPC.velocity.Y += 1f;
            else if (center.Y + 300f > plrCenter.Y && Collision.SolidCollision(NPC.position, NPC.width, NPC.height + 200))
                NPC.velocity.Y -= 1f;
        }
    }
    public void WrapRotations() {
        NPC.rotation %= MathHelper.TwoPi / 5f;
        float diff = NPC.oldRot[0] - NPC.oldRot[0] % (MathHelper.TwoPi / 5f);
        for (int i = 0; i < NPC.oldRot.Length; i++) {
            NPC.oldRot[i] -= diff;
        }
    }
    public float GetArmRotation(int i) {
        return NPC.rotation + MathHelper.TwoPi / 5f * i - MathHelper.PiOver2;
    }

    public override void UpdateLifeRegen(ref int damage) {
        if (Main.dayTime && State != STATE_DEAD && !Main.remixWorld) {
            NPC.lifeRegen = -2500;
            damage = 50;
        }
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo) {
        if (Main.rand.NextBool(Main.expertMode ? 1 : 2))
            target.AddBuff(ModContent.BuffType<BlueFire>(), 240);
        if (Main.rand.NextBool(Main.expertMode ? 1 : 4))
            target.AddBuff(BuffID.Blackout, 600);
        if (Main.rand.NextBool(Main.expertMode ? 4 : 12))
            target.AddBuff(BuffID.Silenced, 120);
    }

    public override bool CheckDead() {
        if (State == STATE_DEAD)
            return true;
        NPC.localAI[1] = NPC.ai[0];
        NPC.localAI[2] = Main.GlobalTimeWrappedHourly;
        State = STATE_DEAD;
        NPC.ai[2] = 0f;
        NPC.velocity *= 0.5f;
        NPC.dontTakeDamage = true;
        NPC.life = NPC.lifeMax;
        return false;
    }

    public override void OnKill() {
        Helper.DropHearts(new EntitySource_Loot(NPC), NPC.Hitbox, 4, 4);
        AequusWorld.MarkAsDefeated(ref AequusWorld.downedUltraStarite, Type);
        AequusWorld.MarkAsDefeated(ref AequusWorld.downedEventCosmic, Type);
    }

    public override int SpawnNPC(int tileX, int tileY) {
        return NPC.NewNPC(null, tileX * 16 + 8, tileY * 16 - 80, NPC.type);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        float innerArmsRotation = Main.GlobalTimeWrappedHourly * 6f;

        armTrail ??= new TrailRenderer(TrailTextures.Trail[2].Value, TrailRenderer.DefaultPass, (p) => new Vector2(60f), (p) => Color.BlueViolet.UseA(0) * 1.25f * (float)Math.Pow(1f - p, 2f));

        armTrailSmoke ??= new ForceCoordTrailRenderer(TrailTextures.Trail[3].Value, TrailRenderer.DefaultPass, (p) => new Vector2(50f), (p) => Color.Blue.UseA(0) * (1f - p) * 0.8f) {
            coord1 = 0f,
            coord2 = 1f
        };
        var texture = TextureAssets.Npc[Type].Value;
        var origin = NPC.frame.Size() / 2f;
        var offset = new Vector2(NPC.width / 2f, NPC.height / 2f);
        float mult = 1f / NPCID.Sets.TrailCacheLength[NPC.type];
        var armFrame = NPC.frame;
        var coreFrame = new Rectangle(NPC.frame.X, NPC.frame.Y + NPC.frame.Height * 2, NPC.frame.Width, NPC.frame.Height);
        var bloom = AequusTextures.Bloom0;
        var bloomFrame = new Rectangle(0, 0, bloom.Width(), bloom.Height());
        var bloomOrigin = bloomFrame.Size() / 2f;

        int state = State;

        bool dying = State == STATE_DEAD;
        float deathScaleShake = 0f;
        float deathScaleBloom = 1f;
        if (dying) {
            innerArmsRotation = NPC.localAI[2];
            state = (int)NPC.localAI[1];
            deathScaleShake = NPC.ai[2] / 60f;
            deathScaleBloom = (float)Math.Min(NPC.scale * (-NPC.ai[2] / 60f), 1f) * 3f;
            ScreenShake.SetShake((deathScaleShake + 2f) * 4f, where: NPC.Center);
        }
        var armLength = 256f * NPC.scale;
        if (NPC.IsABestiaryIconDummy) {
            armLength -= 30f * NPC.scale;
        }

        Main.spriteBatch.Draw(bloom, new Vector2((int)(NPC.position.X + offset.X - screenPos.X), (int)(NPC.position.Y + offset.Y - screenPos.Y)), bloomFrame, new Color(200, 0, 255, 0), 0f, bloomOrigin, NPC.scale * 2, SpriteEffects.None, 0f);
        if (!NPC.IsABestiaryIconDummy && !dying && State == STATE_SPINNY) {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin_World(shader: true);
            int trailLength = NPCID.Sets.TrailCacheLength[Type];
            int armTrailLength = trailLength;
            if (NPC.ai[1] < 90f) {
                armTrailLength = 0;
            }
            else if (NPC.ai[1] > 120f) {
                armTrailLength = (int)(armTrailLength * (1f - (NPC.ai[1] - 120f) / 20f));
            }
            var armPositions = new List<Vector2>[5];

            for (int j = 0; j < 5; j++)
                armPositions[j] = new List<Vector2>();

            for (int i = 0; i < trailLength; i++) {
                var pos = NPC.oldPos[i] + offset - screenPos;
                float progress = Helper.CalcProgress(trailLength, i);
                Color color = new Color(30, 25, 140, 4) * (mult * (NPCID.Sets.TrailCacheLength[NPC.type] - i)) * 0.6f;
                if (i >= armTrailLength || i > 1 && (NPC.oldRot[i] - NPC.oldRot[i - 1]).Abs() < 0.05f)
                    continue;
                for (int j = 0; j < 5; j++) {
                    float rotation = NPC.oldRot[i] + MathHelper.TwoPi / 5f * j;
                    var armPos = NPC.position + offset + (rotation - MathHelper.PiOver2).ToRotationVector2() * (armLength + oldArmsLength[i]) - screenPos;
                    armPositions[j].Add(armPos + screenPos);
                    if (Aequus.GameWorldActive && !NPC.IsABestiaryIconDummy && NPC.ai[1] < 125f && Main.rand.NextBool(2 + i * 15)) {
                        float scale = Main.rand.NextFloat(0.4f, 1.5f);
                        ParticleSystem.New<MonoBloomParticle>(ParticleLayer.AboveDust).Setup(armPos + Main.screenPosition + Main.rand.NextVector2Unit() * 30f,
                        ((armPos - (NPC.Center - Main.screenPosition)).ToRotation() - MathHelper.PiOver2 + Main.rand.NextFloat(-0.4f, 0.4f)).ToRotationVector2() * Main.rand.NextFloat(2f, 8f),
                        Color.White.UseA(40) * scale, Color.BlueViolet.UseA(0) * 0.3f * scale, Main.rand.NextFloat(0.9f, 1.5f) * scale, Main.rand.NextFloat(0.1f, 0.4f), Main.rand.NextFloat(MathHelper.TwoPi));
                    }
                }
            }
            for (int j = 0; j < 5; j++) {
                var arr = armPositions[j].ToArray();
                armTrail.Draw(arr);
                armTrailSmoke.Draw(arr, uvAdd: -Main.GlobalTimeWrappedHourly);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin_World(shader: false); ;
        }
        var armSegmentFrame = new Rectangle(NPC.frame.X, NPC.frame.Y + NPC.frame.Height, NPC.frame.Width, NPC.frame.Height);

        float segmentLength = 100f * NPC.scale;
        float armScale = NPC.scale;
        if (NPC.IsABestiaryIconDummy) {
            segmentLength -= 15f * NPC.scale;
            innerArmsRotation = 0f;
            armLength -= 100f * NPC.scale;
            NPC.rotation = MathHelper.TwoPi / 10f;
            armScale *= 0.9f;
        }
        if (ArmsLength < 0f) {
            segmentLength += ArmsLength;
        }
        float armsPullIn = 1f;
        if (state == STATE_DEATHRAY) {
            float progress = Math.Min(NPC.ai[1] / 300f, 1f);
            float waveFunction = (float)Math.Sin((1f - progress) * MathHelper.Pi * 1.5f - MathHelper.Pi);
            if (waveFunction < 0f) {
                waveFunction *= 0.2f;
            }
            armsPullIn = Math.Clamp(waveFunction, 0.05f, 1.1f);
        }
        else if (state == STATE_DEATHRAY_TRANSITION_END) {
            float progress = Math.Clamp((NPC.ai[1] - 10f) / 75f, 0f, 1f);
            armsPullIn = Math.Clamp(progress, 0.05f, 1.1f);
        }
        if (dying) {
            armsPullIn = MathHelper.Lerp(armsPullIn, 1f, NPC.ai[2] / -60f);
        }
        for (int i = -2; i < 3; i++) {
            int armID = i;
            if (armID >= 0) {
                armID++;
            }
            if (armID == 3) {
                armID = 0;
            }
            float lengthMultiplier = 1f;
            lengthMultiplier += (1f - armsPullIn) * 0.1f;
            float rotation = NPC.rotation + MathHelper.TwoPi / 5f * armID * armsPullIn;
            if (dying) {
                rotation += Main.rand.NextFloat(-0.76f, 0.76f) * Main.rand.NextFloat(NPC.ai[2] / 240f);
            }

            if (armsPullIn <= 0.6f) {
                float lerpValue = 1f - (armsPullIn - 0.05f) / 0.55f;

                float time = Main.GlobalTimeWrappedHourly + MathHelper.TwoPi / 5f * armID;
                time *= 2f;
                time %= MathHelper.TwoPi;
                float waveFunction = (float)Math.Sin(time);

                lengthMultiplier = MathHelper.Lerp(lengthMultiplier, lengthMultiplier - 1f * (1f - ((float)Math.Sin(time * 0.5f + MathHelper.Pi)).Abs() * 0.2f) + 1f, lerpValue);
                rotation = MathHelper.Lerp(rotation, NPC.rotation + waveFunction * 0.275f, lerpValue);
            }
            else {
                lengthMultiplier -= Math.Abs(armID) * 0.05f * (1f - armsPullIn);
            }

            var n = (rotation - MathHelper.PiOver2).ToRotationVector2();
            var armPos = NPC.position + offset + n * ((armLength + NPC.ai[3]) * lengthMultiplier) - screenPos;
            if (dying) {
                armPos += new Vector2(Main.rand.NextFloat(NPC.ai[2] / 8f), Main.rand.NextFloat(NPC.ai[2] / 8f));
                armPos += n * NPC.ai[2] * 2f;
            }
            Main.spriteBatch.Draw(texture, armPos.Floor(), armFrame, Color.White, rotation, origin, armScale, SpriteEffects.None, 0f);

            rotation = innerArmsRotation + MathHelper.TwoPi / 5f * armID;
            armPos = NPC.position + offset + (rotation - MathHelper.PiOver2).ToRotationVector2() * segmentLength - screenPos;
            if (dying)
                rotation += Main.rand.NextFloat(-0.1f, 0.1f) * Main.rand.NextFloat(NPC.ai[2] / 60f);
            if (dying)
                armPos += new Vector2(Main.rand.NextFloat(NPC.ai[2] / 4f), Main.rand.NextFloat(NPC.ai[2] / 4f));
            Main.spriteBatch.Draw(texture, armPos.Floor(), armSegmentFrame, Color.White, rotation, origin, NPC.scale * 0.75f, SpriteEffects.None, 0f);
        }

        var drawCoords = (NPC.position + offset - screenPos).Floor();
        float bloomProgress = 0f;
        if (State == STATE_DEATHRAY) {
            bloomProgress = Math.Min(NPC.ai[1] / 200f, 1f);
            if (NPC.ai[1] > 400f) {
                bloomProgress -= (NPC.ai[1] - 400f) / 100f * 0.25f;
            }
        }
        if (State == STATE_DEATHRAY_TRANSITION_END) {
            bloomProgress = Math.Min((1f - NPC.ai[1] / 90f) * 0.75f, 1f);
        }
        bloomProgress *= bloomProgress;
        if (dying)
            bloomProgress += deathScaleBloom;

        var ray = ModContent.Request<Texture2D>(Aequus.AssetsPath + "LightRay", AssetRequestMode.ImmediateLoad);
        if (ray.IsLoaded) {
            float hpRatio = (1f - MathF.Pow(NPC.life / (float)NPC.lifeMax, 2f)) * 1.5f;
            Main.rand.NextBool(3);
            float rayProgress = Math.Max(bloomProgress, 0.4f);
            var rayOrigin = ray.Size() / 2f;
            for (int i = 0; i < 40; i++) {
                var rand = new FastRandom("Split".GetHashCode() + NPC.whoAmI + i * 612897);
                float rotation = rand.Float(MathHelper.TwoPi) + Main.GlobalTimeWrappedHourly * rand.Float(0.4f, 2f) * rand.Bool().ToDirectionInt();
                float wave = (float)Math.Sin(rand.Float(MathHelper.TwoPi) + Main.GlobalTimeWrappedHourly * rand.Float(0.02f, 1f));
                var scale = new Vector2(rand.Float(0.8f, 1.1f) * wave, NPC.scale * rayProgress * rand.Float(1f, 6f) * wave) * 0.5f * hpRatio;
                Main.spriteBatch.Draw(
                    ray.Value,
                    drawCoords,
                    null,
                    new Color(200, 233, 255, 0) * rayProgress,
                    rotation,
                    rayOrigin,
                    scale, SpriteEffects.None, 0f);

                scale.X *= 1.5f;
                Main.spriteBatch.Draw(
                    ray.Value,
                    drawCoords,
                    null,
                    new Color(120, 20, 255, 0) * rayProgress,
                    rotation,
                    rayOrigin,
                    scale * rand.Float(1.5f, 2f), SpriteEffects.None, 0f);
            }
        }

        Main.spriteBatch.Draw(texture, drawCoords, coreFrame, new Color(255, 255, 255, 100), 0f, origin, NPC.scale, SpriteEffects.None, 0f);

        if (bloomProgress > 0f) {
            if (dying)
                bloomProgress -= deathScaleBloom * 0.5f;
            bloom = AequusTextures.Bloom3;

            Main.spriteBatch.Draw(bloom, drawCoords, null, new Color(255, 233, 200, 0) * bloomProgress, 0f, bloom.Size() / 2f, NPC.scale * bloomProgress * 1.1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(bloom, drawCoords, null, new Color(255, 120, 20, 0) * bloomProgress, 0f, bloom.Size() / 2f, NPC.scale * bloomProgress * 1.35f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(bloom, drawCoords, null, new Color(255, 20, 200, 0) * 0.5f * bloomProgress, 0f, bloom.Size() / 2f, NPC.scale * bloomProgress * 2.2f, SpriteEffects.None, 0f);
        }
        //var toPlayer = NPC.DirectionTo(Main.player[NPC.target].Center);

        //AequusHelpers.DrawLine(NPC.Center - screenPos, NPC.Center + toPlayer * 100f - screenPos, 4f, Color.Red);

        //for (int i = 1; i < 5; i++)
        //    AequusHelpers.DrawLine(NPC.Center - screenPos, NPC.Center + (NPC.rotation + MathHelper.TwoPi / 5f * i).ToRotationVector2() * 100f - screenPos, 4f, Color.Blue);

        //AequusHelpers.DrawLine(NPC.Center - screenPos, NPC.Center + NPC.rotation.ToRotationVector2() * 100f - screenPos, 4f, Color.Red);
        //AequusHelpers.DrawLine(NPC.Center - screenPos, NPC.Center + (NPC.rotation).ToRotationVector2() * 100f - screenPos, 4f, Color.Lime);
        return false;
    }
}