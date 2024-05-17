using Godot;
using System;

public partial class ItemDrop : CharacterBody2D
{
    float ACCELERATION = 460f;
    float MAX_SPEED = 225;
    Vector2 velocity = Vector2.Zero;
    string ItemName;
    private Dave player;
    private bool beingPickedUp = false;
    
    private Node playerInventory;

    public override void _Ready()
    {
        ItemName = "Slime Potion";
        playerInventory = GetNode<Node>("/root/PlayerInventory");
    }

    public override void _PhysicsProcess(double delta)
    {
        MoveAndCollide(velocity);
        if (beingPickedUp) 
        {
            playerInventory.Call("add_item", ItemName, 1);
            QueueFree();
        }
    }


    public void PickupItem(Dave body)
    {
        player = body;
        beingPickedUp = true;
    }
}
