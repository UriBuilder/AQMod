﻿using Aequus.Core.Graphics.Animations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Aequus.Content.Tiles.CrabPots;

public class AnimationCrabPot : ITileAnimation {
    public int Frame;
    public int FrameCount;
    public int RealFrame;

    public bool Update(int x, int y) {
        if (FrameCount == 0 && Frame == 0) {
            SoundEngine.PlaySound(SoundID.DoorOpen, new Vector2(x, y).ToWorldCoordinates());
        }
        if (++FrameCount > 3) {
            FrameCount = 0;
            Frame++;
            RealFrame = Frame;
            if (Frame >= CrabPot.FramesCount) {
                RealFrame = CrabPot.FramesCount - (Frame - CrabPot.FramesCount + 1);
            }
        }
        return RealFrame > -1;
    }
}