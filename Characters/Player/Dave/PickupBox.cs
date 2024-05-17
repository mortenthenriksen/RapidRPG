using Godot;
using Godot.Collections;
using System;

public partial class PickupBox : Area2D
{

    Dictionary itemsInRange;


    public override void _Ready()
    {
       
    }


    public void OnPickupBoxBodyEntered(Area2D body) 
    {
        itemsInRange[body] = body;
    }


    public void OnPickupBoxBodyExited(Area2D body) 
    {
        if (itemsInRange.ContainsKey(body)) 
        {
            itemsInRange.Remove(body);
        }
    }
}
