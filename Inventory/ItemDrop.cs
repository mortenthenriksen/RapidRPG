using Game.Characters;
using Godot;


public partial class ItemDrop : CharacterBody2D
{
    private string itemName;
    private Node InventoryLogic;
    private Dave player = null;
    private bool isBeingPickedUp = false;

    WeightedGroup<string> weightedItemDrops = new WeightedGroup<string>()
        {
            {"Iron Helmet", 10},
            {"Iron Chestplate", 70},
            {"Iron Leggins", 20},
            {"Legendary Sword", 20},
            {"Iron Shield", 20},
        }; 


    public override void _Ready()
    {
        itemName = WeightedItemDrop();
        InventoryLogic = GetNode<Node>("/root/InventoryLogic");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (isBeingPickedUp) 
        {
            InventoryLogic.Call("add_item", itemName, 1);
            QueueFree();
        }
    }

    public string WeightedItemDrop() 
    {
        string itemDropName = weightedItemDrops.GetItem();
        var sprite = GetNode<Sprite2D>("Sprite2D");
        var texture = (Texture)ResourceLoader.Load($"res://Inventory/ItemIcons/{itemDropName}.png");
        sprite.Texture = (Texture2D)texture;
        sprite.Scale = new Vector2(1f, 1f);
        return itemDropName;
    }

    public void PickupItem(Dave body)
    {
        player = body;
        isBeingPickedUp = true;
    }
}
