using System.Linq;
using HarmonyLib;

namespace AetharNet.Mods.ZumbiBlocks2.AccessGranted.Patches;

[HarmonyPatch(typeof(LootController))]
public static class LootControllerPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(LootController.Init))]
    [HarmonyPriority(Priority.Low)]
    public static void AddAllItemsToLootTable(LootController __instance)
    {
        // TODO: Move LootController modification features into a separate library
        // The following code is taken from the CustomItemFramework
        // https://github.com/nyawoi/CustomItemFramework/blob/17de348ea18e4fa93318c12b47f8fb4eaa20358e/CustomItemPlugin/Patches/LootControllerPatch.cs#L16
        
        var tier1 = __instance.lootDistro.tier[1];
        var tier2 = __instance.lootDistro.tier[2];

        // As of 2.1.0.5, the Tier 1 and Tier 2 loot tables point to the same component
        // Changes made to one will be made to the other since they're the same
        // Therefore, we'll create a new one to separate any changes
        // This starts with a check to see if they are identical
        // If not, then there might have been a game update or a mod has overwritten Tier 2 already
        if (tier1 == tier2)
        {
            var newTier2 = __instance.gameObject.AddComponent<TierLootDistribution>();

            newTier2.equipmentRarity = tier2.equipmentRarity;
            newTier2.equipment = tier2.equipment;
            newTier2.resources = tier2.resources;

            __instance.lootDistro.tier[2] = tier2 = newTier2;
        }
        
        
        // This is where the mod begins
        
        // We start off by retrieving the equipment and resources in Tier 2 as a List
        // This allows us to add new items
        var equipment = tier2.equipment.ToList();
        var resources = tier2.resources.ToList();

        // We iterate through every single item currently in the game
        foreach (var item in ItemsBase.instance.item)
        {
            // If the item cannot be spawned, skip processing
            if (item.simplePropPrefab == null) continue;
            // If the item already exists in the loot table, skip processing
            if (equipment.Exists(lootChance => lootChance.itemID == item.itemID)) continue;
            if (resources.Exists(lootChance => lootChance.itemID == item.itemID)) continue;
            
            // If the item is a resource, add it to resources; otherwise, add it to equipment
            // Probability is based on item tier: Tier 1 has 10, Tier 2 has 20, etc.
            // Most equipment has a probability of 10
            if (item is DatabaseConsumable)
            {
                resources.Add(new TierLootDistribution.LootChance
                {
                    itemID = item.itemID,
                    probability = ((int)item.tier + 1) * 10
                });
            }
            else
            {
                equipment.Add(new TierLootDistribution.LootChance
                {
                    itemID = item.itemID,
                    probability = ((int)item.tier + 1) * 10
                });
            }
        }

        // Finalize our changes by replacing the original entries
        tier2.equipment = equipment.ToArray();
        tier2.resources = resources.ToArray();
        // You're here for the weapons, aren't you? Double the odds, then!
        tier2.equipmentRarity *= 2;
    }
}
