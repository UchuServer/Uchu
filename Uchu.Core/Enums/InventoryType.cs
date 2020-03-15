namespace Uchu.Core
{
    /// <summary>
    /// Inventory type enum
    /// </summary>
    /// <remarks>
    /// Based on the InventoryType Enum from https://docs.google.com/document/d/1V_yhtj91QG0VBfMnmD5zC44DXwCRqjbBN98HoXXC7qs/edit?pref=2&pli=1#
    /// </remarks>
    public enum InventoryType
    {
        None = -1,
        Items,
        VaultItems,
        Bricks,
        TemporaryItems = 4,
        Models,
        TemporaryModels,
        Behaviors,
        PropertyDeeds,
        VendorBuyback = 11,
        Hidden = 12,
        BrickDonations,
        VaultModels = 14,
        Invalid = 100
    }
}