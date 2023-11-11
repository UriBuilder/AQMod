﻿using Aequus.Common.Items;
using Aequus.Common.Items.EquipmentBooster;
using Aequus.Core.Autoloading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Tools.AnglerLamp;

[AutoloadGlowMask]
public class AnglerLamp : ModItem {
    public static int PotSightRange { get; set; } = 300;
    public static int ConsumptionRate { get; set; } = 180;
    public static Vector3 LightColor { get; set; } = new Vector3(0.5f, 0.38f, 0.12f);
    public static float LightBrightness { get; set; } = 1.7f;
    public static float LightUseBrightness { get; set; } = 2.5f;
    public static int DebuffType { get; set; } = BuffID.Confused;
    public static int DebuffTime { get; set; } = 180;
    public static int DebuffRange { get; set; } = 240;

    private List<Dust> _dustEffects = new();

    public float animation;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(TextHelper.Seconds(AnglerLamp.ConsumptionRate), AnglerLamp.PotSightRange / 16);

    public override void SetStaticDefaults() {
        EquipBoostDatabase.Instance.SetNoEffect(Type);
    }

    public override void SetDefaults() {
        Item.width = 16;
        Item.height = 24;
        Item.rare = ItemCommons.Rarity.PollutedOceanLoot;
        Item.value = ItemCommons.Price.PollutedOceanLoot;
        Item.useAmmo = AmmoID.Gel;
        Item.holdStyle = ItemHoldStyleID.HoldLamp;
        Item.useStyle = ItemUseStyleID.RaiseLamp;
        Item.useTime = 40;
        Item.useAnimation = 40;
        Item.UseSound = SoundID.Item20;
        Item.autoReuse = true;
    }

    public override bool CanUseItem(Player player) {
        return !player.wet;
    }

    private Vector2 GetLampPosition(Player player) {
        return player.MountedCenter + new Vector2(player.direction * 16f - 0.5f, -2f * player.gravDir);
    }

    public override bool? UseItem(Player player) {
        player.ConsumeItem(ItemID.Gel);
        var lampPosition = GetLampPosition(player);
        _dustEffects.Clear();
        for (int i = 0; i < 12; i++) {
            var d = Dust.NewDustPerfect(lampPosition, DustID.Torch, Scale: 1.5f);
            d.velocity = (i / 13f * MathHelper.TwoPi).ToRotationVector2() * 2f;
            d.noGravity = true;
            d.fadeIn = d.scale + 0.3f;
            d.noLight = true;
            _dustEffects.Add(d);
        }

        int closestNPC = -1;
        float closestNPCDistance = float.MaxValue;
        for (int i = 0; i < Main.maxNPCs; i++) {
            if (Main.npc[i].active && Main.npc[i].damage > 0) {
                float distance = player.Distance(Main.npc[i].Center);
                if (distance >= DebuffRange) {
                    continue;
                }

                Main.npc[i].AddBuff(DebuffType, DebuffTime);
                if (Main.npc[i].HasBuff(BuffID.OnFire3)) {
                    Main.npc[i].AddBuff(BuffID.OnFire3, DebuffTime);
                }
                else if (distance < closestNPCDistance && !Main.npc[i].buffImmune[BuffID.OnFire3]) {
                    closestNPC = i;
                    closestNPCDistance = distance;
                }
            }
        }

        if (closestNPC > -1) {
            Main.npc[closestNPC].AddBuff(BuffID.OnFire3, DebuffTime);
        }
        return true;
    }

    public override void HoldItem(Player player) {
        int animationEndTime = 8;
        if (player.itemTime > animationEndTime) {
            var lampPosition = GetLampPosition(player);
            float animationProgress = (player.itemTime - animationEndTime) / (float)(player.itemTimeMax - animationEndTime);
            float outwards = MathF.Sin((1f - MathF.Pow(animationProgress, 2f)) * 2.2f) * 24f;
            float positionLerp = 1f - MathF.Pow(1f - animationProgress, 3f);
            for (int i = 0; i < _dustEffects.Count; i++) {
                if (!_dustEffects[i].active) {
                    _dustEffects.RemoveAt(i);
                    i--;
                }
                else {
                    _dustEffects[i].rotation = Utils.AngleLerp(_dustEffects[i].rotation, 0f, animationProgress);
                    _dustEffects[i].scale = Math.Max(_dustEffects[i].scale, 0.33f);
                    _dustEffects[i].position = Vector2.Lerp(_dustEffects[i].position, lampPosition + (i / (float)_dustEffects.Count * MathHelper.TwoPi).ToRotationVector2() * outwards, positionLerp);
                }
            }
        }
        else {
            _dustEffects.Clear();
        }

        if (player.wet) {
            Item.holdStyle = ItemHoldStyleID.None;
            return;
        }

        player.aggro += 200;
        player.GetModPlayer<AequusPlayer>().potSightRange += PotSightRange;
        Item.holdStyle = ItemHoldStyleID.HoldLamp;

        float brightness = LightBrightness;
        if (player.itemTime > 0) {
            float animationProgress = player.itemTime / (float)player.itemTimeMax;
            brightness = MathHelper.Lerp(brightness, LightUseBrightness, animationProgress);
        }
        Lighting.AddLight(player.Center, LightColor * brightness);
    }

    public override Color? GetAlpha(Color lightColor) {
        return Color.Lerp(lightColor, Color.White, 0.4f);
    }

    public override void PostUpdate() {
        Lighting.AddLight(Item.Center, LightColor * LightBrightness / 2f);
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        spriteBatch.Draw(TextureAssets.Item[Type].Value, position, frame, drawColor, MathF.Sin(animation * MathHelper.TwoPi * 2f) * animation * 0.3f, origin, scale, SpriteEffects.None, 0f);
        if (animation > 0f) {
            animation -= 0.033f;
            if (animation < 0f) {
                animation = 0f;
            }
        }
        return false;
    }

    public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) {
        animation = 0f;
    }
}