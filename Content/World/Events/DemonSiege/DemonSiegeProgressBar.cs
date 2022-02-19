﻿using AQMod.Assets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AQMod.Content.World.Events.DemonSiege
{
    public class DemonSiegeProgressBar : EventProgressBar
    {
        public override Texture2D IconTexture => ModContent.GetTexture(TexturePaths.EventIcons + "demonsiege");
        public override string EventName => Language.GetTextValue("Mods.AQMod.EventName.DemonSiege");
        public override Color NameBGColor => new Color(120, 90 + (int)(Math.Sin(Main.GlobalTime * 5f) * 10), 20, 128);
        public override float EventProgress => 1f - EventDemonSiege.UpgradeTime / (float)EventDemonSiege.Upgrade.upgradeTime;

        public override bool IsActive() => Main.LocalPlayer.Biomes().zoneDemonSiege;
        public override string ModifyProgressText(string text) => Language.GetTextValue("Mods.AQMod.Common.TimeLeft", AQUtils.TimeText3(EventDemonSiege.UpgradeTime));
    }
}
