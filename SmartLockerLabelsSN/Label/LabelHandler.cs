using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartLockerLabelsSN.Label;

public static class LabelHandler
{
    private static readonly (Func<TechType, bool> Condition, StorageCategory Category)[] CategoryRules =
    {
        (Utility.IsEquipment, StorageCategory.Equipment),
        (Utility.IsVehicleModule, StorageCategory.VehicleModule),
        (Utility.IsPowercell, StorageCategory.Powercell),
        (Utility.IsBattery, StorageCategory.Battery),
        (Utility.IsDecoy, StorageCategory.Decoys),
        (Utility.IsReactorRod, StorageCategory.ReactorRods),
        (Utility.IsPrecursorKey, StorageCategory.PrecursorKeys), 
        (Utility.IsRawMaterial, StorageCategory.RawMaterial)
    };
    
    private static readonly Dictionary<TechType, StorageCategory> SpecialItems = new()
    {
    };
    
    private static StorageCategory GetStorageCategory(TechType techType, StorageContainer container)
    {
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

        return SpecialItems.TryGetValue(techType, out StorageCategory category) 
            ? category 
            : StorageCategory.Unknown;
    }

    private static StorageLabel AnalyzeContents(StorageContainer container)
    {
        var items = container.container.GetItemTypes();
        
        if (items.Count == 0)
            return StorageLabel.Empty;
        
        if (items.Count == 1)
        {
            return StorageLabel.ForItem(items[0]);
        }
        
        var category = items
            .Select(item => GetStorageCategory(item, container))
            .Distinct()
            .ToList();

        if (category.Count == 1 &&
            category[0] != StorageCategory.Unknown)
        {
            return StorageLabel.ForCategory(category[0]);
        }
        
        // When we don't know what shit is in the Locker
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