﻿using Aequus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Boss.Expert
{
    public class LittleInferno : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemDefaults.RarityDustDevil;
            Item.value = ItemDefaults.DustDevilValue;
            Item.expert = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().accDustDevilFire = true;
        }

        public static void InfernoPotionEffect(Player player, Vector2 where, int npcWhoAmIBlacklist = -1)
        {
            Lighting.AddLight(where, 0.65f, 0.4f, 0.1f);
            if (player.whoAmI != Main.myPlayer)
            {
                return;
            }

            int fireDebuff = BuffID.OnFire;
            float minDistance = 200f;
            bool dealDamage = player.infernoCounter % 60 == 0;
            int damageDealt = 10;
            for (int k = 0; k < 200; k++)
            {
                if (k == npcWhoAmIBlacklist)
                    continue;

                NPC nPC = Main.npc[k];
                if (nPC.active && !nPC.friendly && nPC.damage > 0 && !nPC.dontTakeDamage && !nPC.buffImmune[fireDebuff] && player.CanNPCBeHitByPlayerOrPlayerProjectile(nPC) && Vector2.Distance(where, nPC.Center) <= minDistance)
                {
                    int d = nPC.FindBuffIndex(fireDebuff);
                    if (d == -1 || nPC.buffTime[d] < 8)
                    {
                        nPC.AddBuff(fireDebuff, 120);
                    }
                    if (dealDamage)
                    {
                        player.ApplyDamageToNPC(nPC, damageDealt, 0f, 0, crit: false);
                    }
                }
            }
            if (!player.hostile)
            {
                return;
            }
            for (int l = 0; l < 255; l++)
            {
                Player plr = Main.player[l];
                if (plr == player || !plr.active || plr.dead || !plr.hostile || plr.buffImmune[fireDebuff] || plr.team == player.team && player.team != 0 || !(Vector2.Distance(where, plr.Center) <= minDistance))
                {
                    continue;
                }
                if (plr.FindBuffIndex(fireDebuff) == -1)
                {
                    plr.AddBuff(fireDebuff, 120);
                }
                if (dealDamage)
                {
                    plr.Hurt(PlayerDeathReason.LegacyEmpty(), damageDealt, 0, pvp: true);
                    if (Main.netMode != NetmodeID.SinglePlayer)
                    {
                        NetMessage.SendPlayerHurt(l, PlayerDeathReason.ByOther(16), damageDealt, 0, critical: false, pvp: true, -1);
                    }
                }
            }
        }

        public static void DrawInfernoRings(Vector2 screenPosition, float opacity = 1f)
        {
            Main.instance.LoadFlameRing();

            var texture = TextureAssets.FlameRing.Value;
            var frame = texture.Frame(verticalFrames: 3, frameY: 0);
            var origin = frame.Size() / 2f;
            for (int i = 0; i < 3; i++)
            {
                float wave = ((float)Math.Sin(Main.GlobalTimeWrappedHourly * 10f + i * (MathHelper.Pi / 1.5f)) + 1f) / 2f;
                Main.spriteBatch.Draw(texture, screenPosition, frame, new Color(255, 255, 255, 128) * opacity * wave, Main.GlobalTimeWrappedHourly * 1.5f + i, origin, 1f + wave * 0.1f - 0.05f, SpriteEffects.None, 0f);
            }
        }
    }
}