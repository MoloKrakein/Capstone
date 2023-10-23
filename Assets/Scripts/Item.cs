using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable/Item", order = 0)]
public class InventoryItem : ScriptableObject {
   public string ItemName;
    public int ItemPower;
    public ItemType itemType;
    public int ItemPrice;
    public int ItemQuantity;
    public string ItemDescription;
    public Sprite ItemSprite;
    public InventoryItem(string name, int power, ItemType type, int price, int quantity, string description, Sprite sprite)
    {
        ItemName = name;
        ItemPower = power;
        itemType = type;
        ItemPrice = price;
        ItemQuantity = quantity;
        ItemDescription = description;
        ItemSprite = sprite;
    }
}