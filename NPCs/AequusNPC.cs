﻿using Aequus.Buffs;
using Aequus.Buffs.Debuffs;
using Aequus.Common.ItemDrops;
using Aequus.Content.Necromancy;
using Aequus.Graphics;
using Aequus.Graphics.RenderTargets;
using Aequus.Items;
using Aequus.Items.Accessories;
using Aequus.Items.Accessories.Summon.Sentry;
using Aequus.Items.Consumables.CursorDyes;
using Aequus.Items.Consumables.Foods;
using Aequus.Items.Misc.Energies;
using Aequus.Items.Pets;
using Aequus.Items.Placeable;
using Aequus.Items.Weapons.Ranged;
using Aequus.Items.Weapons.Summon.Necro.Candles;
using Aequus.NPCs.Monsters;
using Aequus.NPCs.Monsters.Jungle;
using Aequus.Particles;
using Aequus.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.NPCs
{
    public class AequusNPC : GlobalNPC
    {
        public struct OnKillPlayerInfo
        {
            public Player player;
            public AequusPlayer aequus;
            public float distance;
        }

        public static HashSet<int> HeatDamage { get; private set; }
        public static HashSet<int> DontModifyVelocity { get; private set; }

        public override bool InstancePerEntity => true;

        public bool heatDamage;
        public bool noHitEffect;
        public bool infernoOnFire;

        public int oldLife;
        public byte mindfungusStacks;
        public byte corruptionHellfireStacks;
        public byte crimsonHellfireStacks;
        public byte locustStacks;
        public int jungleCoreInvasion;
        public int jungleCoreInvasionIndex;

        public float statSpeed;

        public override void Load()
        {
            HeatDamage = new HashSet<int>()
            {
                NPCID.Lavabat,
                NPCID.LavaSlime,
                NPCID.FireImp,
                NPCID.MeteorHead,
                NPCID.HellArmoredBones,
                NPCID.HellArmoredBonesMace,
                NPCID.HellArmoredBonesSpikeShield,
                NPCID.HellArmoredBonesSword,
                NPCID.BlazingWheel,
            };
            DontModifyVelocity = new HashSet<int>()
            {
                NPCID.CultistBoss,
                NPCID.HallowBoss,
            };

            On.Terraria.NPC.UpdateNPC_Inner += NPC_UpdateNPC_Inner; // fsr detouring NPC.Update(int) doesn't work, but this does
            On.Terraria.NPC.UpdateCollision += NPC_UpdateCollision;
            On.Terraria.NPC.VanillaHitEffect += Hook_PreHitEffect;
        }

        private static void NPC_UpdateNPC_Inner(On.Terraria.NPC.orig_UpdateNPC_Inner orig, NPC self, int i)
        {
            if (AequusHelpers.iterations == 0 && self.HasBuff<BitCrushedDebuff>())
            {
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    return;
                }
                int rate = 7;
                if (Main.GameUpdateCount % rate == 0)
                {
                    for (int k = 0; k < rate - 1; k++)
                    {
                        AequusHelpers.iterations = k + 1;
                        orig(self, i);
                    }
                    AequusHelpers.iterations = 0;
                }
                return;
            }
            orig(self, i);
        }

        private static void NPC_UpdateCollision(On.Terraria.NPC.orig_UpdateCollision orig, NPC self)
        {
            if (DontModifyVelocity.Contains(self.netID))
            {
                orig(self);
                return;
            }

            float velocityBoost = self.Aequus().statSpeed - 1f;
            var oldVelocity = self.velocity;
            if (velocityBoost != 0f)
            {
                self.velocity *= 1f + velocityBoost;
            }
            orig(self);
            if (velocityBoost != 0f)
            {
                self.velocity = oldVelocity;
            }
        }
        private static void Hook_PreHitEffect(On.Terraria.NPC.orig_VanillaHitEffect orig, NPC self, int hitDirection, double dmg)
        {
            try
            {
                if (self.TryGetGlobalNPC<AequusNPC>(out var aequus))
                {
                    if (aequus.noHitEffect)
                    {
                        return;
                    }
                }

                if (Main.netMode != NetmodeID.Server)
                {
                    goto Orig;
                }

                if (self.HasBuff<Bleeding>())
                {
                    int amt = (int)Math.Min(4.0 + dmg / 20.0, 20.0);
                    for (int i = 0; i < amt; i++)
                    {
                        bool foodParticle = Main.rand.NextBool();
                        var d = Dust.NewDustDirect(self.position, self.width, self.height, foodParticle ? DustID.Blood : DustID.FoodPiece, newColor: foodParticle ? new Color(200, 20, 30, 100) : default);
                        d.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(2f, 5f);
                        d.velocity += self.velocity * 0.5f;
                        if (Main.rand.NextBool(3))
                        {
                            d.noGravity = true;
                        }
                    }
                }

                if (self.life <= 0 && self.HasBuff<SnowgraveDebuff>()
                    && SnowgraveCorpse.CanFreezeNPC(self))
                {
                    SoundEngine.PlaySound(SoundID.Item30, self.Center);
                    return;
                }
            }
            catch
            {

            }
        Orig:
            orig(self, hitDirection, dmg);
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            switch (npc.type)
            {
                case NPCID.QueenBee:
                    npcLoot.Add(ItemDropRule.ByCondition(DropRulesBuilder.NotExpertCondition, ModContent.ItemType<OrganicEnergy>(), 1, 3, 3));
                    break;

                case NPCID.Pixie:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PixieCandle>(), 100));
                    break;

                case NPCID.BloodZombie:
                case NPCID.Drippler:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodMoonCandle>(), 100));
                    break;

                case NPCID.DevourerHead:
                case NPCID.GiantWormHead:
                case NPCID.BoneSerpentHead:
                case NPCID.TombCrawlerHead:
                case NPCID.DiggerHead:
                case NPCID.DuneSplicerHead:
                case NPCID.SeekerHead:
                case NPCID.BloodEelHead:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SpicyEel>(), 25));
                    break;

                case NPCID.CultistBoss:
                    npcLoot.Add(ItemDropRule.ByCondition(DropRulesBuilder.FlawlessCondition, ModContent.ItemType<MothmanMask>()));
                    break;

                case NPCID.WallofFlesh:
                    npcLoot.Add(ItemDropRule.ByCondition(new LegacyFuncConditional(() => AequusWorld.downedEventDemon, "DemonSiege", "Mods.Aequus.DropCondition.NotBeatenDemonSiege"), ModContent.ItemType<GoreNest>()));
                    break;
            }
        }

        public override void SetDefaults(NPC npc)
        {
            if (HeatDamage.Contains(npc.type))
            {
                heatDamage = true;
            }
            infernoOnFire = false;
        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (npc.type == NPCID.DungeonSpirit && AequusHelpers.HereditarySource(source, out var ent) && ent is NPC parent)
            {
                if (Heckto.Spawnable.Contains(parent.type))
                {
                    npc.Transform(ModContent.NPCType<Heckto>());
                }
            }
        }

        public override void ResetEffects(NPC npc)
        {
            statSpeed = 1f;
        }

        public void PostAI_JustHit_UpdateInferno(NPC npc)
        {
            foreach (var b in AequusBuff.CountsAsFire)
            {
                if (npc.HasBuff(b))
                {
                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        if (Main.player[i].active && !Main.player[i].dead && npc.Distance(Main.player[i].Center) < 1000f)
                        {
                            if (Main.player[i].Aequus().accDustDevilFire)
                            {
                                infernoOnFire = true;
                            }
                        }
                    }
                    break;
                }
            }
        }
        public void PostAI_UpdateInferno(NPC npc)
        {
            int player = npc.HasValidTarget ? npc.target : Player.FindClosest(npc.position, npc.width, npc.height);
            LittleInferno.InfernoPotionEffect(Main.player[player], npc.Center, npc.whoAmI);

            for (int i = 0; i < NPC.maxBuffs; i++)
            {
                if (AequusBuff.CountsAsFire.Contains(npc.buffType[i]))
                {
                    return;
                }
            }

            infernoOnFire = false;
        }
        public void PostAI_VelocityBoostHack(NPC npc)
        {
            if (npc.noTileCollide)
            {
                float velocityBoost = npc.Aequus().statSpeed - 1f;
                if (velocityBoost > 0f)
                {
                    npc.position += npc.velocity * velocityBoost;
                }
            }
        }
        public void PostAI_DoDebuffEffects(NPC npc)
        {
            if (npc.HasBuff<BlueFire>())
            {
                int amt = (int)(npc.Size.Length() / 16f);
                for (int i = 0; i < amt; i++)
                    AequusEffects.BehindPlayers.Add(new BloomParticle(Main.rand.NextCircularFromRect(npc.getRect()) + Main.rand.NextVector2Unit() * 8f, -npc.velocity * 0.1f + new Vector2(Main.rand.NextFloat(-1f, 1f), -Main.rand.NextFloat(2f, 6f)),
                        new Color(60, 100, 160, 10) * 0.5f, new Color(5, 20, 40, 10), Main.rand.NextFloat(1f, 2f), 0.2f, Main.rand.NextFloat(MathHelper.TwoPi)));
            }
            if (npc.HasBuff<CorruptionHellfire>())
            {
                int amt = (int)(npc.Size.Length() / 16f);
                for (int i = 0; i < amt; i++)
                    AequusEffects.BehindPlayers.Add(new BloomParticle(Main.rand.NextCircularFromRect(npc.getRect()) + Main.rand.NextVector2Unit() * 8f, -npc.velocity * 0.1f + new Vector2(Main.rand.NextFloat(-1f, 1f), -Main.rand.NextFloat(2f, 6f)),
                        CorruptionHellfire.FireColor, CorruptionHellfire.BloomColor * 0.6f, Main.rand.NextFloat(1f, 2f), 0.2f, Main.rand.NextFloat(MathHelper.TwoPi)));
            }
            if (npc.HasBuff<CrimsonHellfire>())
            {
                int amt = (int)(npc.Size.Length() / 16f);
                for (int i = 0; i < amt; i++)
                    AequusEffects.BehindPlayers.Add(new BloomParticle(Main.rand.NextCircularFromRect(npc.getRect()) + Main.rand.NextVector2Unit() * 8f, -npc.velocity * 0.1f + new Vector2(Main.rand.NextFloat(-1f, 1f), -Main.rand.NextFloat(2f, 6f)),
                        CrimsonHellfire.FireColor, CrimsonHellfire.BloomColor * 0.2f, Main.rand.NextFloat(1f, 2f), 0.2f, Main.rand.NextFloat(MathHelper.TwoPi)));
            }
        }
        public override void PostAI(NPC npc)
        {
            oldLife = npc.life;
            if (!npc.SpawnedFromStatue && npc.justHit)
            {
                PostAI_JustHit_UpdateInferno(npc);
            }

            if (infernoOnFire)
            {
                PostAI_UpdateInferno(npc);
            }

            PostAI_VelocityBoostHack(npc);

            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }
            PostAI_DoDebuffEffects(npc);
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (npc.HasBuff<Weakness>())
            {
                byte a = drawColor.A;
                drawColor = (drawColor * 0.9f).UseA(a);
                if (npc.life >= 0 && Main.rand.NextBool(20))
                {
                    npc.HitEffect(0, 10);
                }
            }
            if (npc.life >= 0 && npc.HasBuff<Bleeding>())
            {
                if (Main.rand.NextBool(3))
                {
                    if (Main.rand.NextBool(5))
                        npc.HitEffect(0, 4);
                    bool foodParticle = Main.rand.NextBool();
                    var d = Dust.NewDustDirect(npc.position, npc.width, npc.height, foodParticle ? DustID.Blood : DustID.FoodPiece, newColor: foodParticle ? new Color(200, 20, 30, 100) : default);
                    d.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(1f, 2f);
                    d.velocity += npc.velocity * 0.5f;
                }
            }
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!npc.IsABestiaryIconDummy)
            {
                if (infernoOnFire)
                {
                    float opacity = 0.5f;
                    int time = 0;
                    for (int i = 0; i < NPC.maxBuffs; i++)
                    {
                        if (AequusBuff.CountsAsFire.Contains(npc.buffType[i]))
                        {
                            time = Math.Max(npc.buffTime[i], time);
                        }
                    }
                    if (time < 60)
                    {
                        opacity *= time / 60f;
                    }
                    LittleInferno.DrawInfernoRings(npc.Center - screenPos, opacity);
                }
                if (npc.HasBuff<BitCrushedDebuff>())
                {
                    var r = AequusEffects.EffectRand;
                    r.SetRand((int)(Main.GlobalTimeWrappedHourly * 32f) / 10 + npc.whoAmI * 10);
                    int amt = Math.Max((npc.width + npc.height) / 20, 1);
                    for (int k = 0; k < amt; k++)
                    {
                        var dd = new DrawData(SquareParticle.SquareParticleTexture.Value, npc.Center - Main.screenPosition, null, Color.White, 0f, SquareParticle.SquareParticleTexture.Value.Size() / 2f, (int)r.Rand(amt * 2, amt * 5), SpriteEffects.None, 0);
                        dd.position.X += (int)r.Rand(-npc.width, npc.width);
                        dd.position.Y += (int)r.Rand(-npc.height, npc.height);
                        GamestarRenderer.DrawData.Add(dd);
                    }
                }
            }
            return true;
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (npc.HasBuff<MindfungusDebuff>())
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= 16 + 8 * mindfungusStacks;
                damage += 8 + mindfungusStacks;
            }
            else
            {
                mindfungusStacks = 0;
            }
            if (npc.HasBuff<BlueFire>())
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= 8 * 8;
                damage += 8;
            }
            if (npc.HasBuff<Bleeding>())
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= 8;
                damage += 4;
            }
            UpdateDebuffStack(npc, npc.HasBuff<CorruptionHellfire>(), ref corruptionHellfireStacks, ref damage, 20, 1f);
            UpdateDebuffStack(npc, npc.HasBuff<CrimsonHellfire>(), ref crimsonHellfireStacks, ref damage, 20, 1.1f);
            UpdateDebuffStack(npc, npc.HasBuff<LocustDebuff>(), ref locustStacks, ref damage, 20, 1f);
        }
        public void UpdateDebuffStack(NPC npc, bool has, ref byte stacks, ref int damageNumbers, byte cap = 20, float dotMultiplier = 1f)
        {
            if (!has)
            {
                stacks = 0;
            }
            else
            {
                stacks = Math.Min(stacks, cap);
                int dot = (int)(stacks * dotMultiplier);

                if (dot >= 0)
                {
                    npc.AddRegen(-dot);
                    if (damageNumbers < dot)
                        damageNumbers = dot;
                }
            }
        }

        public override bool SpecialOnKill(NPC npc)
        {
            if (Main.netMode != NetmodeID.Server && npc.HasBuff<SnowgraveDebuff>())
            {
                DeathEffect_SnowgraveFreeze(npc);
            }

            if (Main.netMode == NetmodeID.MultiplayerClient
                || npc.SpawnedFromStatue || NPCID.Sets.BelongsToInvasionOldOnesArmy[npc.type])
            {
                return false;
            }

            var players = GetCloseEnoughPlayers(npc);

            if (npc.realLife == -1)
            {
                if (npc.HasBuff<SoulStolen>())
                {
                    CheckSouls(npc, players);
                }
                var info = NecromancyDatabase.TryGet(npc, out var g) ? g : default(GhostInfo);
                var zombie = npc.GetGlobalNPC<NecromancyNPC>();
                if ((info.PowerNeeded != 0f || zombie.zombieDebuffTier >= 100f) && GhostKill(npc, zombie, info, players))
                {
                    zombie.SpawnZombie(npc);
                }
                int ammoBackpackChance = (int)Math.Max(10f - npc.value / 1000f, 1f);
                foreach (var tuple in players)
                {
                    if (!npc.playerInteraction[tuple.player.whoAmI])
                    {
                        continue;
                    }
                    if (npc.value > (Item.copper * 20) && tuple.aequus.ammoBackpackItem != null && (ammoBackpackChance <= 1 || Main.rand.NextBool(ammoBackpackChance)))
                    {
                        if (tuple.aequus.ammoBackpackItem.Aequus().accBoost)
                        {
                            tuple.aequus.UseAmmoBackpack(npc, tuple.aequus.ammoBackpackItem);
                        }
                        tuple.aequus.UseAmmoBackpack(npc, tuple.aequus.ammoBackpackItem);
                    }
                }
            }
            return false;
        }
        public void CheckSouls(NPC npc, List<OnKillPlayerInfo> players)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                foreach (var p in players)
                {
                    if (p.aequus.candleSouls < p.aequus.soulCandleLimit)
                    {
                        Projectile.NewProjectile(npc.GetSource_Death(), npc.Center, Main.rand.NextVector2Unit() * 1.5f, ModContent.ProjectileType<SoulAbsorbtion>(), 0, 0f, p.player.whoAmI);
                        p.aequus.candleSouls++;
                    }
                }
            }
            else
            {
                var candlePlayers = new List<int>();
                foreach (var p in players)
                {
                    if (p.aequus.candleSouls < p.aequus.soulCandleLimit)
                    {
                        candlePlayers.Add(p.player.whoAmI);
                    }
                }

                if (candlePlayers.Count > 0)
                {
                    PacketSystem.Send((p) =>
                    {
                        p.Write(candlePlayers.Count);
                        p.WriteVector2(npc.Center);
                        for (int i = 0; i < candlePlayers.Count; i++)
                        {
                            p.Write(candlePlayers[i]);
                        }
                    }, PacketType.CandleSouls);
                }
            }
        }
        public bool GhostKill(NPC npc, NecromancyNPC zombie, GhostInfo info, List<OnKillPlayerInfo> players)
        {
            if (zombie.zombieDrain > 0 && info.EnoughPower(zombie.zombieDebuffTier))
            {
                return true;
            }
            if (zombie.conversionChance > 0 && Main.rand.NextBool(zombie.conversionChance))
            {
                return true;
            }
            //for (int i = 0; i < players.Count; i++)
            //{
            //    if (players[i].Aequus().dreamMask && Main.rand.NextBool(4))
            //    {
            //        zombie.zombieOwner = players[i].whoAmI;
            //        zombie.zombieDebuffTier = info.PowerNeeded;
            //        return true;
            //    }
            //}
            return false;
        }
        public void DeathEffect_SnowgraveFreeze(NPC npc)
        {
            if (SnowgraveCorpse.CanFreezeNPC(npc))
            {
                AequusEffects.BehindProjs.Add(new SnowgraveCorpse(npc.Center, npc));
            }
        }
        public List<OnKillPlayerInfo> GetCloseEnoughPlayers(NPC npc)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                return new List<OnKillPlayerInfo>() { new OnKillPlayerInfo { player = Main.LocalPlayer, aequus = Main.LocalPlayer.Aequus(), distance = npc.Distance(Main.LocalPlayer.Center) }, };
            }
            var list = new List<OnKillPlayerInfo>();
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i].active && !Main.player[i].dead)
                {
                    float d = npc.Distance(Main.player[i].Center);
                    if (d < 2000f)
                    {
                        list.Add(new OnKillPlayerInfo { player = Main.player[i], aequus = Main.player[i].Aequus(), distance = d, });
                    }
                }
            }
            return list;
        }

        public override void OnKill(NPC npc)
        {
            if (npc.SpawnedFromStatue || npc.friendly || npc.lifeMax < 5)
                return;

            if (jungleCoreInvasion > 0)
            {
                if (Main.npc[jungleCoreInvasion - 1].active && Main.npc[jungleCoreInvasion - 1].ModNPC is BaseCore core)
                {
                    core.OnKilledMinion(npc, jungleCoreInvasionIndex);
                }
            }

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (npc.playerInteraction[i])
                {
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Main.player[i].Aequus().OnKillEffect(npc.netID, npc.position, npc.width, npc.height, npc.lifeMax);
                        continue;
                    }

                    var p = Aequus.GetPacket(PacketType.OnKillEffect);
                    p.Write(i);
                    p.Write(npc.netID);
                    p.WriteVector2(npc.position);
                    p.Write(npc.width);
                    p.Write(npc.height);
                    p.Write(npc.lifeMax);
                    p.Send(toClient: i);
                }
            }
        }

        public override bool PreChatButtonClicked(NPC npc, bool firstButton)
        {
            if (npc.type == NPCID.Angler)
            {
                if (firstButton)
                {
                    var inv = Main.LocalPlayer.inventory;
                    for (int i = 0; i < Main.InventoryItemSlotsCount; i++)
                    {
                        if (AequusItem.LegendaryFishIDs.Contains(inv[i].type))
                        {
                            Main.LocalPlayer.Aequus().LegendaryFishRewards(npc, inv[i], i);
                            inv[i].stack--;
                            if (inv[i].stack <= 0)
                            {
                                inv[i].TurnToAir();
                            }
                            SoundEngine.PlaySound(SoundID.Grab);
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public override void SetupShop(int type, Chest shop, ref int nextSlot)
        {
            switch (type)
            {
                case NPCID.Merchant:
                    {
                        var inv = Main.LocalPlayer.inventory;
                        for (int i = 0; i < Main.InventoryItemSlotsCount; i++)
                        {
                            if (inv[i].type == ModContent.ItemType<StarPhish>())
                            {
                                AddAvoidDupes(ItemID.Seed, Item.buyPrice(copper: 3), shop, ref nextSlot);
                                break;
                            }
                        }
                    }
                    break;

                case NPCID.Clothier:
                    {
                        if (Aequus.HardmodeTier)
                        {
                            int slot = -1;
                            for (int i = 0; i < Chest.maxItems - 1; i++)
                            {
                                if (shop.item[i].type == ItemID.FamiliarWig || shop.item[i].type == ItemID.FamiliarShirt || shop.item[i].type == ItemID.FamiliarPants)
                                {
                                    slot = i + 1;
                                }
                            }
                            if (slot != -1 && slot != Chest.maxItems - 1)
                            {
                                shop.Insert(ModContent.ItemType<FamiliarPickaxe>(), slot);
                            }
                            nextSlot++;
                        }
                    }
                    break;

                case NPCID.DyeTrader:
                    {
                        int removerSlot = nextSlot;
                        if (Main.LocalPlayer.statLifeMax >= 200)
                        {
                            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<HealthCursorDye>());
                        }
                        if (Main.LocalPlayer.statManaMax >= 100)
                        {
                            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<ManaCursorDye>());
                        }
                        if (LanternNight.LanternsUp)
                        {
                            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SwordCursorDye>());
                        }
                        if (AequusWorld.downedEventDemon)
                        {
                            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<DemonicCursorDye>());
                        }
                        if (nextSlot != removerSlot)
                        {
                            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<CursorDyeRemover>());
                        }
                    }
                    break;

                case NPCID.Mechanic:
                    {
                        shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SantankSentry>());
                    }
                    break;
            }
        }
        public bool AddAvoidDupes(int itemID, int? customPrice, Chest shop, ref int nextSlot)
        {
            for (int i = 0; i < Chest.maxItems; i++)
            {
                if (shop.item[i].type == itemID)
                {
                    if (shop.item[i].shopCustomPrice == customPrice)
                        return false;

                    shop.item[i].shopCustomPrice = customPrice;
                    return true;
                }
            }
            shop.item[nextSlot].SetDefaults(itemID);
            shop.item[nextSlot].shopCustomPrice = customPrice;
            nextSlot++;
            return true;
        }

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            var bb = new BitsByte(jungleCoreInvasion > 0);
            if (bb[0])
            {
                binaryWriter.Write(jungleCoreInvasion);
                binaryWriter.Write(jungleCoreInvasionIndex);
            }
            binaryWriter.Write(locustStacks);
            binaryWriter.Write(corruptionHellfireStacks);
            binaryWriter.Write(crimsonHellfireStacks);
            binaryWriter.Write(mindfungusStacks);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            var bb = (BitsByte)binaryReader.ReadByte();
            if (bb[0])
            {
                jungleCoreInvasion = binaryReader.ReadInt32();
                jungleCoreInvasionIndex = binaryReader.ReadInt32();
            }
            locustStacks = binaryReader.ReadByte();
            corruptionHellfireStacks = binaryReader.ReadByte();
            crimsonHellfireStacks = binaryReader.ReadByte();
            mindfungusStacks = binaryReader.ReadByte();
        }
    }
}