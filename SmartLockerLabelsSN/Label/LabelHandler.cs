using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartLockerLabelsSN.Label;

public static class LabelHandler
{
    private static readonly (Func<TechType, bool> Condition, StorageCategory Category)[] CategoryRules =
    {
        (Utility.IsEquipment, StorageCategory.Equipment),
        (Utility.IsPdaChip, StorageCategory.PdaChips),
        (Utility.IsVehicleModule, StorageCategory.VehicleModule),
        (Utility.IsPowercell, StorageCategory.Powercell),
        (Utility.IsBattery, StorageCategory.Battery),
        (Utility.IsDecoy, StorageCategory.Decoys),
        (Utility.IsReactorRod, StorageCategory.ReactorRods),
        (Utility.IsPrecursorKey, StorageCategory.PrecursorKeys), 
        (Utility.IsRawMaterial, StorageCategory.RawMaterial),
        (Utility.IsAdvancedMaterial, StorageCategory.AdvancedMaterial),
    };
    
    private static readonly (HashSet<StorageCategory> Categories, StorageCategory Result)[] CategoryCombinations =
    {
        (new HashSet<StorageCategory>{ StorageCategory.Equipment, StorageCategory.PdaChips }, StorageCategory.Equipment),
        (new HashSet<StorageCategory> { StorageCategory.RawMaterial, StorageCategory.AdvancedMaterial }, StorageCategory.MixedMaterial),
        (new HashSet<StorageCategory> { StorageCategory.PlantAir, StorageCategory.PlantAirSeed }, StorageCategory.PlantAir),
        (new HashSet<StorageCategory> { StorageCategory.PlantWater, StorageCategory.PlantWaterSeed }, StorageCategory.PlantWater),
        (new HashSet<StorageCategory> { StorageCategory.Poster, StorageCategory.Placeables }, StorageCategory.Deco),
        (new HashSet<StorageCategory> { StorageCategory.Decoys, StorageCategory.VehicleModule }, StorageCategory.VehicleModule),
        (new HashSet<StorageCategory> { StorageCategory.Battery, StorageCategory.Powercell, StorageCategory.ReactorRods }, StorageCategory.Power),
        (new HashSet<StorageCategory> { StorageCategory.Battery, StorageCategory.Powercell }, StorageCategory.Power),
    };

    private static StorageCategory GetStorageCategory(TechType techType, StorageContainer container)
    {
        if (Utility.IsFish(techType, container))
        {
            ErrorMessage.AddMessage("Fish");
            return StorageCategory.Fish;
        }
        
        if (Utility.IsPlaceable(techType, container))
        {
            bool isPoster = Utility.IsPoster(techType, container);
            
            ErrorMessage.AddMessage($"Placeable, Poster: {isPoster}");
            
            return isPoster ? StorageCategory.Poster : StorageCategory.Placeables;
        }

        if (Utility.IsTool(techType, container))
        {
            ErrorMessage.AddMessage("Tool");
            return StorageCategory.Tools;
        }
        
        // Plants
        if (TechData.GetBackgroundType(techType) == CraftData.BackgroundType.PlantAir)
        {
            // ErrorMessage.AddMessage("PlantAir");
            return StorageCategory.PlantAir;
        }
        if (TechData.GetBackgroundType(techType) == CraftData.BackgroundType.PlantAirSeed)
        {
            // ErrorMessage.AddMessage("PlantAirSeed");
            return StorageCategory.PlantAirSeed;
        }
        if (TechData.GetBackgroundType(techType) == CraftData.BackgroundType.PlantWaterSeed)
        {
            // ErrorMessage.AddMessage("PlantWaterSeed");
            return StorageCategory.PlantWaterSeed;
        }
        if (TechData.GetBackgroundType(techType) == CraftData.BackgroundType.PlantWater)
        {
            // ErrorMessage.AddMessage("PlantWater");
            return StorageCategory.PlantWater;
        }
        
        if (Utility.IsEatable(techType, container))
            return StorageCategory.Food;
        
        foreach (var rule in CategoryRules)
        {
            if (rule.Condition(techType))
                return rule.Category;
        }

        return StorageCategory.Unknown;
    }

    private static StorageLabel AnalyzeContents(StorageContainer container)
    {
        var items = container.container.GetItemTypes();

        if (items.Count == 0)
            return StorageLabel.Empty;

        if (items.Count == 1)
            return StorageLabel.ForItem(items[0]);

        var categories = items
            .Select(item => GetStorageCategory(item, container))
            .ToHashSet();

        if (categories.Count == 1)
            return StorageLabel.ForCategory(categories.First());

        // if multiple categories, decide which should be displayed
        foreach (var (combination, result) in CategoryCombinations)
        {
            if (categories.SetEquals(combination))
            {
                Plugin.Logger.LogInfo($"Matched combination: {string.Join(", ", combination)} -> {result}");
                return StorageLabel.ForCategory(result);
            }
        }
        
        return StorageLabel.ForCategory(StorageCategory.Resource);
    }

    private static string Localize(StorageLabel label)
    {
        switch (label.Kind)
        {
            case StorageLabelKind.Empty:
                return Language.main.Get("Empty");
            
            case StorageLabelKind.Item:
                return Language.main.Get(label.Type.ToString());
            
            case StorageLabelKind.Category:
                return Language.main.Get($"SL_{label.Category.ToString()}");
        }
        
        return "Unknown";
    }
    
    public static TechType GetStorageType(StorageContainer container)
    {
        return container.gameObject.GetComponentInParent<TechTag>().type;
    }
    
    public static string GetStorageLabel(StorageContainer container)
    {
        var label = AnalyzeContents(container);
        return Localize(label);
    }
}