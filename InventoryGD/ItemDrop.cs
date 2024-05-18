using Godot;
using System;

public partial class ItemDrop : CharacterBody2D
{
    float ACCELERATION = 460f;
    float MAX_SPEED = 225;
    Vector2 velocity = Vector2.Zero;
    string itemName;
    private Dave player = null;
    public bool isBeingPickedUp = false;
    
    private Node playerInventory;
    private Node2D inventory;

    public override void _Ready()
    {
        itemName = "Slime Potion";
        playerInventory = GetNode<Node>("/root/PlayerInventory");
        inventory = GetNode<Node2D>("/root/Game/UserInterface/Inventory");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (isBeingPickedUp) 
        {
            playerInventory.Call("add_item", itemName, 1);
            inventory.Call("initialize_inventory");
            QueueFree();
        }
    }


    public void PickupItem(Dave body)
    {
        player = body;
        isBeingPickedUp = true;
    }
}
