extends CanvasLayer

var holding_item = null

func _input(event):
    if event.is_action_pressed("open_inventory"):
        $Inventory.visible = !$Inventory.visible
        
    if event.is_action_released("scroll_up"):
        PlayerInventory.active_item_scroll_down()
    if event.is_action_released("scroll_down"):
        PlayerInventory.active_item_scroll_up()