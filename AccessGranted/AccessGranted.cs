using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace AetharNet.Mods.ZumbiBlocks2.AccessGranted;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
public class AccessGranted : BaseUnityPlugin
{
    public const string PluginGUID = "AetharNet.Mods.ZumbiBlocks2.AccessGranted";
    public const string PluginAuthor = "awoi";
    public const string PluginName = "AccessGranted";
    public const string PluginVersion = "0.1.0";

    internal new static ManualLogSource Logger;
    
    private void Awake()
    {
        Logger = base.Logger;

        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PluginGUID);
    }
}
