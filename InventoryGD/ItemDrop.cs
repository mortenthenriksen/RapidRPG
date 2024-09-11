using Godot;
using System;

public partial class ItemDrop : CharacterBody2D
{
    string itemName;
    private Dave player = null;
    public bool isBeingPickedUp = false;
    
    private Node InventoryLogic;

    public override void _Ready()
    {
        itemName = RandomItemDrop();
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

    public string RandomItemDrop() 
    {
        Random random = new Random();
		if (random.NextDouble() >= 0.5) 
		{
			var sprite = GetNode<Sprite2D>("Sprite2D");
			var texture = (Texture)ResourceLoader.Load("res://InventoryGD/ItemIcons/Iron Helmet.png");
			sprite.Texture = (Texture2D)texture;
            sprite.Scale = new Vector2(1f, 1f); // Scale down to 50%
			return "Iron Helmet";
		} 
		else 
		{
		    var sprite = GetNode<Sprite2D>("Sprite2D");
			var texture = (Texture)ResourceLoader.Load("res://InventoryGD/ItemIcons/Tree Branch.png");
			sprite.Texture = (Texture2D)texture;
			return "Tree Branch";
		}
    }

    public void PickupItem(Dave body)
    {
        player = body;
        isBeingPickedUp = true;
    }
}
