﻿using AQMod.Assets;
using AQMod.Assets.Textures;
using AQMod.Common.Utilities;
using AQMod.Items.Placeable.Banners;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace AQMod.NPCs.Monsters.AtmosphericEvent
{
    public class TemperatureBalloon : ModNPC, IDecideFallThroughPlatforms
    {
        public const int FramesX = 2;

        private bool _setupFrame; // no need to sync this since find frame stuff is client only (I think)

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 7;

            NPCID.Sets.TrailingMode[npc.type] = 7;
            NPCID.Sets.TrailCacheLength[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.width = 46;
            npc.height = 36;
            npc.lifeMax = 500;
            npc.damage = 45;
            npc.defense = 4;
            npc.HitSound = SoundID.DD2_GoblinHurt;
            npc.DeathSound = SoundID.NPCDeath56;
            npc.aiStyle = -1;
            npc.noGravity = true;
            npc.knockBackResist = 0.1f;
            npc.value = Item.buyPrice(silver: 50);
            npc.buffImmune[BuffID.OnFire] = true;
            banner = npc.type;
            bannerItem = ModContent.ItemType<TemperatureBalloonBanner>();
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f);
        }

        public override void AI() // ai[0] attack stuff, -1 means it's fleeing.
                                  // ai[1] is temperature (1 = hot, 2 = cold)
        {
            if (npc.ai[1] == 0f)
            {
                npc.ai[1] = Main.rand.Next(2) + 1f; // whether to be a hot or cold enemy
                npc.netUpdate = true;
            }
            bool hot = (int)npc.ai[1] == 1;
            if (!hot)
            {
                npc.defense = npc.defDefense * 2;
            }
            if ((int)npc.ai[0] == -1)
            {
                npc.velocity.X *= 0.97f;
                if (npc.velocity.Y < 0f)
                {
                    npc.velocity.Y *= 0.97f;
                }
                npc.velocity.Y -= 0.1f;
                return;
            }
            var center = npc.Center;
            npc.TargetClosest(faceTarget: false);
            npc.spriteDirection = 1;
            if (!npc.HasValidTarget)
            {
                npc.ai[0] = -1f;
                return;
            }

            Player target = Main.player[npc.target];
            float targetY = target.position.Y;
            if (hot)
            {
                targetY -= npc.height * 6f;
            }
            else
            {
                targetY += npc.width * 4f;
            }
            float diffY = targetY - npc.position.Y;
            if (targetY < npc.position.Y)
            {
                if (npc.velocity.Y > -4f)
                {
                    npc.velocity.Y -= 0.1f;
                }
            }
            else
            {
                if (diffY > 240f)
                {
                    if (npc.velocity.Y < 3.5f)
                    {
                        npc.velocity.Y += 0.15f;
                    }
                }
                else
                {
                    if (npc.ai[1] == 1f)
                    {
                        if (npc.velocity.Y < 2f)
                        {
                            npc.velocity.Y += 0.05f;
                        }
                    }
                    else
                    {
                        if (npc.velocity.Y < 1.5f)
                        {
                            npc.velocity.Y += 0.025f;
                        }
                    }
                }
            }
            float targetX = target.position.X + target.width / 2f;
            if (npc.velocity.X < 0f)
            {
                targetX -= npc.width * 3f;
            }
            else
            {
                targetX += npc.width * 3f;
            }
            float diffX = targetX - (npc.position.X + npc.width / 2f);
            if (targetX < npc.position.X) // moving left
            {
                if (npc.ai[1] == 1f)
                {
                    if (npc.velocity.X > -4f)
                    {
                        npc.velocity.X -= 0.15f;
                    }
                }
                else
                {
                    if (npc.velocity.X > -2f)
                    {
                        npc.velocity.X -= 0.075f;
                    }
                }
            }
            else // moving right
            {
                if (npc.ai[1] == 1f)
                {
                    if (npc.velocity.X < 4f)
                    {
                        npc.velocity.X += 0.15f;
                    }
                }
                else
                {
                    if (npc.velocity.X < 2f)
                    {
                        npc.velocity.X += 0.075f;
                    }
                }
            }

            // attack stuff
            if (diffY.Abs() < 200f)
            {
                if (diffX.Abs() < 100f)
                {
                    npc.ai[0] += 0.25f;
                }
                else
                {
                    npc.ai[0]++;
                    npc.velocity.X *= 0.99f;
                }
                if ((int)npc.ai[0] > 90)
                {
                    int time = (int)(npc.ai[0] - 90) % 25;
                    if (time == 0)
                    {
                        npc.netUpdate = true;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float veloX = targetX - npc.position.X + npc.width / 2f;
                            veloX /= 60f;
                            if (hot)
                            {

                            }
                            else
                            {
                                Main.PlaySound(SoundID.Item66, npc.Center);
                                Projectile.NewProjectile(npc.Center, new Vector2(veloX, -48f),
                                    ModContent.ProjectileType<Projectiles.Monster.TemperatureBombCold>(), 30, 1f, Main.myPlayer);
                            }
                        }
                    }
                    if (npc.ai[0] > 165f)
                    {
                        npc.ai[0] = 0f;
                    }
                }
            }
            else
            {
                if (npc.ai[0] > 0f && npc.ai[0] < 90f)
                {
                    npc.ai[0] -= 0.5f;
                }
            }

            // collision
            var rect = npc.getRect();
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (i != npc.whoAmI && Main.npc[i].active && Main.npc[i].type == npc.type && Main.npc[i].getRect().Intersects(rect)) // if there are multiple temperature balloons colliding
                {
                    var normal = Vector2.Normalize(center - Main.npc[i].Center);
                    npc.velocity += normal;
                }
            }

            // particle stuff
            float yMult = npc.height / (float)npc.width;
            for (int i = 0; i < 10 * AQMod.EffectQuality; i++)
            {
                int d = Dust.NewDust(npc.position, npc.width, npc.height, 16);
                Main.dust[d].position = center;
                var offset = new Vector2(Main.rand.NextFloat(npc.width - 20f) / 2f, 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
                offset.Y *= yMult;
                Main.dust[d].alpha = 222;
                Main.dust[d].position += offset;
                Main.dust[d].velocity *= 0.1f;
                Main.dust[d].velocity += npc.velocity.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f));
                Main.dust[d].noGravity = true;
                Main.dust[d].scale = Main.rand.NextFloat(0.2f, 2.25f);
            }

            Lighting.AddLight(npc.Center, new Vector3(-1f, -1f, -1f));
        }

        public override void FindFrame(int frameHeight)
        {
            if (!_setupFrame) // sets up the width of the frame since this NPC has horizontal frames
            {
                _setupFrame = true;
                npc.frame.Width /= FramesX;
            }
            npc.frame.X = (int)(npc.frame.Width * (npc.ai[1] - 1));
            if (npc.velocity.X > 0f)
            {
                npc.frameCounter += 1.0d;
            }
            else
            {
                npc.frameCounter -= 1.0d;
            }
            if (npc.frameCounter > 5.0d)
            {
                npc.frameCounter = 0.0d;
                if (npc.frame.Y > 0)
                {
                    npc.frame.Y -= frameHeight;
                }
            }
            else if (npc.frameCounter < -5.0d)
            {
                npc.frameCounter = 0.0d;
                if (npc.frame.Y < frameHeight * (Main.npcFrameCount[npc.type] - 1))
                {
                    npc.frame.Y += frameHeight;
                }
            }
        }

        public override void NPCLoot()
        {
            if (Main.rand.NextBool(2))
                Item.NewItem(npc.getRect(), ModContent.ItemType<Items.Materials.Energies.AtmosphericEnergy>());

            if (!Main.hardMode)
                return;

            if ((int)npc.ai[1] == 1)
            {
                if (Main.rand.NextBool(12))
                {
                    Item.NewItem(npc.getRect(), ModContent.ItemType<Items.Weapons.Magic.SunbaskMirror>());
                }
            }
            else
            {
                if (Main.rand.NextBool(12))
                {
                    Item.NewItem(npc.getRect(), ModContent.ItemType<Items.Weapons.Magic.UnityMirror>());
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = Main.npcTexture[npc.type];
            var offset = new Vector2(npc.width / 2f, npc.height / 2f);
            Vector2 origin = npc.frame.Size() / 2f;
            Vector2 drawPos = npc.Center - Main.screenPosition;

            float mult = 1f / NPCID.Sets.TrailCacheLength[npc.type];
            for (int i = 0; i < NPCID.Sets.TrailCacheLength[npc.type]; i++)
            {
                Main.spriteBatch.Draw(texture, npc.oldPos[i] + offset - Main.screenPosition, npc.frame, drawColor * 0.05f * (mult * (NPCID.Sets.TrailCacheLength[npc.type] - i)), npc.oldRot[i], origin, npc.scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(texture, drawPos, npc.frame, drawColor, npc.rotation, origin, npc.scale, SpriteEffects.None, 0f);
            
            if (AQMod.EffectQuality >= 1f) // extra cloud fx
            {
                UnifiedRandom random = new UnifiedRandom(npc.whoAmI);
                var cloudTexture = TextureCache.Lights[LightTex.Spotlight30x30];
                var cloudOrigin = cloudTexture.Size() / 2f;
                for (int i = 0; i < NPCID.Sets.TrailCacheLength[npc.type] * 2 * AQMod.EffectQuality; i++)
                {
                    var off = new Vector2(random.NextFloat(-10, 8), random.NextFloat(-6, 6));
                    float time = random.NextFloat(-MathHelper.Pi, MathHelper.Pi);
                    var scale = new Vector2(npc.scale * (float)Math.Sin(time + Main.GlobalTime * 4f) + 2f, npc.scale * (float)Math.Sin(time + random.NextFloat(-0.1f, 0.1f) + Main.GlobalTime * 4f) + 1f);
                    scale *= random.NextFloat(0.25f, 0.2f);
                    var clr = drawColor * scale.Length();
                    Main.spriteBatch.Draw(cloudTexture, npc.position + offset - Main.screenPosition + off, null, clr * random.NextFloat(0.65f, 0.85f), 0f, cloudOrigin, scale, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(cloudTexture, npc.position + offset - Main.screenPosition + off, null, clr * random.NextFloat(0.05f, 0.28f), 0f, cloudOrigin, scale * random.NextFloat(1.5f, 2f), SpriteEffects.None, 0f);
                }
            }
            return false;
        }

        bool IDecideFallThroughPlatforms.Decide()
        {
            return true;
        }
    }
}