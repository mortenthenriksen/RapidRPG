using Godot;
using System;

public partial class GameEngine : Node
{

    private Node2D inventory;

    public override void _Ready()
    {
        inventory = GetNode<Node2D>("/root/Game/UserInterface/Inventory");
    }


    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("open_inventory")) 
        {   
            if (inventory.Visible == false) 
            {
                inventory.Visible = true;

            }
            else 
            {
                inventory.Visible = false;
            }
        }
    }



}
