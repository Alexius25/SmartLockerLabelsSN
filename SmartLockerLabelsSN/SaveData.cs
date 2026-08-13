using System.Collections.Generic;
using Nautilus.Json;

namespace SmartLockerLabelsSN;

public class SaveData : SaveDataCache
{
    public Dictionary<string, string> LockerLabelPair =  new Dictionary<string, string>();

    public static SaveData main;
}