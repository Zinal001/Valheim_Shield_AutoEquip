using BepInEx;
using System.Linq;

namespace tech.zinals.valheim.shieldautoequip;

[BepInPlugin("tech.zinals.valheim.shieldautoequip", "Shield Auto-Equip", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
    internal static BepInEx.Logging.ManualLogSource PluginLogger { get; private set; }

    private BepInEx.Configuration.ConfigEntry<string> _DetectedWeapons;
    private BepInEx.Configuration.ConfigEntry<string> _EquippableShields;

    internal static string[] DetectedWeapons { get; private set; }
    internal static string[] EquippableShields { get; private set; }

    private void Awake()
    {
        PluginLogger = Logger;

        SetupConfig();
        Patches.Patch();

        Logger.LogInfo($"Plugin Shield Auto-Equip (1.0.0) is loaded!");
    }

    #region Configuration
    private void SetupConfig()
    {
        _DetectedWeapons = Config.Bind("Common", "Detected Weapons", "", "Weapons/Tools that should equip a shield. Separated by comma (,). Leave empty to allow all one-handed weapons.\nExample: MaceBronze, KnifeFlint, Club");
        _EquippableShields = Config.Bind("Common", "Equippable Shields", "", "Allowed shield types to auto-equip. Separated by comma (,). Leave empty to allow all shields.\nExample: ShieldWood, ShieldBronzeBuckler, ShieldBanded");

        Config.ConfigReloaded += Config_ConfigReloaded;
        Config_ConfigReloaded(null, null);
    }

    private void Config_ConfigReloaded(object sender, System.EventArgs e)
    {
        if (string.IsNullOrEmpty(_DetectedWeapons.Value))
            DetectedWeapons = new string[0];
        else
            DetectedWeapons = _DetectedWeapons.Value.Split(',').Select(s => s.Trim()).ToArray();

        if (string.IsNullOrEmpty(_EquippableShields.Value))
            EquippableShields = new string[0];
        else
            EquippableShields = _EquippableShields.Value.Split(',').Select(s => s.Trim()).ToArray();
    }
    #endregion
}
