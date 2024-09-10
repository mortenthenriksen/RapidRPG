extends CanvasLayer

var holding_item = null

func _input(event):
	if event.is_action_pressed("open_inventory"):
		$Inventory.visible = !$Inventory.visible
		
	if event.is_action_released("scroll_up"):
		InventoryLogic.active_item_scroll_down()
	if event.is_action_released("scroll_down"):
		InventoryLogic.active_item_scroll_up()
