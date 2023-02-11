﻿using Aequus.Items.Misc.Energies;
using Aequus.Items.Misc.Materials;
using Aequus.NPCs.Boss.DustDevil;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Boss.Summons
{
    public class TornadoInABottle : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Type] = ItemID.Sets.SortingPriorityBossSpawns[ItemID.QueenSlimeCrystal];
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToHoldUpItem();
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 5);
        }

        public override bool CanUseItem(Player player)
        {
            return player.ZoneSkyHeight && !NPC.AnyNPCs(ModContent.NPCType<DustDevil>());
        }

        public override bool? UseItem(Player player)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                NPC.SpawnBoss((int)player.position.X + player.width / 2, (int)player.position.Y - 1200, ModContent.NPCType<DustDevil>(), player.whoAmI);
            }
            else if (Main.myPlayer == player.whoAmI)
            {
                NetMessage.SendData(MessageID.SpawnBoss, number: player.whoAmI, number2: ModContent.NPCType<DustDevil>());
            }
            SoundEngine.PlaySound(SoundID.Roar, player.position);
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Fluorescence>(10)
                .AddIngredient<FrozenTear>(10)
                .AddIngredient<AtmosphericEnergy>()
                .AddTile(TileID.DemonAltar)
                .TryRegisterAfter(ItemID.GoblinBattleStandard);
        }
    }
}