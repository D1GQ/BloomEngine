using BloomEngine.Config;
using MelonLoader;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using UnityEngine;

namespace BloomEngine.Menu;

/// <summary>
/// A static class responsible for registering mod entries and adding them to the mod menu.
/// </summary>
public static class ModMenu
{
    private static ConcurrentDictionary<MelonMod, ModEntry> entries = new ConcurrentDictionary<MelonMod, ModEntry>();
    private static Dictionary<ModEntry, ConfigPanel> configs = new Dictionary<ModEntry, ConfigPanel>();
    private static ConfigPanel currentConfigPanel = null;

    /// <summary>
    /// A read-only dictionary of all registered mod entries in the Mod Menu, indexed by their associated MelonMod.
    /// </summary>
    public static ReadOnlyDictionary<MelonMod, ModEntry> Entries => new ReadOnlyDictionary<MelonMod, ModEntry>(entries);

    /// <summary>
    /// Event invoked when a mod is registered to the Mod Menu
    /// </summary>
    public static event Action<ModEntry> OnModRegistered;

    /// <summary>
    /// Creates a new mod entry which can be customised and added with <see cref="ModEntry.Register"/>.
    /// </summary>
    /// <param name="mod">The mod this entry belongs to.</param>
    /// <param name="displayName"></param>
    /// <returns></returns>
    public static ModEntry CreateEntry(MelonMod mod)
    {
        if(entries.ContainsKey(mod))
        {
            Log($"Cannot create an entry for the mod {mod.Info.Name} since one has already been created.");
            return null;
        }

        return new ModEntry(mod);
    }

    internal static void RegisterConfigPanel(ConfigPanel panel) => configs[panel.Mod] = panel;

    public static void ShowConfigPanel(ModEntry mod)
    {
        if (currentConfigPanel is not null)
            return;

        if (configs.TryGetValue(mod, out var panel))
        {
            panel.ShowPanel();
            currentConfigPanel = panel;
        }
        else Log($"Attempted to open mod config panel for {mod.DisplayName} with no config registered.", LogType.Warning);
    }

    public static void HideConfigPanel()
    {
        if (currentConfigPanel is null)
            return;

        currentConfigPanel.HidePanel();
        currentConfigPanel = null;
    }

    /// <summary>
    /// Registers a mod entry with the mod menu and invoked the <see cref="OnModRegistered"/> event.
    /// </summary>
    internal static void Register(ModEntry entry)
    {
        entries[entry.Mod] = entry;

        if (OnModRegistered is not null)
            OnModRegistered.Invoke(entry);

        if (!entry.HasConfig)
        {
            Log($"Successfully registered {entry.DisplayName} with the mod menu.");
            return;
        }

        // List all registered input fields
        ModMenu.Log($"Successfully registered {entry.DisplayName} with {entry.ConfigInputFields.Count} config input field{(entry.ConfigInputFields.Count > 1 ? "s" : "")}.");
        foreach (var field in entry.ConfigInputFields)
            ModMenu.Log($"    - {field.Name} ({field.ValueType.ToString()})");
    }

    /// <summary>
    /// Logs a message to the MelonLoader console with the ModMenu prefix.
    /// Uses Unity's LogType enum because I can't be bothered to make my own XD.
    /// Defaults to Log, can also use Warning or Error.
    /// </summary>
    internal static void Log(string text, LogType type = LogType.Log)
    {
        switch (type)
        {
            case LogType.Warning:
                Melon<BloomEngineMod>.Logger.Warning($"[ModMenu] {text}");
                break;
            case LogType.Error:
                Melon<BloomEngineMod>.Logger.Error($"[ModMenu] {text}");
                break;
            default:
                Melon<BloomEngineMod>.Logger.Msg($"[ModMenu] {text}");
                break;
        }
    }
}
