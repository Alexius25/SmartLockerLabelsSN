using System.Reflection;
using Nautilus.Handlers;

namespace SmartLockerLabelsSN;

public static class CompatibilityManager
{
    public static bool IsPrototypeItem(TechType techType)
    {
        if (!Plugin.IsPrototypePossible) return false;
        
        if (EnumHandler.TryGetOwnerAssembly(techType, out Assembly assembly))
        {
            string assemblyName = assembly.GetName().Name;

            if (string.IsNullOrEmpty(assemblyName))
                return false;
            
            return assemblyName == "PrototypeSubMod";
        }
        
        return false;
    }
}