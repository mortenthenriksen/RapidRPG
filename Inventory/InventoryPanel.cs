using Godot;

namespace Game.Inventory;

public partial class InventoryPanel : Panel
{
    private bool isMouseHoveringInventory = false;

    private void OnMouseEntered()
    {
        GD.Print("entered");
        isMouseHoveringInventory = true;
    }
    
    private void OnMouseExited()
    {
        GD.Print("exited");
        isMouseHoveringInventory = false;
    }

    public bool GetIsMouseHoveringInventory()
    {
        return isMouseHoveringInventory;
    }

}
