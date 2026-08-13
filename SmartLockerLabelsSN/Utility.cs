using System.Collections.Generic;
using System;
using UnityEngine;

namespace SmartLockerLabelsSN;

public static class Utility
{
    public static bool IsEquipment(TechType techType)
    {
        EquipmentType type = TechData.GetEquipmentType(techType);
        return type is EquipmentType.Tank or EquipmentType.Head or EquipmentType.Body or EquipmentType.Gloves or EquipmentType.Foots;
    }

    public static bool IsPdaChip(TechType techType)
    {
        EquipmentType type = TechData.GetEquipmentType(techType);
        return type is EquipmentType.Chip;
    }

    public static bool IsVehicleModule(TechType techType)
    {
        EquipmentType type = TechData.GetEquipmentType(techType);
        return type is EquipmentType.CyclopsModule or EquipmentType.ExosuitArm or EquipmentType.ExosuitModule or EquipmentType.SeamothModule or EquipmentType.VehicleModule;
    }

    public static bool IsPowercell(TechType techType) => TechData.GetEquipmentType(techType) == EquipmentType.PowerCellCharger;

    public static bool IsBattery(TechType techType) => TechData.GetEquipmentType(techType) == EquipmentType.BatteryCharger;

    public static bool IsReactorRod(TechType techType) => TechData.GetEquipmentType(techType) == EquipmentType.NuclearReactor;

    public static bool IsDecoy(TechType techType) => TechData.GetEquipmentType(techType) == EquipmentType.DecoySlot;

    public static bool IsEatable(TechType techType, StorageContainer container)
    {
        GameObject gameObject = GetItemGameObject(techType, container);
        Eatable eatable = gameObject.GetComponent<Eatable>();

        if (eatable != null)
        {
            return true;
        }
        return false;
    }

    public static bool IsPlaceable(TechType techType, StorageContainer container)
    {
        GameObject gameObject = GetItemGameObject(techType, container);
        PlaceTool placeTool = gameObject.GetComponent<PlaceTool>();
        if (placeTool != null)
        {
            return true;
        }
        return false;
    }

    public static bool IsPrecursorKey(TechType techType, StorageContainer container)
    {
        bool baseGameKey = Enum.TryParse<PrecursorKeyTerminal.PrecursorKeyType>(techType.ToString(), out _);
        
        if (baseGameKey)
        {
            return true;
        }

        GameObject gameObject = GetItemGameObject(techType, container);
        InspectOnFirstPickup inspect =  gameObject.GetComponent<InspectOnFirstPickup>();

        // for compability with mods like The Prototype Expansion
        if (inspect != null && inspect.animParam == "holding_precursorkey")
        {
            return true;
        }

        return false;
    }

    public static bool IsRawMaterial(TechType techType)
    {
        // Special princess treatment for ✨ Princess Titanium ✨
        if (techType == TechType.Titanium)
        {
            return true;
        }

        if (!CraftTree.IsCraftable(techType))
        {
            return true;
        }
        
        return false;
    }

    public static bool IsAdvancedMaterial(TechType techType)
    {
        return CraftTree.IsCraftable(techType);
    }
    
    public static bool IsTool(TechType techType, StorageContainer container)
    {
        GameObject gameObject = GetItemGameObject(techType, container);
        Pickupable pickupable = gameObject.GetComponent<Pickupable>();
        
        EquipmentType equipmentType = TechData.GetEquipmentType(techType);
        
        return pickupable != null && equipmentType == EquipmentType.Hand;
    }

    public static bool IsPoster(TechType techType, StorageContainer container)
    {
        GameObject gameObject = GetItemGameObject(techType, container);
        PlaceTool placeTool = gameObject.GetComponent<PlaceTool>();

        bool isPoster = false;

        if (placeTool != null)
        {
            if (placeTool.allowedOnWalls
                && !placeTool.allowedOnCeiling
                && !placeTool.allowedOnGround
                && placeTool.allowedInBase
                && placeTool.allowedUnderwater
               )
            {
                isPoster = true;
            }
        }
        
        return isPoster;
    }

    public static bool IsFish(TechType techType, StorageContainer container)
    {
        if (IsPlant(techType, container)) return false;
        
        GameObject gameObject = GetItemGameObject(techType, container);
        LiveMixin liveMixin = gameObject.GetComponent<LiveMixin>();

        return liveMixin != null;
    }

    private static bool IsPlant(TechType techType, StorageContainer container)
    {
        if (TechData.GetBackgroundType(techType) == CraftData.BackgroundType.PlantAir)
            return true;
        if (TechData.GetBackgroundType(techType) == CraftData.BackgroundType.PlantAirSeed)
            return true;
        if (TechData.GetBackgroundType(techType) == CraftData.BackgroundType.PlantWaterSeed)
            return true;
        if (TechData.GetBackgroundType(techType) == CraftData.BackgroundType.PlantWater)
            return true;
        
        return false;
    }

    private static GameObject GetItemGameObject(TechType techType, StorageContainer container)
    {
        IList<InventoryItem> items = container.container.GetItems(techType);
        if (items.Count == 0)
            return null;
        
        GameObject gameObject = items[0].item.gameObject;
        return gameObject;
    }
}