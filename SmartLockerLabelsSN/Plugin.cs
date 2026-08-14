using BepInEx.Logging;
using BepInEx;
using HarmonyLib;
using Nautilus.Handlers;
using System.Reflection;

namespace SmartLockerLabelsSN;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.prototech.prototypesub", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("com.snmodding.nautilus")]
public class Plugin : BaseUnityPlugin
{
    internal new static ManualLogSource Logger { get; private set; }

    private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

    internal static readonly bool IsPrototypePossible = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.prototech.prototypesub");

    internal static Config config;
    
    private void Awake()
    {
        // set project-scoped logger instance
        Logger = base.Logger;
        
        config = OptionsPanelHandler.RegisterModOptions<Config>();
        
        LanguageHandler.RegisterLocalizationFolder();

        SaveData.main = SaveDataHandler.RegisterSaveDataCache<SaveData>();
        
        // register harmony patches, if there are any
        Harmony.CreateAndPatchAll(Assembly, $"{PluginInfo.PLUGIN_GUID}");
        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
    }
}