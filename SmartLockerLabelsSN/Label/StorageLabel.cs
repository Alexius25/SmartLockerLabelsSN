namespace SmartLockerLabelsSN.Label;

public readonly struct StorageLabel
{
    public StorageLabelKind Kind { get; }
    public TechType? Type { get; }
    public StorageCategory? Category { get; }

    private StorageLabel(
        StorageLabelKind kind,
        TechType? type,
        StorageCategory? category)
    {
        Kind = kind;
        Type = type;
        Category = category;
    }
        
    public static StorageLabel Empty => 
        new(StorageLabelKind.Empty, null, null);
        
    public static StorageLabel ForItem(TechType techType) =>
        new (StorageLabelKind.Item, techType, null);
        
    public static StorageLabel ForCategory(StorageCategory category) =>
        new (StorageLabelKind.Category, null, category);

    public override string ToString()
    {
        return Kind switch
        {
            StorageLabelKind.Empty => "Empty",
            StorageLabelKind.Item => $"Item: {Type.ToString()}",
            StorageLabelKind.Category => $"Category: {Category.ToString()}",

            _ => "Unknown"
        };
    }
}
