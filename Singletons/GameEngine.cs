using Godot;
using System;

public partial class GameEngine : Node
{

    private Control inventory;

    public override void _Ready()
    {
        if (inventory == null)
        {
            inventory = GetNode<Control>("/root/Game/Dave/Inventory");
        }
    }


    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("open_inventory")) 
        {   
            if (inventory.Visible == false) {
                inventory.Visible = true;
            }
            else {
                inventory.Visible = false;
            }
        }
    }



}
