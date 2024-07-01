using System;
using System.Collections.Generic;
using HarmonyLib;

namespace AetharNet.Mods.ZumbiBlocks2.AccessGranted.Patches;

[HarmonyPatch(typeof(LootCategories))]
public static class LootCategoriesPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(LootCategories.PrimaryGuns), MethodType.Getter)]
    public static void AddAllPrimaryGuns(LootCategory __result)
    {
        AddAllItemsOfType<DatabasePrimaryGun>(__result, chance: 2.0);
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(nameof(LootCategories.SecondaryGuns), MethodType.Getter)]
    public static void AddAllSecondaryGuns(LootCategory __result)
    {
        AddAllItemsOfType<DatabaseSecondaryGun>(__result, chance: 2.0);
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(nameof(LootCategories.Melee), MethodType.Getter)]
    public static void AddAllMelee(LootCategory __result)
    {
        AddAllItemsOfType<DatabaseMelee>(__result, chance: 2.0);
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(nameof(LootCategories.Food), MethodType.Getter)]
    public static void AddAllFood(LootCategory __result)
    {
        AddAllItemsOfType<DatabaseConsumable>(__result, chance: 2.0, filter: consumable => !consumable.ishealing);
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(nameof(LootCategories.Medicine), MethodType.Getter)]
    public static void AddAllMedicine(LootCategory __result)
    {
        AddAllItemsOfType<DatabaseConsumable>(__result, chance: 2.0, filter: consumable => consumable.ishealing);
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(nameof(LootCategories.Throwables), MethodType.Getter)]
    public static void AddAllThrowables(LootCategory __result)
    {
        AddAllItemsOfType<DatabaseThrowable>(__result, chance: 2.0);
    }

    private static void AddAllItemsOfType<T>(LootCategory lootCategory, double chance = 1.0, Func<T, bool> filter = null) where T : DatabaseItem
    {
        // Retrieve private fields to get and set values
        var fieldLoots = AccessTools.Field(typeof(LootCategory), "loots");
        var fieldItem = AccessTools.Field(typeof(ItemsBase), "item");

        // Retrieve field values for loot table and item list
        var lootTable = fieldLoots.GetValue(lootCategory) as List<LootChance>;
        var itemsList = fieldItem.GetValue(ItemsBase.instance) as List<DatabaseItem>;

        // In case the game renames the fields, check if they exist
        if (lootTable == null || itemsList == null)
        {
            AccessGranted.Logger.LogWarning("Failed to patch loot table: fields do not exist");
            return;
        }
        
        // Add all primary guns into the loot table
        foreach (var item in itemsList)
        {
            // Filter out items that do not match type, cannot be spawned
            if (item is not T fullItemType || item.simplePropPrefab == null) continue;
            // If the item fails to pass the provided filter, ignore it
            if (filter != null && !filter.Invoke(fullItemType)) continue;
            // Ignore items that already have entries
            if (lootTable.Exists(lootChance => lootChance.itemID == item.itemID)) continue;
            // Add item to the loot table
            lootTable.Add(new LootChance(item.itemID, chance));
        }
        
        // Set loot table to modified table
        fieldLoots.SetValue(lootCategory, lootTable);
    }
}
