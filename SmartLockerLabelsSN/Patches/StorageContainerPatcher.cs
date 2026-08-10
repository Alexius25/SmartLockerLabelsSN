// ReSharper disable InconsistentNaming

using HarmonyLib;
using SmartLockerLabelsSN.Label;

namespace SmartLockerLabelsSN.Patches;

[HarmonyPatch(typeof(StorageContainer))]
public static class StorageContainerPatcher
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(StorageContainer.OnClose))]
    public static void OnClosePostfix(StorageContainer __instance)
    {
        var storageType = LabelHandler.GetStorageType(__instance);

        if (storageType == TechType.Aquarium)
        {
            // ErrorMessage.AddDebug("Closed Aquarium");
            return;
        }

        if (storageType == TechType.Locker)
        {
            // maybe support in the future
            // ErrorMessage.AddDebug("Closed Locker"); 
            return;
        }

        if (storageType is TechType.SmallLocker or TechType.SmallStorage)
        {
            ErrorMessage.AddDebug("Closed supported storage");
            ErrorMessage.AddDebug($"Would be label: {LabelHandler.GetStorageLabel(__instance)}");

            var signInput = storageType == TechType.SmallLocker
                ? __instance.gameObject.GetComponentInChildren<uGUI_SignInput>()
                : __instance.gameObject.transform.parent.gameObject.GetComponentInChildren<uGUI_SignInput>();

            if (signInput != null)
            {
                signInput.text = LabelHandler.GetStorageLabel(__instance);
            }
        }
    }
}