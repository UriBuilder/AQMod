﻿using System;
using System.Reflection;

namespace Aequus.Core;

public class IOHooks : ILoadable {
    public delegate void PreSaveDelegate(bool toCloud);
    public static PreSaveDelegate PreSaveWorld { get; internal set; }

    public void Load(Mod mod) {
        DetourHelper.AddHook(Aequus.TerrariaAssembly.GetType("Terraria.ModLoader.IO.WorldIO").GetMethod("Save", BindingFlags.NonPublic | BindingFlags.Static), typeof(IOHooks).GetMethod(nameof(On_WorldIO_Save), BindingFlags.NonPublic | BindingFlags.Static));
    }

    private static void On_WorldIO_Save(Action<string, bool> orig, string path, bool isCloudSave) {
        PreSaveWorld?.Invoke(isCloudSave);
        orig(path, isCloudSave);
    }

    public void Unload() {
        PreSaveWorld = null;
    }
}