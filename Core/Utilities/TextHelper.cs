﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Terraria.Chat;
using Terraria.Localization;

namespace Aequus.Core.Utilities;

public static class TextHelper {
    public const char AirCharacter = '⠀';
    public const string AirString = "⠀";

    private record struct ColoredText(string Text, Color Color);

    #region Colors
    public static readonly Color BossSummonMessage = new Color(175, 75, 255);
    public static readonly Color EventMessageColor = new Color(50, 255, 130);
    public static readonly Color TownNPCArrived = new Color(50, 125, 255);
    public static readonly Color PrefixGood = new Color(120, 190, 120);
    public static readonly Color PrefixBad = new Color(190, 120, 120);
    #endregion

    #region Localization
    /// <summary>
    /// Gets a <see cref="ILocalizedModType"/>'s "DisplayName" value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static LocalizedText GetDisplayName(ILocalizedModType localizedModType) {
        return localizedModType.GetLocalization("DisplayName");
    }

    /// <summary>
    /// Gets a <see cref="ILocalizedModType"/>'s "DisplayName" value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static LocalizedText GetDisplayName<T>() where T : class, ILocalizedModType {
        return GetDisplayName(ModContent.GetInstance<T>());
    }

    public static bool ContainsKey(string key) {
        return Language.GetTextValue(key) != key;
    }

    public static bool TryGet(string key, out LocalizedText text) {
        text = Language.GetText(key);
        return text.Key != text.Value;
    }
    public static bool TryGetValue(string key, out string text) {
        bool value = TryGet(key, out var localizedText);
        text = localizedText.Value;
        return value;
    }

    public static string GetCategoryTextValue(this ILocalizedModType self, string suffix) {
        return self.GetCategoryText(suffix).Value;
    }

    public static string GetCategoryKey(this ILocalizedModType self, string suffix, Func<string> defaultValueFactory = null) {
        return $"Mods.{self.Mod.Name}.{self.LocalizationCategory}.{suffix}";
    }

    public static LocalizedText GetCategoryText(this ILocalizedModType self, string suffix, Func<string> defaultValueFactory = null) {
        return Language.GetOrRegister(GetCategoryKey(self, suffix), defaultValueFactory);
    }

    public static LocalizedText DayOfWeek(DayOfWeek dayOfWeek) {
        return Language.GetText("Mods.Aequus.Misc.DayOfTheWeek." + dayOfWeek.ToString());
    }

    public static string PriceTextColored(long value, string NoValueText = "", bool AlphaPulse = false) {
        return string.Join(' ', GetPriceTextSegments(value, NoValueText).Select((t) => t.Color == Color.White ? t.Text : ColorCommand(t.Text, t.Color, AlphaPulse)));
    }

    public static string PriceText(long value, string NoValueText = "") {
        return string.Join(' ', GetPriceTextSegments(value, NoValueText).Select((t) => t.Text));
    }

    private static IEnumerable<ColoredText> GetPriceTextSegments(long value, string NoValueText = "") {
        int platinum = 0;
        int gold = 0;
        int silver = 0;
        int copper = 0;
        int itemValue = (int)value;

        if (itemValue < 1) {
            yield return new ColoredText(NoValueText, Color.White);
        }

        if (itemValue >= Item.platinum) {
            platinum = itemValue / Item.platinum;
            itemValue -= platinum * Item.platinum;
        }
        if (itemValue >= Item.gold) {
            gold = itemValue / Item.gold;
            itemValue -= gold * Item.gold;
        }
        if (itemValue >= Item.silver) {
            silver = itemValue / Item.silver;
            itemValue -= silver * Item.silver;
        }
        if (itemValue >= Item.copper) {
            copper = itemValue;
        }

        if (platinum > 0) {
            yield return new ColoredText(platinum + " " + Lang.inter[15].Value, Colors.CoinPlatinum);
        }
        if (gold > 0) {
            yield return new ColoredText(gold + " " + Lang.inter[16].Value, Colors.CoinGold);
        }
        if (silver > 0) {
            yield return new ColoredText(silver + " " + Lang.inter[17].Value, Colors.CoinSilver);
        }
        if (copper > 0) {
            yield return new ColoredText(copper + " " + Lang.inter[18].Value, Colors.CoinCopper);
        }
    }

    public static string GetUseAnimationText(float useAnimation) {
        if (useAnimation <= 8) {
            return Language.GetTextValue("LegacyTooltip.6");
        }
        else if (useAnimation <= 20) {
            return Language.GetTextValue("LegacyTooltip.7");
        }
        else if (useAnimation <= 25) {
            return Language.GetTextValue("LegacyTooltip.8");
        }
        else if (useAnimation <= 30) {
            return Language.GetTextValue("LegacyTooltip.9");
        }
        else if (useAnimation <= 35) {
            return Language.GetTextValue("LegacyTooltip.10");
        }
        else if (useAnimation <= 45) {
            return Language.GetTextValue("LegacyTooltip.11");
        }
        else if (useAnimation <= 55) {
            return Language.GetTextValue("LegacyTooltip.12");
        }
        return Language.GetTextValue("LegacyTooltip.13");
    }

    public static string GetKnockbackText(float knockback) {
        if (knockback == 0f) {
            return Language.GetTextValue("LegacyTooltip.14");
        }
        else if (knockback <= 1.5) {
            return Language.GetTextValue("LegacyTooltip.15");
        }
        else if (knockback <= 3f) {
            return Language.GetTextValue("LegacyTooltip.16");
        }
        else if (knockback <= 4f) {
            return Language.GetTextValue("LegacyTooltip.17");
        }
        else if (knockback <= 6f) {
            return Language.GetTextValue("LegacyTooltip.18");
        }
        else if (knockback <= 7f) {
            return Language.GetTextValue("LegacyTooltip.19");
        }
        else if (knockback <= 9f) {
            return Language.GetTextValue("LegacyTooltip.20");
        }
        else if (knockback <= 11f) {
            return Language.GetTextValue("LegacyTooltip.21");
        }
        return Language.GetTextValue("LegacyTooltip.22");
    }

    public static string GetKeybindKeys(ModKeybind keybind) {
        string value = "";
        foreach (var s in keybind.GetAssignedKeys()) {
            if (!string.IsNullOrEmpty(value))
                value += ", ";
            value += s;
        }
        if (string.IsNullOrEmpty(value)) {
            value = Language.GetTextValue("Mods.Aequus.KeyUnbound");
        }
        return value;
    }
    #endregion

    #region Text Commands
    public static string ColorCommandStart(Color color, bool alphaPulse = false) {
        if (alphaPulse) {
            color = Colors.AlphaDarken(color);
        }
        return $"[c/{color.Hex3()}:";
    }
    public static string ColorCommand(string text, Color color, bool alphaPulse = false) {
        return $"{ColorCommandStart(color, alphaPulse)}{text}]";
    }

    public static string ItemCommand(int itemID) {
        return "[i:" + itemID + "]";
    }
    public static string ItemCommand<T>() where T : ModItem {
        return ItemCommand(ModContent.ItemType<T>());
    }
    #endregion

    #region Time
    public static string WatchTime(double time, bool dayTime) {
        string text = "AM";
        if (!dayTime) {
            time += 54000.0;
        }

        time = time / 86400.0 * 24.0;
        time = time - 7.5 - 12.0;
        if (time < 0.0) {
            time += 24.0;
        }

        if (time >= 12.0) {
            text = "PM";
        }

        int intTime = (int)time;
        double deltaTime = time - intTime;
        deltaTime = (int)(deltaTime * 60.0);
        string text2 = string.Concat(deltaTime);
        if (deltaTime < 10.0) {
            text2 = "0" + text2;
        }

        if (intTime > 12) {
            intTime -= 12;
        }

        if (intTime == 0) {
            intTime = 12;
        }

        return $"{intTime}:{text2} {text}";
    }

    /// <summary>
    /// Converts ticks to seconds, up to 1 decimal place.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string Minutes(double value) {
        return Decimals(value / 3600.0);
    }

    /// <summary>
    /// Converts ticks to seconds, up to 1 decimal place.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string Seconds(double value) {
        return Decimals(value / 60.0);
    }
    #endregion

    #region Chat Messages
    /// <summary>
    /// Broadcasts a message. Only does something when <see cref="Main.netMode"/> is not equal to <see cref="NetmodeID.MultiplayerClient"/>.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="color"></param>
    /// <param name="args"></param>
    internal static void Broadcast(string key, Color color, params object[] args) {
        if (Main.netMode == NetmodeID.SinglePlayer) {
            Main.NewText(Language.GetTextValue(key, args), color);
        }
        else if (Main.netMode == NetmodeID.Server) {
            ChatHelper.BroadcastChatMessage(NetworkText.FromKey(key, args), color);
        }
    }

    /// <summary>
    /// Broadcasts a language key.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="color"></param>
    public static void Broadcast(string text, Color color) {
        BroadcastLiteral(text, color);
    }

    /// <summary>
    /// Broadcasts a literal key.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="color"></param>
    public static void BroadcastLiteral(string text, Color color) {
        if (Main.netMode != NetmodeID.Server) {
            Main.NewText(Language.GetTextValue(text), color);
            return;
        }

        ChatHelper.BroadcastChatMessage(NetworkText.FromKey(text), color);
    }

    public static void BroadcastAwakened(NPC npc) {
        if (Main.netMode == NetmodeID.SinglePlayer) {
            BroadcastAwakened(npc.TypeName);
        }
        else if (Main.netMode == NetmodeID.Server) {
            BroadcastAwakened(Lang.GetNPCName(npc.netID).Key);
        }
    }

    public static void BroadcastAwakened(string npcName) {
        Broadcast("Announcement.HasAwoken", BossSummonMessage, npcName);
    }
    #endregion

    #region Numbers
    public static string Percent(double value) {
        return Decimals(value * 100f);
    }
    public static string Decimals(double value) {
        return value.ToString("0.0", Language.ActiveCulture.CultureInfo.NumberFormat).Replace(".0", "");
    }
    #endregion

    #region Debug
    /// <summary>Registers a localization key if it doesn't exist. Only ran if compiled with a DEBUG symbol.</summary>
    [Conditional("DEBUG")]
    internal static void RegisterKey(string key) {
        Language.GetOrRegister(key);
    }
    #endregion
}