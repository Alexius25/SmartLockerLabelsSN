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
        TechType storageType = LabelHandler.GetStorageType(__instance);

        if (storageType is TechType.Aquarium or TechType.Locker)
        {
            return;
        }

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