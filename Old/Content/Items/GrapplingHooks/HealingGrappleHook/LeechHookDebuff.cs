﻿using Terraria.Localization;

namespace Aequu2.Old.Content.Items.GrapplingHooks.HealingGrappleHook;

public class LeechHookDebuff : ModBuff {
    public override string Texture => Aequu2Textures.TemporaryDebuffIcon;

    public override void SetStaticDefaults() {
        Main.debuff[Type] = true;
    }

    public override LocalizedText DisplayName => Language.GetText(this.GetLocalizationKey("DisplayName"));
    public override LocalizedText Description => LocalizedText.Empty;
}
