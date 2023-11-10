﻿using Aequus.Content.DataSets;
using Aequus.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common.Tiles;

public class PotsGlobalTile : GlobalTile {
    public override bool? IsTileDangerous(int i, int j, int type, Player player) {
        if (type != TileID.Pots) {
            return null;
        }

        return (!PotsSystem.LootPreviews.TryGetValue(new(i, j), out var value) || !value.Dangerous) ? null : true;
    }

    public override void PostDraw(int i, int j, int type, SpriteBatch spriteBatch) {
        if (!TileSets.IsSmashablePot[type] || Main.tile[i, j].TileFrameX % 36 != 18 || Main.tile[i, j].TileFrameY % 36 != 18) {
            return;
        }

        i--;
        j--;
        var point = new Point(i, j);
        if (PotsSystem.LootPreviews.TryGetValue(point, out var preview)) {
            return;
        }

        if (Main.LocalPlayer.GetModPlayer<AequusPlayer>().accAnglerLamp != null && PotsSystem.InPotSightRange(Main.LocalPlayer, point, Main.LocalPlayer.GetModPlayer<AequusPlayer>().accAnglerLamp.potSightRange)) {
            GoreDisabler.Begin();
            NewItemCache.Begin();
            NewProjectileCache.Begin();
            NewNPCCache.Begin();

            int x2 = 0;
            int y2 = j;
            for (x2 += Main.tile[i, j].TileFrameX / 18; x2 > 1; x2 -= 2) {
            }
            x2 *= -1;
            x2 += i;
            int num3 = Main.tile[i, j].TileFrameY / 18;
            int style = 0;
            while (num3 > 1) {
                num3 -= 2;
                style++;
            }
            y2 -= num3;

            PotsSystem.KillTile_DropItems.Invoke(null, new object[] { x2, y2, Main.tile[x2, y2], true, true });
            PotsSystem.SpawnThingsFromPot.Invoke(null, new object[] { i, j, x2, y2, style });

            PotsSystem.PotLootPreview newPreview;
            if (NewNPCCache.NPCs.Count > 0) {
                int npcType = NewNPCCache.NPCs[0].type;
                Main.instance.LoadNPC(npcType);
                var texture = TextureAssets.Npc[npcType];
                newPreview = new(texture.Value, texture.Frame(verticalFrames: Main.npcFrameCount[npcType], frameY: 0), Stack: NewNPCCache.NPCs.Count, Dangerous: !ContentSamples.NpcsByNetId[npcType].friendly && ContentSamples.NpcsByNetId[npcType].damage > 0);
            }
            else if (NewProjectileCache.Projectiles.Count > 0) {
                int projectileType = NewProjectileCache.Projectiles[0].type;
                Main.instance.LoadProjectile(projectileType);
                var texture = TextureAssets.Projectile[projectileType];
                newPreview = new(texture.Value, texture.Frame(verticalFrames: Main.projFrames[projectileType], frameY: 0), Stack: NewProjectileCache.Projectiles.Count, Dangerous: !ContentSamples.ProjectilesByType[projectileType].friendly || ContentSamples.ProjectilesByType[projectileType].hostile || ContentSamples.ProjectilesByType[projectileType].aiStyle == ProjAIStyleID.Explosive);
            }
            else if (NewItemCache.DroppedItems.Count > 0) {
                int itemType = NewItemCache.DroppedItems[0].type;
                Main.GetItemDrawFrame(itemType, out var itemTexture, out var itemFrame);
                newPreview = new(itemTexture, itemFrame, Stack: NewItemCache.DroppedItems[0].stack, Dangerous: false);
            }
            else {
                newPreview = new(TextureAssets.Cd.Value, null, Stack: 1, Dangerous: true);
            }
            PotsSystem.LootPreviews.Add(point, newPreview);
        }
    }
}