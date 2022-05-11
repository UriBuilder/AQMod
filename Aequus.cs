using Aequus.Common.Networking;
using Aequus.Common.Utilities;
using Aequus.Content.Necromancy;
using Aequus.Items;
using Aequus.Items.Recipes;
using Aequus.NPCs;
using Aequus.NPCs.Monsters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus
{
    public class Aequus : Mod
    {
        internal delegate void LegacyDrawMethod(Texture2D texture, Vector2 position, Rectangle? frame, Color color, float scale, Vector2 origin, float rotation, SpriteEffects effects, float layerDepth);

        public const string TextureNone = "Aequus/Assets/None";
        public const string VanillaTexture = "Terraria/Images/";

        public static Aequus Instance { get; private set; }
        public static UserInterface InventoryInterface { get; private set; }
        public static UserInterface NPCTalkInterface { get; private set; }

        public static bool GameWorldActive => Main.instance.IsActive && !Main.gamePaused && !Main.gameInactive;
        public static bool HQ => ClientConfig.Instance.HighQuality;
        public static bool LogMore => ClientConfig.Instance.InfoDebugLogs;

        internal static Color GreenSlimeColor => ContentSamples.NpcsByNetId[NPCID.GreenSlime].color;
        internal static Color BlueSlimeColor => new Color(0, 80, 255, 100);

        public override void Load()
        {
            Instance = this;
            AequusHelpers.OnModLoad(this);
            AequusText.OnModLoad(this);
            ClientConfig.OnModLoad(this);
            if (Main.netMode != NetmodeID.Server)
            {
                InventoryInterface = new UserInterface();
                NPCTalkInterface = new UserInterface();
            }
        }

        public override void AddRecipeGroups()
        {
            AequusRecipes.Groups.AddRecipeGroups();
            NecromancyDatabase.FinalizeContent();
            NecromancyNPC.AdjustBuffImmunities();
        }

        public override void AddRecipes()
        {
            AlternativeRecipes.AddRecipes();
            ItemsCatalogue.LoadAutomaticEntries();
        }

        public override void Unload()
        {
            Instance = null;
            AequusHelpers.Unload();
            InventoryInterface = null;
            NPCTalkInterface = null;
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            PacketType type = PacketSender.ReadPacketType(reader);

            var l = Instance.Logger;
            if (type != PacketType.SyncNPCNetworkerGlobals && type != PacketType.SyncNecromanyProjectile)
            {
                l.Debug("Recieving Packet: " + type);
            }
            if (type == PacketType.SyncNPCNetworkerGlobals)
            {
                int npc = reader.ReadInt32();
                var globals = PacketSender.GetNetworkerGlobals(Main.npc[npc]);
                for (int i = 0; i < globals.Length; i++)
                {
                    globals[i].Receive(npc, reader);
                }
            }
            else if (type == PacketType.SyncNecromancyOwnerTier)
            {
                int npc = reader.ReadInt32();
                Main.npc[npc].GetGlobalNPC<NecromancyNPC>().zombieOwner = reader.ReadInt32();
                Main.npc[npc].GetGlobalNPC<NecromancyNPC>().zombieDebuffTier = reader.ReadSingle();
            }
            else if (type == PacketType.SyncNecromanyProjectile)
            {
                int projectileOwner = reader.ReadInt32();
                int projectileIdentity = reader.ReadInt32();
                int projectile = AequusHelpers.FindProjectileIdentity(projectileOwner, projectileIdentity);
                l.Debug("Data for: " + projectile + ", " + Lang.GetProjectileName(Main.projectile[projectile].type));
                var globals = PacketSender.GetNetworkerGlobals(Main.projectile[projectile]);
                for (int i = 0; i < globals.Length; i++)
                {
                    globals[i].Receive(projectile, reader);
                }
            }
        }

        public override object Call(params object[] args)
        {
            switch ((string)args[0])
            {
                case "NecroStats":
                    return ModContent.GetInstance<NecromancyDatabase>().HandleModCall(this, args);

                case "Downed":
                    return ModContent.GetInstance<AequusWorld.DownedCalls>().HandleModCall(this, args);
            }
            return null;
        }
    }
}