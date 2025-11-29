using BloomEngine.Menu;
using BloomEngine.Utilities;
using Il2CppReloaded.Input;
using Il2CppTekly.Localizations;
using Il2CppTekly.PanelViews;
using Il2CppTMPro;
using MelonLoader;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BloomEngine.Config;

public class ConfigPanel
{
    const int PropertiesPerPage = 7;

    public ModEntry Mod { get; set; }

    private readonly List<RectTransform> pages = new List<RectTransform>();
    private readonly Dictionary<IConfigProperty, GameObject> inputFields = new Dictionary<IConfigProperty, GameObject>();

    private readonly PanelView panelView;
    private readonly Transform window;

    private GameObject pageCountLabel;
    private GameObject pageBackButton;
    private GameObject pageNextButton;

    private readonly int totalPageCount = 0;
    private int pageIndex = 0;

    internal ConfigPanel(PanelView panel, ModEntry mod)
    {
        if (mod.Config is null || mod.Config.Properties.IsNullOrEmpty())
        {
            Melon<BloomEngineMod>.Logger.Warning($"Failed to setup config panel for {mod.DisplayName}: No config or config properties registered.");
            return;
        }

        Mod = mod;

        panelView = panel;
        panelView.m_id = $"modConfig_{mod.Mod.Info.Name}";
        panelView.gameObject.name = $"P_ModConfig_{mod.DisplayName.Replace(" ", "")}";

        // Resize window
        window = panelView.transform.Find("Canvas/Layout/Center/Window");
        var windowRect = window.GetComponent<RectTransform>();
        windowRect.sizeDelta = new Vector2(2200, 0);
        windowRect.anchoredPosition = new Vector2(0, -80);

        // Add background click blocker
        GameObject.Instantiate(UIHelper.MainMenu.transform.parent.Find("P_UsersPanel/Canvas/P_Scrim").gameObject, window.parent).transform.SetAsFirstSibling();

        // Setup panel buttons
        var buttons = window.Find("Buttons").GetComponentsInChildren<Button>();
        SetupApplyButton(buttons[0]);
        SetupCancelButton(buttons[1]);

        // Change header text
        window.Find("HeaderText").GetComponent<TextMeshProUGUI>().text = $"{mod.DisplayName}";
        window.Find("SubheadingText").GetComponent<TextMeshProUGUI>().text = " ";

        // Split properties into pages
        totalPageCount = (int)Math.Ceiling((double)mod.Config.Properties.Count / PropertiesPerPage);
        var pagesData = mod.Config.Properties.Chunk(PropertiesPerPage).ToList();

        // Create inputs for each property on each page
        for (int i = 0; i < pagesData.Count; i++)
        {
            var page = CreateLayout(window.GetComponent<RectTransform>(), $"LayoutPage_{i}");
            pages.Add(page);

            RectTransform labelColumn = page.GetChild(0).GetComponent<RectTransform>();
            RectTransform fieldColumn = page.GetChild(1).GetComponent<RectTransform>();

            foreach (var property in pagesData[i])
            {
                CreateInputLabel(property, labelColumn);
                inputFields[property] = CreateInputField(property, fieldColumn);
            }
        }

        CreatePageControls(window.GetComponent<RectTransform>());

        // Destroy all localisers
        foreach (var localiser in panelView.GetComponentsInChildren<TextLocalizer>(true))
            GameObject.Destroy(localiser);
    }

    /// <summary>
    /// Displays the panel and populates its input fields with the current values of the associated properties.
    /// </summary>
    public void ShowPanel()
    {
        // Populate input fields with current property values
        foreach (var input in inputFields)
        {
            var field = input.Value.GetComponent<ReloadedInputField>();
            var property = input.Key;

            if (property is not null)
                field.text = property.GetValue().ToString();
        }

        panelView.gameObject.SetActive(true);
    }

    public void HidePanel()
    {
        panelView.gameObject.SetActive(false);
    }

    private void SetupApplyButton(Button button)
    {
        // Update name and text
        button.name = "P_ConfigButton_Apply";
        button.GetComponentInChildren<TextMeshProUGUI>().SetText("Apply");

        // Remove garbage components
        GameObject.Destroy(button.GetComponent<Il2CppReloaded.ExitGame>());
        GameObject.Destroy(button.GetComponent<TextLocalizer>());

        // Apply all input fields on click
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener((UnityAction)(() =>
        {
            ModMenu.Log($"Updating all config properties of {Mod.DisplayName}");

            foreach (var input in inputFields)
                ApplyInputField(input.Value.GetComponent<ReloadedInputField>(), input.Key);

            ModMenu.HideConfigPanel();
        }));
    }

    private static void SetupCancelButton(Button button)
    {
        // Update name and text
        button.name = "P_ConfigButton_Cancel";
        button.GetComponentInChildren<TextMeshProUGUI>().SetText("Cancel");

        // Remove garbage components
        UnityEngine.Object.Destroy(button.GetComponent<TextLocalizer>());

        // Hide config panel on click
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener((UnityAction)ModMenu.HideConfigPanel);
    }

    private void CreatePageControls(RectTransform parent)
    {
        if(Mod.Config.Properties.Count <= PropertiesPerPage)
            return;

        var pageControls = new GameObject("PageControls");
        var pageControlsRect = pageControls.AddComponent<RectTransform>();
        pageControlsRect.SetParent(parent);

        var horizontalLayout = pageControls.AddComponent<HorizontalLayoutGroup>();
        horizontalLayout.childAlignment = TextAnchor.MiddleCenter;

        // Create previous page button
        pageBackButton = GameObject.Instantiate(UIHelper.MainMenu.transform.parent.FindChild("P_HelpPanel/Canvas/Layout/Center/Arrows/NavArrow_Back").gameObject, pageControlsRect);
        pageBackButton.AddComponent<LayoutElement>().preferredHeight = 200;
        var backAspect = pageBackButton.AddComponent<AspectRatioFitter>();
        backAspect.aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;
        backAspect.aspectRatio = pageBackButton.GetComponent<RectTransform>().sizeDelta.x / pageBackButton.GetComponent<RectTransform>().sizeDelta.y;
        var backButton = pageBackButton.GetComponent<Button>();
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener((UnityAction)(() => SetPageIndex(pageIndex - 1)));

        // Create page count label
        pageCountLabel = GameObject.Instantiate(UIHelper.MainMenu.transform.parent.FindChild("P_HelpPanel/Canvas/Layout/Center/PageCount").gameObject, pageControlsRect);

        // Create next page button
        pageNextButton = GameObject.Instantiate(UIHelper.MainMenu.transform.parent.FindChild("P_HelpPanel/Canvas/Layout/Center/Arrows/NavArrow_Next").gameObject, pageControlsRect);
        pageNextButton.AddComponent<LayoutElement>().preferredHeight = 200;
        var nextAspect = pageNextButton.AddComponent<AspectRatioFitter>();
        nextAspect.aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;
        nextAspect.aspectRatio = pageNextButton.GetComponent<RectTransform>().sizeDelta.x / pageNextButton.GetComponent<RectTransform>().sizeDelta.y;
        var nextButton = pageNextButton.GetComponent<Button>();
        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener((UnityAction)(() => SetPageIndex(pageIndex + 1)));

        SetPageIndex(0);
    }

    private void SetPageIndex(int index)
    {
        if (Mod.Config.Properties.Count <= PropertiesPerPage)
            return;

        pageIndex = Mathf.Clamp(index, 0, totalPageCount - 1);
        pageCountLabel.transform.FindChild("Count").GetComponent<TextMeshProUGUI>().text = $"{pageIndex + 1}/{totalPageCount}";

        for (int i = 0; i < pages.Count; i++)
            pages[i].gameObject.SetActive(i == index);

        pageBackButton.GetComponent<Button>().interactable = pageIndex > 0;
        //pageNextButton.GetComponentInChildren<Image>().

        pageNextButton.GetComponent<Button>().interactable = pageIndex < totalPageCount - 1;
    }

    private static RectTransform CreateLayout(RectTransform parent, string name)
    {
        GameObject layoutObj = new GameObject(name);
        var layoutTransform = layoutObj.AddComponent<RectTransform>();
        layoutTransform.SetParent(parent, false);
        layoutTransform.anchorMin = new Vector2(0, 1);
        layoutTransform.anchorMax = new Vector2(1, 1);
        layoutTransform.pivot = new Vector2(0.5f, 1);
        layoutTransform.offsetMin = Vector2.zero;
        layoutTransform.offsetMax = Vector2.zero;
        var layoutGroup = layoutObj.AddComponent<HorizontalLayoutGroup>();
        layoutGroup.spacing = 10;
        layoutGroup.childControlWidth = true;
        layoutGroup.childControlHeight = true;

        CreateColumn(layoutTransform, "LabelColumn");
        CreateColumn(layoutTransform, "FieldColumn");

        return layoutTransform;
    }

    private static void CreateColumn(RectTransform parent, string name)
    {
        GameObject column = new GameObject(name);
        var rect = column.AddComponent<RectTransform>();
        rect.SetParent(parent, false);

        VerticalLayoutGroup layout = column.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 10;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = true;

        // Each column takes up half the available width
        var layoutElement = rect.gameObject.AddComponent<LayoutElement>();
        layoutElement.flexibleWidth = 1;
    }

    private void CreateInputLabel(IConfigProperty property, RectTransform parent)
    {
        GameObject obj = UnityEngine.Object.Instantiate(window.Find("SubheadingText").gameObject, parent);
        obj.name = $"PropertyLabel_{property.Name}";
        obj.SetActive(true);

        var text = obj.GetComponent<TextMeshProUGUI>();
        text.text = property.Name;
        text.overflowMode = TextOverflowModes.Ellipsis;
        text.alignment = TextAlignmentOptions.Left;
    }

    private static GameObject CreateInputField(IConfigProperty property, RectTransform parent)
    {
        GameObject inputObj = null;
        string name = $"PropertyInput_{property.Name}";
        string typeName = property.ValueType.Name;

        // Basic string input
        if (property.ValueType == typeof(string))
        {
            inputObj = UIHelper.CreateTextField(name, parent, typeName, onDeselect: field => ApplyInputField(field, property));
            inputObj.GetComponent<ReloadedInputField>().m_Text = property.GetValue()?.ToString();
        }
        // Sanitised numeric input
        else if (TypeHelper.IsNumericType(property.ValueType))
        {
            inputObj = UIHelper.CreateTextField(name, parent, typeName, onTextChanged: field =>
            {
                string sanitised = TextHelper.SanitiseNumericInput(field.m_Text);
                field.m_Text = sanitised;
            }, onDeselect: field => ApplyInputField(field, property));

            inputObj.GetComponent<ReloadedInputField>().m_Text = property.GetValue()?.ToString();
        }

        return inputObj;
    }

    private static void ApplyInputField(ReloadedInputField field, IConfigProperty property)
    {
        // Filter input value
        object value = null;
        if (TypeHelper.IsNumericType(property.ValueType))
            value = TextHelper.ValidateNumericInput(field.m_Text, property.ValueType);
        else if (property.ValueType == typeof(string))
            value = field.m_Text;

        // Transform value
        value = property.TransformValue(value);

        // Validate and apply value
        if (!property.ValidateValue(value))
            value = property.GetValue();
        else property.SetValue(Convert.ChangeType(value, property.ValueType));

        field.text = value.ToString();
    }
}