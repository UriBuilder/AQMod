﻿using System;
using System.Collections.Generic;
using tModLoaderExtended.Terraria.ModLoader;

namespace AequusRemake.Systems.Synergy;

public class ReplaceItems : GlobalItem, IAddRecipes {
    private static List<WeakReference<Item>> _updateQueue = new();
    public static readonly Dictionary<int, int> Replace = [];

    public override bool IsLoadingEnabled(Mod mod) {
        return ModLoader.HasMod("Aequus");
    }

    public override void Load() {
        On_Item.SetDefaults_int_bool_ItemVariant += On_Item_SetDefaults;
    }

    void IAddRecipes.AddRecipes(Mod mod) {
        foreach (WeakReference<Item> weakRef in _updateQueue) {
            if (weakRef.TryGetTarget(out Item item) && item != null && Replace.TryGetValue(item.type, out int replaceType)) {
                item.SetDefaults(replaceType);
            }
        }
    }

    private static void On_Item_SetDefaults(On_Item.orig_SetDefaults_int_bool_ItemVariant orig, Item self, int Type, bool noMatCheck, Terraria.GameContent.Items.ItemVariant variant) {
        if (Replace.TryGetValue(Type, out int replaceType)) {
            if (AequusRemake.Instance.PostSetupContentOccured) {
                Type = replaceType;
            }
            else {
                _updateQueue.Add(new(self));
            }
        }

        orig(self, Type, noMatCheck, variant);
    }
}
