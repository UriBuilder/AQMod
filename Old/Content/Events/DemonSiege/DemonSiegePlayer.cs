﻿namespace Aequus.Old.Content.Events.DemonSiege;

public class DemonSiegePlayer : ModPlayer {
    public Point GoreNest { get; private set; }

    public override void PreUpdate() {
        GoreNest = Point.Zero;

        Vector2 center = Player.Center;
        float closestDistance = float.MaxValue;
        foreach (var s in DemonSiegeSystem.ActiveSacrifices) {
            float distance = Vector2.Distance(center, new Vector2(s.Value.TileX * 16f + 24f, s.Value.TileY * 16f));
            if (distance < s.Value.Range && distance < closestDistance) {
                closestDistance = distance;
                GoreNest = s.Key;
            }
        }
    }
}
