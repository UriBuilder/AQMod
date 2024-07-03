﻿using System.Collections.Generic;
using Terraria.GameContent;
using Terraria.Localization;

namespace Aequu2.Core.ContentGeneration;

public abstract class UnifiedWhipItem : ModItem, IWhipController {
    public abstract void SetWhipSettings(Projectile projectile, ref WhipSettings settings);
    public abstract void DrawWhip(IWhipController.WhipDrawParams drawInfo);

    public virtual void ModifyWhipHitNPC(Projectile whip, NPC target, ref NPC.HitModifiers modifiers) { }
    public virtual void OnWhipHitNPC(ref float damagePenalty, Projectile whip, NPC target, in NPC.HitInfo hit, int damageDone) { }

    public virtual void WhipAI(Projectile projectile, List<Vector2> WhipPoints, float Progress) { }

    public virtual Color GetWhipStringColor(Vector2 position) {
        return ExtendLight.Get(position);
    }

    [CloneByReference]
    protected ModProjectile WhipProjectile { get; private set; }

    protected override bool CloneNewInstances => true;

    public sealed override void Load() {
        WhipProjectile = new InstancedWhipProjectile(this, Name, Texture + "Proj");
        Mod.AddContent(WhipProjectile);

        if (this is IMinionTagController whipTagController) {
            whipTagController.TagBuff = new InstancedMinionTagDebuff(this as IMinionTagController, Name);
            Mod.AddContent(whipTagController.TagBuff);
        }
    }

    public sealed override void Unload() {
        WhipProjectile = null;
    }

    public sealed override bool MeleePrefix() {
        return true;
    }
}

public interface IWhipController : ILocalizedModType {
    void SetWhipSettings(Projectile projectile, ref WhipSettings settings);
    void ModifyWhipHitNPC(Projectile whip, NPC target, ref NPC.HitModifiers modifiers);
    /// <param name="damagePenalty">Defaults to 0.5.</param>
    /// <param name="whip"></param>
    /// <param name="target"></param>
    /// <param name="hit"></param>
    /// <param name="damageDone"></param>
    void OnWhipHitNPC(ref float damagePenalty, Projectile whip, NPC target, in NPC.HitInfo hit, int damageDone);
    void WhipAI(Projectile projectile, List<Vector2> WhipPoints, float Progress);
    void DrawWhip(WhipDrawParams drawInfo);
    Color GetWhipStringColor(Vector2 position);
    public record struct WhipDrawParams(Texture2D Texture, Projectile Projectile, int SegmentIndex, int SegmentCount, Vector2 Position, Vector2 Next, SpriteEffects SpriteEffects);
}

internal class InstancedWhipProjectile(IWhipController controller, string name, string texture) : InstancedModProjectile(name, texture) {
    private readonly IWhipController _controller = controller;

    private readonly List<Vector2> _controlPoints = [];

    public override LocalizedText DisplayName => _controller.GetLocalization("DisplayName");

    public override void SetStaticDefaults() {
        ProjectileID.Sets.IsAWhip[Type] = true;
    }

    public override void SetDefaults() {
        Projectile.DefaultToWhip();
        Projectile.hide = true;
        _controller.SetWhipSettings(Projectile, ref Projectile.WhipSettings);
    }

    public override void AI() {
        _controlPoints.Clear();
        Projectile.FillWhipControlPoints(Projectile, _controlPoints);
        Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out int _, out float _);
        float progress = Projectile.ai[0] / timeToFlyOut;

        _controller.WhipAI(Projectile, _controlPoints, progress);
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) {
        _controller.ModifyWhipHitNPC(Projectile, target, ref modifiers);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        float penalty = 0.5f;
        _controller.OnWhipHitNPC(ref penalty, Projectile, target, in hit, damageDone);

        if (_controller is IMinionTagController minionTag) {
            target.AddBuff(minionTag.TagBuff.Type, minionTag.TagDuration);
        }

        // Whips set the target of the player's minions.
        Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;

        // Handle multipierce damage penalty.
        Projectile.damage = (int)(Projectile.damage * penalty);
    }

    private void DrawLine(List<Vector2> list) {
        Texture2D texture = TextureAssets.FishingLine.Value;
        Rectangle frame = texture.Frame();
        Vector2 origin = new Vector2(frame.Width / 2, 2);

        Vector2 position = list[0];
        for (int i = 0; i < list.Count - 2; i++) {
            Vector2 next = list[i];
            Vector2 difference = list[i + 1] - next;

            float rotation = difference.ToRotation() - MathHelper.PiOver2;
            Color color = _controller.GetWhipStringColor(next);
            Vector2 scale = new Vector2(1, (difference.Length() + 2) / frame.Height);

            Main.EntitySpriteDraw(texture, position - Main.screenPosition, frame, color, rotation, origin, scale, SpriteEffects.None, 0);

            position += difference;
        }
    }

    private void DrawWhip(List<Vector2> list) {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

        Vector2 position = list[0];
        for (int i = 0; i < list.Count - 1; i++) {
            Vector2 next = list[i + 1];
            Vector2 difference = next - position;
            float rotation = difference.ToRotation() + MathHelper.PiOver2;

            _controller.DrawWhip(new(texture, Projectile, i, list.Count, position, next, spriteEffects));

            position = next;
        }
    }

    public override bool PreDraw(ref Color lightColor) {
        _controlPoints.Clear();
        Projectile.FillWhipControlPoints(Projectile, _controlPoints);

        DrawLine(_controlPoints);

        DrawWhip(_controlPoints);
        return false;
    }
}

public interface IMinionTagController : IMinionTagNPCController, ILocalizedModType {
    public ModBuff TagBuff { get; set; }

    public abstract int TagDuration { get; }

    public virtual void UpdateTagBuff(NPC npc, ref int buffIndex) { }
}

public interface IMinionTagNPCController {
    public virtual void ModifyMinionHit(NPC npc, Projectile minionProj, ref NPC.HitModifiers modifiers, float tagMultiplier) { }
    public virtual void OnMinionHit(NPC npc, Projectile minionProj, in NPC.HitInfo hit, int damageDone) { }
}

internal class InstancedMinionTagDebuff(IMinionTagController controller, string name) : InstancedBuff(name, AequusTextures.TemporaryDebuffIcon), IMinionTagNPCController {
    internal readonly IMinionTagController _controller = controller;

    public override LocalizedText DisplayName => _controller.GetLocalization("DisplayName");
    public override LocalizedText Description => LocalizedText.Empty;

    public override void SetStaticDefaults() {
        BuffSets.IsATagBuff[Type] = true;
        BuffSets.CanBeRemovedByNetMessage[Type] = true;
    }

    public override void Update(NPC npc, ref int buffIndex) {
        _controller.UpdateTagBuff(npc, ref buffIndex);
    }

    void IMinionTagNPCController.ModifyMinionHit(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers, float tagMultiplier) {
        _controller.ModifyMinionHit(npc, projectile, ref modifiers, tagMultiplier);
    }

    void IMinionTagNPCController.OnMinionHit(NPC npc, Projectile minionProj, in NPC.HitInfo hit, int damageDone) {
        _controller.OnMinionHit(npc, minionProj, in hit, damageDone);
    }
}

internal class MinionTagNPC : GlobalNPC {
    public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers) {
        if (projectile.npcProj || projectile.trap || !projectile.IsMinionOrSentryRelated) {
            return;
        }

        float tagMultiplier = ProjectileID.Sets.SummonTagDamageMultiplier[projectile.type];

        for (int i = 0; i < NPC.maxBuffs; i++) {
            if (npc.buffTime[i] > 0 && BuffLoader.GetBuff(npc.buffType[i]) is IMinionTagNPCController controller) {
                controller.ModifyMinionHit(npc, projectile, ref modifiers, tagMultiplier);
            }
        }
    }

    public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone) {
        if (projectile.npcProj || projectile.trap || !projectile.IsMinionOrSentryRelated) {
            return;
        }

        for (int i = 0; i < NPC.maxBuffs; i++) {
            if (npc.buffTime[i] > 0 && BuffLoader.GetBuff(npc.buffType[i]) is IMinionTagNPCController controller) {
                controller.OnMinionHit(npc, projectile, in hit, damageDone);
            }
        }
    }
}

internal static partial class Extensions {
    public static void RemoveTagBuff(this IMinionTagController tag, NPC npc) {
        npc.RequestBuffRemoval(tag.TagBuff.Type);
    }
}