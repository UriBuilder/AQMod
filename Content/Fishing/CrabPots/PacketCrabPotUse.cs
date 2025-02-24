﻿using Aequus.Common;
using Aequus.Common.Drawing.TileAnimations;
using Aequus.Common.Net;
using System.IO;
using Terraria.DataStructures;

namespace Aequus.Content.Fishing.CrabPots;

public class PacketCrabPotUse : PacketHandler {
    public override PacketType LegacyPacketType => PacketType.CrabPotUse;

    public void Send(int x, int y, int player, int waterStyleId) {
        var packet = GetPacket();
        packet.Write((ushort)x);
        packet.Write((ushort)y);
        packet.Write((byte)player);
        LiquidsSystem.SendWaterStyle(packet, waterStyleId);
        SendPacket(packet);
    }

    public override void Receive(BinaryReader reader, int sender) {
        var x = reader.ReadUInt16();
        var y = reader.ReadUInt16();
        var plr = reader.ReadByte();
        int waterStyle = LiquidsSystem.ReceiveWaterStyle(reader);

        if (!TileEntity.ByPosition.TryGetValue(new(x, y), out var te) || te is not TECrabPot crabPot) {
            return;
        }

        if (Main.myPlayer == plr) {
            UnifiedCrabPot.GrabItem(x, y, plr, crabPot);
        }
        if (Main.netMode == NetmodeID.Server) {
            Send(x, y, plr, waterStyle);
        }
        else {
            AnimationSystem.GetValueOrAddDefault<AnimationOpenCrabPot>(x, y);
        }
        crabPot.biomeData = new(waterStyle);
        crabPot.ClearItem();
    }
}