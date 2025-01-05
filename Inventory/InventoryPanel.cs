using Godot;

namespace Game.Inventory;

public partial class InventoryPanel : Panel
{
    private bool isMouseHoveringInventory = false;

    
    [Export]
    private GridContainer equippedSlots;

    [Export]
    private GridContainer inventorySlots;

    private AudioStreamPlayer2D audioStreamPlayer2D;

    public override void _Ready()
    {
        audioStreamPlayer2D = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
        equippedSlots.Connect("slot_interacted", Callable.From(OnSlotInteracted));
        inventorySlots.Connect("slot_interacted", Callable.From(OnSlotInteracted));
    }

    private void OnMouseEntered()
    {
        isMouseHoveringInventory = true;
    }
    
    private void OnMouseExited()
    {
        isMouseHoveringInventory = false;
    }

    private void OnSlotInteracted()
    {
        audioStreamPlayer2D.Play();
    }

    public bool GetIsMouseHoveringInventory()
    {
        return isMouseHoveringInventory;
    }

}
