using BloomEngine.Menu;
using HarmonyLib;
using Il2CppUI.Scripts;

namespace BloomEngine.Patches;

[HarmonyPatch(typeof(AchievementsUI))]
internal static class ModMenuPatches
{
    /// <summary>
    /// When the achievements menu is initialized, create the mod menu
    /// </summary>
    [HarmonyPatch(nameof(AchievementsUI.Start))]
    private static void Postfix(AchievementsUI __instance)
    {
        if(!__instance.GetComponent<ModMenuManager>())
            __instance.gameObject.AddComponent<ModMenuManager>();
    }

    /// <summary>
    /// When the mod menu is closed, hide the currently open config panel
    /// </summary>
    [HarmonyPatch(nameof(AchievementsUI.SetAchievementsIsActive))]
    private static void Prefix(AchievementsUI __instance, bool isActive)
    {
        if (!isActive && ModMenu.IsConfigPanelOpen)
            ModMenu.HideConfigPanel();
    }
}