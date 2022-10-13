﻿using ShopQuotesMod;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Summon.Sentry
{
    public class SantankSentry : ModItem, ItemHooks.IUpdateItemDye
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            ModContent.GetInstance<QuoteDatabase>().AddNPC(NPCID.Mechanic, Mod).SetQuote(Type, "Mods.Aequus.ShopQuote.Mechanic.SantankSentry");
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.Yellow;
            Item.canBePlacedInVanityRegardlessOfConditions = true;
            Item.value = Item.sellPrice(gold: 9);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().accSentryInheritence = Item;
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return CheckMechsSentry(equippedItem) && CheckMechsSentry(incomingItem);
        }
        public bool CheckMechsSentry(Item item)
        {
            return item.type != ModContent.ItemType<MechsSentry>();
        }

        void ItemHooks.IUpdateItemDye.UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem)
        {
            if (!isSetToHidden || !isNotInVanitySlot)
            {
                player.Aequus().equippedEyes = Type;
                player.Aequus().cEyes = dyeItem.dye;
            }
        }
    }
}