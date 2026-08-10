using System;
using UnityEngine;

namespace SmartLockerLabelsSN;

public static class Utility
{
    public static bool IsEquipment(TechType techType)
    {
        var type = TechData.GetEquipmentType(techType);
        return type is EquipmentType.Tank or EquipmentType.Head or EquipmentType.Body or EquipmentType.Gloves or EquipmentType.Foots or EquipmentType.Chip;
    }

    public static bool IsVehicleModule(TechType techType)
    {
        var type = TechData.GetEquipmentType(techType);
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

    public static bool IsPrecursorKey(TechType techType)
    {
        return Enum.TryParse<PrecursorKeyTerminal.PrecursorKeyType>(
            techType.ToString(),
            out _
        );
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

    private static GameObject GetItemGameObject(TechType techType, StorageContainer container)
    {
        var items = container.container.GetItems(techType);
        if (items.Count == 0)
            return null;
        
        GameObject gameObject = items[0].item.gameObject;
        return gameObject;
    }
}