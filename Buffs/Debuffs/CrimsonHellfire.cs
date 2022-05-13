﻿using Aequus.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs.Debuffs
{
    public class CrimsonHellfire : ModBuff
    {
        public static Color FireColor => new Color(175, 65, 20, 10);
        public static Color BloomColor => new Color(175, 10, 2, 10);

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<NPCDebuffs>().hasCrimsonHellfire = true;
        }

        public static void AddStack(NPC npc, int time, int stacksAmt, bool inflictOnFireInsteadIfItsImmune = true)
        {
            if (npc.buffImmune[ModContent.BuffType<CrimsonHellfire>()])
            {
                if (inflictOnFireInsteadIfItsImmune)
                {
                    npc.AddBuff(BuffID.OnFire, time);
                }
            }
            else
            {
                npc.AddBuff(ModContent.BuffType<CrimsonHellfire>(), time);
                npc.GetGlobalNPC<NPCDebuffs>().hasCrimsonHellfire = true;
                npc.GetGlobalNPC<NPCDebuffs>().crimsonHellfireStacks += (byte)stacksAmt;
                npc.netUpdate = true;
            }
        }
    }
}
