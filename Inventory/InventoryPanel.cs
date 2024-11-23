using Godot;

namespace Game.Inventory;

public partial class InventoryPanel : Panel
{
    private bool isMouseHoveringInventory = false;

    private void OnMouseEntered()
    {
        isMouseHoveringInventory = true;
    }
    
    private void OnMouseExited()
    {
        isMouseHoveringInventory = false;
    }

    public bool GetIsMouseHoveringInventory()
    {
        return isMouseHoveringInventory;
    }

}
