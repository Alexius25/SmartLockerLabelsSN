// ReSharper disable InconsistentNaming

using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using SmartLockerLabelsSN.Label;
using UnityEngine;

namespace SmartLockerLabelsSN.Patches;

[HarmonyPatch(typeof(StorageContainer))]
public static class StorageContainerPatcher
{
    private static Dictionary<string, uGUI_SignInput> StorageLabelCache = new Dictionary<string, uGUI_SignInput>();
    
    [HarmonyPostfix]
    [HarmonyPatch(nameof(StorageContainer.OnClose))]
    public static void OnClosePostfix(StorageContainer __instance)
    {
        TechType storageType = LabelHandler.GetStorageType(__instance);

        if (storageType is TechType.Aquarium)
        {
            return;
        }
        
        if (storageType is TechType.Locker)
        {
            string lockerId = Utility.GetPrefabId(__instance.gameObject);
            
            if (SaveData.main.LockerLabelPair.TryGetValue(lockerId, out string signId))
            {
                if (StorageLabelCache.ContainsKey(lockerId))
                {
                    ErrorMessage("Found cached storage label");
                    ApplyLabel(__instance, StorageLabelCache[lockerId]);
                }
                else
                {
                    Sign[] signs = Object.FindObjectsOfType<Sign>();
                    foreach (Sign sign in signs)
                    {
                        PrefabIdentifier prefabIdentifier = sign.GetComponentInParent<PrefabIdentifier>();

                        if (prefabIdentifier.Id != signId)
                            continue;
                        
                        uGUI_SignInput signInput = sign.GetComponentInChildren<uGUI_SignInput>();
                        
                        if (signInput == null)
                            continue;
                        
                        StorageLabelCache[lockerId] = signInput;
                        ApplyLabel(__instance, signInput);
                        
                        ErrorMessage($"Found not cached, but paired storage label");
                        
                        break;
                    }
                }
            }
            else
            {
                ErrorMessage("Did not find paired storage label, pairing...");
                
                Sign[] signs = Object.FindObjectsOfType<Sign>();
                Sign nearest = Utility.GetNearestSign(signs, __instance.transform.position, maxDistance: Plugin.config.SignDiscoveryDistance);

                if (nearest == null)
                {
                    ErrorMessage("Did not find any sign nearby");
                    return;
                }
                
                uGUI_SignInput signInput = nearest.GetComponentInChildren<uGUI_SignInput>();
                
                if (signInput == null)
                {
                    ErrorMessage("Nearby sign has no uGUI_SignInput");
                    return;
                }
                
                string signId2 = nearest.GetComponentInParent<PrefabIdentifier>().Id;
                
                if (SaveData.main.LockerLabelPair.Values.Contains(signId2))
                {
                    ErrorMessage("Sign is already paired to another locker");
                    return;
                }

                ErrorMessage("Found unbound sign nearby");
                    
                SaveData.main.LockerLabelPair[lockerId] = signId2;
                StorageLabelCache[lockerId] = signInput;
                ApplyLabel(__instance, signInput);
                    
                ErrorMessage("Successfully paired sign to locker");
            }
        }

        if (storageType is TechType.SmallLocker or TechType.SmallStorage)
        {
            uGUI_SignInput signInput = storageType == TechType.SmallLocker
                ? __instance.gameObject.GetComponentInChildren<uGUI_SignInput>()
                : __instance.gameObject.transform.parent.gameObject.GetComponentInChildren<uGUI_SignInput>();

            ApplyLabel(__instance, signInput);
        }
    }

    private static void ApplyLabel(StorageContainer __instance, uGUI_SignInput signInput)
    {
        if (signInput != null)
        {
            string localized = LabelHandler.GetStorageLabel(__instance);
            signInput.text = localized;
            Plugin.Logger.LogInfo($"Localized storage label: {localized}");
        }
        else
        {
            ErrorMessage($"Storage label not found");
            Plugin.Logger.LogError($"Storage label not found");
        }
    }

    private static void ErrorMessage(string message)
    {
        if (Plugin.config.DisplayMessages)
        {
            global::ErrorMessage.AddMessage(message);
        }
        else
        {
            Plugin.Logger.LogInfo(message);
        }
    }
}