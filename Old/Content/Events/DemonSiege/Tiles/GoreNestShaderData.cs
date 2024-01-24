﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Shaders;

namespace Aequus.Tiles.CraftingStations;

public class GoreNestShaderData : MiscShaderData {
    public GoreNestShaderData(Ref<Effect> shader, string passName) : base(shader, passName) {
        UseColor(new Vector3(5f, 0f, 0f)).UseSecondaryColor(new Vector3(4f, 0, 2f))
            .UseSaturation(1f).UseOpacity(1f);
    }

    public override void Apply() {
        Shader.Parameters["colorLerpMult"].SetValue(0.45f + MathF.Sin(Main.GlobalTimeWrappedHourly * 10 * 0.1f));
        Shader.Parameters["thirdColor"].SetValue(new Vector3(2f, 4f, 0));
        base.Apply();
    }
}