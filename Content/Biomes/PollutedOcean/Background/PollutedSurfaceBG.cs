﻿namespace Aequus.Content.Biomes.PollutedOcean.Background;

public class PollutedSurfaceBG : ModSurfaceBackgroundStyle {
    public override void Load() {
        BackgroundTextureLoader.AddBackgroundTexture(Mod, AequusTextures.PollutedSurfaceBG.FullPath);
    }

    public override void ModifyFarFades(float[] fades, float transitionSpeed) {
        for (int i = 0; i < fades.Length; i++) {
            if (i == Slot) {
                fades[i] += transitionSpeed;
                if (fades[i] > 1f) {
                    fades[i] = 1f;
                }
            }
            else {
                fades[i] -= transitionSpeed;
                if (fades[i] < 0f) {
                    fades[i] = 0f;
                }
            }
        }
    }

    public override int ChooseFarTexture() {
        return BackgroundTextureLoader.GetBackgroundSlot(Mod, AequusTextures.PollutedSurfaceBG.Path);
    }

    public override int ChooseMiddleTexture() {
        return -1;
    }

    public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b) {
        return -1;
    }
}