﻿namespace Aequus;
public enum PacketType : byte {
    RequestTileSectionFromServer,
    SyncNecromancyOwner,
    SyncAequusPlayer,
    SyncSound,
    DemonSiegeSacrificeStatus,
    StartDemonSiege,
    RemoveDemonSiege,
    PumpinatorWindSpeed,
    SpawnHostileOccultist,
    PhysicsGunBlock,
    RequestGlimmerEvent,
    Unused5,
    SpawnOmegaStarite,
    GlimmerStatus,
    Unused2,
    SyncDronePoint,
    CarpenterBountiesCompleted,
    AequusTileSquare,
    OnKillEffect,
    ApplyNameTagToNPC,
    RequestChestItems,
    RequestAnalysisQuest,
    SpawnShutterstockerClip,
    AnalysisRarity,
    Unused,
    GravityChestPickupEffect,
    SpawnPixelCameraClip,
    PlacePixelPainting,
    RegisterPhotoClip,
    Unused3,
    AddBuilding,
    RemoveBuilding,
    CompleteCarpenterBounty,
    ResetCarpenterBounties,
    Unused4,
    SendDebuffFlatDamage,
    UniqueTileInteraction,
    NightfallOnHit,
    NightfallPush,
    FoolsGoldRingEffect,
    DayNightInit,
    EliteBuffPlantsActivate,
    UseSundialResetItem,
    RecordBreaker,
    WormScarfDodge,
    LuckyDropSpawn,
    SyncNecromancyNPCPacket,
    ZombieConvertEffectsPacket,
    SyncNecromancyOwnerPacket,
    NewNameTag,
    AddNewNameTagMarker,
    RemoveNewNameTagMarker,
    CartographyTable,
    CartographyTableSubmit,
    MagicChestPlacement,
    Backpack,
    EightBal,
    CrabPotUse,
    CrabPotPlace,
    CrabPotAddBait,
    Count
}