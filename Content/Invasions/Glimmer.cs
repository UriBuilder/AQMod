﻿using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Aequus.Content.Invasions
{
    public sealed class Glimmer : ModSystem
    {
        public static Color CosmicEnergyColor => new Color(200, 10, 255, 0);

        public static InvasionStatus Status { get; set; }
        public static int omegaStarite;
    }
}