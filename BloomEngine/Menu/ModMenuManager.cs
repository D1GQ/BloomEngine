using BloomEngine.Utilities;
using Il2CppInterop.Runtime.Attributes;
using Il2CppTMPro;
using Il2CppUI.Scripts;
using MelonLoader;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BloomEngine.Menu;

internal class ModMenuManager : MonoBehaviour
{
    public bool ModMenuOpen { get; private set; } = false;
    private readonly List<GameObject> entryObjects = new List<GameObject>();
    private Transform container;
    private AchievementsUI ui;

    private static Sprite configIconSprite;

    public void Start()
    {
        ui = transform.GetComponentInParent<AchievementsUI>();
        container = transform.parent.Find("ScrollView").Find("Viewport").Find("Content").Find("Achievements");

        // Prevent header from blocking clicks on mod entries
        container.parent.FindComponent<Image>("Header/Shadow").raycastTarget = false;
        container.parent.FindComponent<Image>("Header/Left/Background_grass02").raycastTarget = false;

        configIconSprite = AssetHelper.LoadSprite("BloomEngine.Assets.ConfigIcon.png");

        CreateButtons();
        CreateEntries();
    }


    private void CreateButtons()
    {
        GameObject obj = UIHelper.CreateButton("ModsButton", transform, "Mods", OpenModList);

        // Position the modsButton in the bottom left corner
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.pivot = new Vector2(0, 1);
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(25, rect.rect.height + 100);

        // Update the achievements button to reset the text and hide the mod entries
        transform.parent.parent.FindComponent<Button>("Main/BG_Tree/AchievementsButton").onClick.AddListener((UnityAction)(() =>
        {
            SetHeaderText("Achievements");
            SetEntriesEnabled(false);
        }));

        ui.m_backButton.onClick.AddListener((UnityAction)(() => ModMenuOpen = false));
    }

    private void CreateEntries()
    {
        foreach (var mod in MelonMod.RegisteredMelons)
        {
            // Create a new mod achievement for this mod
            GameObject modObj = GameObject.Instantiate(transform.parent.parent.Find("Achievements/AchievementItem").gameObject, container);
            modObj.SetActive(true);
            modObj.name = $"ModEntry_{mod.Info.Name}";
            entryObjects.Add(modObj);

            // Get this mod's mod menu entry if it has one
            bool isRegistered = ModMenu.Entries.TryGetValue(mod, out ModEntry entry);

            var title = modObj.FindComponent<TextMeshProUGUI>("Title");
            var subheader = modObj.FindComponent<TextMeshProUGUI>("Subheader");

            // Sets the name and description of unregistered mods to default values
            if(!isRegistered)
            {
                title.text = mod.Info.Name;
                title.color = new Color(1f, 0.6f, 0.1f, 1f);
                subheader.text = $"{mod.Info.Author}\n{mod.Info.Version}";
                continue;
            }

            // If this mod is registered, update the display name, description and icon
            title.text = entry.DisplayName;
            subheader.text = entry.Description;

            GameObject modIconObj = modObj.transform.Find("Icon").gameObject;
            if(entry.Icon is not null) modIconObj.GetComponent<Image>().sprite = entry.Icon;

            // If this entry has a config, create a config button
            if (entry.HasConfig)
            {
                // Add a button component to the icon object
                Button configButton = modIconObj.AddComponent<Button>();
                configButton.onClick.AddListener(() => ModMenu.ShowConfigPanel(entry));

                // Adjust the icon's colors on hover
                var colors = configButton.colors;
                colors.normalColor = Color.white;
                colors.highlightedColor = new Color(0.7f, 0.7f, 0.7f, 1f);
                colors.fadeDuration = 0.1f;
                configButton.colors = colors;

                // Create a config icon that appears when you hover over the mod entry
                GameObject configIconObj = GameObject.Instantiate(modIconObj, modIconObj.transform.parent);
                RectTransform configIconRect = configIconObj.GetComponent<RectTransform>();
                configIconRect.sizeDelta = new Vector2(150, 150);
                configIconRect.anchoredPosition = new Vector2(25, 0);
                Image configIconImg = configIconObj.GetComponent<Image>();
                configIconImg.sprite = configIconSprite;
                configIconImg.raycastTarget = false;
                configIconObj.AddComponent<CanvasGroup>().alpha = 0f;

                EventTrigger trigger = modIconObj.AddComponent<EventTrigger>();
                trigger.triggers = new Il2CppSystem.Collections.Generic.List<EventTrigger.Entry>();

                // On pointer enter trigger - fade in config icon
                EventTrigger.Entry pointerEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
                pointerEnter.callback.AddListener(_ => UIHelper.FadeUIAlpha(configIconRect, 1f, 0.25f));
                trigger.triggers.Add(pointerEnter);

                // On pointer exit trigger - fade out config icon
                EventTrigger.Entry pointerExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
                pointerExit.callback.AddListener(_ => UIHelper.FadeUIAlpha(configIconRect, 0f, 0.25f));
                trigger.triggers.Add(pointerExit);
            }
        }
    }

    


    /// <summary>
    /// Sets the title text of the container screen
    /// </summary>
    private void SetHeaderText(string text)
    {
        container.parent.FindComponent<TextMeshProUGUI>("Header/Center/HeaderRock").text = text;
        container.parent.FindComponent<TextMeshProUGUI>("Header/Center/HeaderRockTop").text = text;
        container.parent.FindComponent<TextMeshProUGUI>("Header/Center/HeaderRockBottom").text = text;
    }

    /// <summary>
    /// Replaces the achievement entries with the mod entires, or vice versa.
    /// </summary>
    /// <param name="enableMods">Whether to show the mod list or the achievement list.</param>
    private void SetEntriesEnabled(bool enableMods)
    {
        // Change the state of all mod entries
        foreach(var mod in entryObjects)
            mod.SetActive(enableMods);

        // Change the state of all achievement entries
        for (int i = 0; i < container.childCount; i++)
        {
            GameObject achievement = container.GetChild(i).gameObject;

            if (achievement.name.StartsWith("P_") && achievement.name.EndsWith("(Clone)"))
                achievement.SetActive(!enableMods);
        }
    }

    /// <summary>
    /// Opens the mod list and updates the UI.
    /// </summary>
    private void OpenModList()
    {
        SetHeaderText("Mods");
        SetEntriesEnabled(true);

        PlayTransitionAnim();
        ModMenuOpen = true;
        ui.m_achievementsIsActive = true;
    }

    /// <summary>
    /// Triggers the animation that plays when the camera pans down to the container screen.
    /// </summary>
    private static void PlayTransitionAnim() => UIHelper.MainMenu.PlayAnimation("A_MainMenu_Achievements_In");
}