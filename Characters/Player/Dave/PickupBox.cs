using Godot;
using Godot.Collections;

public partial class PickupBox : Area2D
{

    Dictionary itemsInRange;


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
