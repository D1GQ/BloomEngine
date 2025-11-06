using BloomEngine.Behaviours;
using HarmonyLib;
using Il2CppReloaded.UI;
using UnityEngine;

namespace PvZEnhanced.Patches;

[HarmonyPatch(typeof(MainMenuPanelView))]
static internal class MainMenuPatches
{
    [HarmonyPatch(nameof(MainMenuPanelView.Start))]
    [HarmonyPostfix]
    private static void MainMenuStart(MainMenuPanelView __instance)
    {
        if (__instance.transform.Find("Canvas").Find("ModListManager") is not null)
            return;

        GameObject modList = new GameObject("ModListManager");
        modList.transform.SetParent(__instance.transform.Find("Canvas"), false);
        modList.AddComponent<ModListManager>();
    }
}