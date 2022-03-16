﻿using AQMod.Content.World;
using AQMod.Effects;
using System.Reflection;
using Terraria;
using Terraria.ID;

namespace AQMod.Common.Utilities
{
    public static class TimeActions
    {
        public static class Hooks
        {
            internal static MethodInfo Main_UpdateTime_SpawnTownNPCs;

            internal static void Main_UpdateTime(On.Terraria.Main.orig_UpdateTime orig)
            {
                AQWorld.UpdatingTime = true;
                Main.dayRate += AQWorld.DayrateIncrease;
                if (Main.dayTime)
                {
                    if (Main.time + Main.dayRate > Main.dayLength)
                    {
                        AprilFoolsJoke.Check();
                        if (Main.netMode != NetmodeID.Server)
                        {
                            SkyGlimmerEvent.InitNight();
                        }
                    }
                    orig();
                }
                else
                {
                    if (Main.time + Main.dayRate > Main.nightLength)
                    {
                    }
                    orig();
                    if (MiscWorldInfo.villagerMoveInAtNight && !Main.dayTime)
                    {
                        Main_UpdateTime_SpawnTownNPCs.Invoke(null, null);
                    }
                }
                AQWorld.DayrateIncrease = 0;
                MessageBroadcast.PreventChat = false;
                MessageBroadcast.PreventChatOnce = false;
                AQWorld.UpdatingTime = false;
            }
        }

        public const double MaxTime = Main.dayLength + Main.nightLength;
        public const double HourMultiplier = 60d * 60d;
        public const double FourHoursThirtyMinutes = HourMultiplier * 4.5d;

        public static double GetInGameTime()
        {
            return Main.dayTime ? Main.time : Main.dayLength + Main.time;
        }

        public static double GetInGameTimePercentage()
        {
            return GetInGameTime() / MaxTime;
        }

        public static double GetInGameTimePercentageUsing12AMAs0Percent()
        {
            double time = GetInGameTime() + FourHoursThirtyMinutes;
            if (time > MaxTime)
            {
                time -= MaxTime; // wrap around
            }
            return time / MaxTime;
        }
    }
}