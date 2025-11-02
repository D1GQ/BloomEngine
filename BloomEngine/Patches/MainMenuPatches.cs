using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Reloaded.UI;
using System.Text;
using TMPro;
using UnityEngine;

namespace PvZEnhanced.Patches;

[HarmonyPatch(typeof(MainMenuPanelView))]
static internal class MainMenuPatches
{
    [HarmonyPatch(nameof(MainMenuPanelView.Start))]
    [HarmonyPostfix]
    private static void MainMenuStart(MainMenuPanelView __instance)
    {
        Canvas canvas = __instance.gameObject.GetComponentInChildren<Canvas>();
        Transform parent = canvas.transform.FindChild("Layout").FindChild("Center");

        TMP_FontAsset font = parent.FindChild("Main").FindChild("AccountSign").FindChild("SignTop").FindChild("NameLabel").GetComponent<TextMeshProUGUI>().font;
        TextMeshProUGUI text = CreateLabel(parent, font);
        CreateLabelBackground(parent, text, 15, 0.9f);
    }

    private static TextMeshProUGUI CreateLabel(Transform parent, TMP_FontAsset font)
    {
        var obj = new GameObject("PluginsListText");
        obj.transform.SetParent(parent, false);

        TextMeshProUGUI label = obj.AddComponent<TextMeshProUGUI>();
        label.fontSize = 56;
        label.characterSpacing = 3;
        label.alignment = TextAlignmentOptions.TopLeft;
        label.enableWordWrapping = false;
        label.font = font;

        RectTransform rect = label.GetComponent<RectTransform>();
        rect.pivot = new Vector2(0, 0);
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(0, 0);
        rect.anchoredPosition = new Vector2(25, 25);

        // Set text to list of loaded plugins
        StringBuilder pluginsText = new StringBuilder();
        foreach (var plugin in IL2CPPChainloader.Instance.Plugins.Values)
            pluginsText.AppendLine($"{plugin.Metadata.Name} v{plugin.Metadata.Version}");

        label.text = pluginsText.ToString();

        // Determine the height based on content
        label.ForceMeshUpdate();
        Vector2 textSize = label.GetRenderedValues(false);
        rect.sizeDelta = new Vector2(600, textSize.y);

        return label;
    }

    private static void CreateLabelBackground(Transform parent, TextMeshProUGUI text, int padding, float transparency)
    {
        GameObject bg = new GameObject("PluginsListTextBackground");
        bg.transform.SetParent(parent, false);

        UnityEngine.UI.Image img = bg.AddComponent<UnityEngine.UI.Image>();
        img.color = new Color(0, 0, 0, transparency);

        RectTransform bgRect = img.GetComponent<RectTransform>();
        bgRect.sizeDelta = text.GetRenderedValues(false) + new Vector2(padding * 2, padding * 2);
        bgRect.pivot = text.rectTransform.pivot;
        bgRect.anchorMin = text.rectTransform.anchorMin;
        bgRect.anchorMax = text.rectTransform.anchorMax;
        bgRect.anchoredPosition = text.rectTransform.anchoredPosition - new Vector2(padding, padding);

        bg.transform.SetAsLastSibling();
        text.transform.SetAsLastSibling();
    }
}