using HarmonyLib;

namespace AetharNet.Mods.ZumbiBlocks2.AccessGranted.Patches;

[HarmonyPatch(typeof(LootFilter))]
public static class LootFilterPatch
{
    [HarmonyPostfix]
    [HarmonyPatch("FilterChanceByTier")]
    public static void IgnoreTierFilter(ref double __result, double originalChance)
    {
        // The game implements a tier-bias system, in which it penalizes
        // items whose tier does not match the requested area's.
        // For example, the Police Department (Tier 2 area) will decrease
        // the odds of Tier 1 and Tier 3 items from spawning by 95%,
        // and Tier 4 items by 99%. Tier 5 items have no chance of spawning.
        // 
        // This patch ignores the mismatched tier penalty, allowing all items
        // to maintain their original spawn chance, regardless of tier.
        __result = originalChance;
    }
}
