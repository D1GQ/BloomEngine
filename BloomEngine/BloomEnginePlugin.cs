using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace BloomEngine;

[BepInPlugin(Name, Name, Version)]
[BepInProcess("Replanted.exe")]
public class BloomEnginePlugin : BasePlugin
{
    public const string Name = "BloomEngine";
    public const string Version = "1.0.0";
    public const string Id = "com.palmforest.bloomengine";

    public Harmony Harmony { get; } = new Harmony(Id);

    public static ManualLogSource Logger;

    public override void Load()
    {
        Logger = Log;
        Logger.LogInfo($"Successfully initialised {nameof(BloomEngine)}.");

        Harmony.PatchAll();
    }
}