// ReSharper disable InconsistentNaming
using HarmonyLib;
using SmartLockerLabelsSN.Label;
using UnityEngine;

namespace SmartLockerLabelsSN.Patches;

[HarmonyPatch(typeof(StorageContainer))]
public static class StorageContainerPatcher
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(StorageContainer.OnClose))]
    public static void OnClosePostfix(StorageContainer __instance)
    {
        TechType storageType = LabelHandler.GetStorageType(__instance);

        if (storageType is TechType.Aquarium or TechType.Locker)
        {
            return;
        }

        /* Janky af
        if (storageType is TechType.Locker)
        {
            Sign[] signs = GameObject.FindObjectsOfType<Sign>();
            Sign nearest = Utility.GetNearestSign(signs, __instance.transform.position);

            if (nearest == null)
            {
                Plugin.Logger.LogError("Could not find nearest sign");
                return;
            }
            
            uGUI_SignInput signInput = nearest.gameObject.GetComponentInChildren<uGUI_SignInput>();

            if (signInput != null)
            {
                string localized = LabelHandler.GetStorageLabel(__instance);
                signInput.text = localized;
                Plugin.Logger.LogInfo($"Localized storage label: {localized}");
            }
            else
            {
                ErrorMessage.AddError($"Storage label not found");
                Plugin.Logger.LogError($"Storage label not found");
            }
        }
        */

        if (storageType is TechType.SmallLocker or TechType.SmallStorage)
        {
            uGUI_SignInput signInput = storageType == TechType.SmallLocker
                ? __instance.gameObject.GetComponentInChildren<uGUI_SignInput>()
                : __instance.gameObject.transform.parent.gameObject.GetComponentInChildren<uGUI_SignInput>();

            if (signInput != null)
            {
                string localized = LabelHandler.GetStorageLabel(__instance);
                signInput.text = localized;
                Plugin.Logger.LogInfo($"Localized storage label: {localized}; Storage type: {storageType}");
            }
            else
            {
                ErrorMessage.AddError($"Storage label not found");
                Plugin.Logger.LogError($"Storage label not found");
            }
        }
    }
}