﻿using Aequus.Content.ItemPrefixes.Armor;
using Terraria;
using Terraria.ID;

namespace Aequus.Items.Armor.Misc {
    public class ArgonArmorPolish : ArmorPolishItem<ArgonPrefix> {
        public override string Texture => AequusTextures.ElitePlantArgon.Path;

        public override void SetDefaults() {
            base.SetDefaults();
            Item.color = Colors.CoinCopper;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(silver: 20);
        }
    }
}