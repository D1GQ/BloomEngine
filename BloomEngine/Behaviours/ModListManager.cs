using Il2CppReloaded.UI;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.Events;

namespace BloomEngine.Behaviours;

public class ModListManager : MonoBehaviour
{
    private TMP_FontAsset font;
    private Transform achievementsMenu;
    private UnityEngine.UI.Button achievementsButton;

    public void Start()
    {
        font = transform.parent.Find("Layout").Find("Center").Find("Main").Find("AccountSign").Find("SignTop").Find("NameLabel").GetComponent<TextMeshProUGUI>().font;
        achievementsMenu = transform.parent.Find("Layout").Find("Center").Find("Achievements").Find("ScrollView").Find("Viewport").Find("Content").Find("Achievements");
        achievementsButton = transform.parent.Find("Layout").Find("Center").Find("Main").Find("BG_Tree").Find("AchievementsButton").GetComponent<UnityEngine.UI.Button>();

        //CreateModsButton();
    }

    private void CreateModsButton()
    {
        var obj = new GameObject("ModsButton");
        obj.transform.SetParent(transform, false);

        UnityEngine.UI.Button modsButton = obj.AddComponent<UnityEngine.UI.Button>();

        // Position the modsButton in the bottom left corner
        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.pivot = new Vector2(0, 0);
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(0, 0);
        rect.anchoredPosition = new Vector2(25, 25);
        rect.sizeDelta = new Vector2(160, 30);

        // Add a label that says "Mods"
        GameObject text = new GameObject("Text");
        text.transform.SetParent(obj.transform);
        TextMeshProUGUI buttonText = obj.AddComponent<TextMeshProUGUI>();
        buttonText.text = "Mods";
        buttonText.font = font;
        buttonText.alignment = TextAlignmentOptions.Center;

        modsButton.onClick.AddListener((UnityAction)ShowModList);
        achievementsButton.onClick.AddListener((UnityAction)ShowAchievementList);
    }

    private void ShowModList()
    {
        BloomEnginePlugin.Logger.Msg("Opening the Mods list.");

        SetHeaderText("Mods");
        ReplaceWithModList(true);
        PanScreenDown();
    }

    private void ShowAchievementList()
    {
        SetHeaderText("Achievements");
        ReplaceWithModList(false);
    }

    /// <summary>
    /// Sets the title text of the achievements screen
    /// </summary>
    private void SetHeaderText(string text)
    {
        achievementsMenu.Find("HeaderRock").GetComponent<TextMeshProUGUI>().text = text;
        achievementsMenu.Find("HeaderRockTop").GetComponent<TextMeshProUGUI>().text = text;
        achievementsMenu.Find("HeaderRockBottom").GetComponent<TextMeshProUGUI>().text = text;
    }

    /// <summary>
    /// Replaces all the achievements with the loaded mods
    /// </summary>
    /// <param name="show">Whether to show the mod list of to return the achievement list</param>
    private void ReplaceWithModList(bool showMods)
    {
        for (int i = 0; i < achievementsMenu.childCount; i++)
        {
            achievementsMenu.GetChild(i);
        }

        foreach (var achievement in achievementsMenu.GetComponentsInChildren<Transform>())
        {
            // Hide all achievements if showing mods, otherwise show only cloned achievements
            if (showMods)
                achievement.gameObject.SetActive(false);
            else if (achievement.name.EndsWith("(Clone)"))
                achievement.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Opens the achievement screen below the main menu
    /// </summary>
    private void PanScreenDown()
    {
        transform.parent.parent.GetComponent<MainMenuPanelView>().PlayAnimation("A_MainMenu_Achievements_In");
    }
}
