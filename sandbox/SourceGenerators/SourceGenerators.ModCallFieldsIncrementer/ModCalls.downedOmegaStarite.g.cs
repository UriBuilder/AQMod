﻿namespace Aequus;

public partial class ModCallHandler {
    [System.Runtime.CompilerServices.CompilerGenerated]
    static bool downedOmegaStarite(object[] args) {
        if (args.Length > 1) {
            if (args[1] is Mod mod) {
                LogInfo($"Mod ({mod.Name}) can remove the legacy Mod parameter. As it is no longer needed.");
            }

            if (args[^1] is bool value) {
                global::Aequus.AequusWorld.downedOmegaStarite = value;
            }
            else {
                LogError($"Mod Call Parameter index ^1 (\"value\") did not match Type \"bool\".");
            }
        }

        return global::Aequus.AequusWorld.downedOmegaStarite;
    }

    [System.Runtime.CompilerServices.CompilerGenerated]
    static System.Func<bool> downedOmegaStariteGetter(object[] args) {            
        return () => global::Aequus.AequusWorld.downedOmegaStarite;
    }

    [System.Runtime.CompilerServices.CompilerGenerated]
    static System.Action<bool> downedOmegaStariteSetter(object[] args) {            
        return value => global::Aequus.AequusWorld.downedOmegaStarite = value;
    }
}