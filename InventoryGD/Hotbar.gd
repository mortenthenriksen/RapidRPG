extends Node2D

const SlotClass = preload("res://InventoryGD/Slot.gd")
@onready var hotbar = $HotbarSlots
@onready var slots = hotbar.get_children()


func _ready():
	for i in range(slots.size()):
		slots[i].gui_input.connect(slot_gui_input.bind(slots[i]))
		# old godot: 
		# PlayerInventory.Connect("active_item_updated, slots[i], "refresh_style)
		# new godot method for the same thing:
		PlayerInventory.active_item_updated.connect(slots[i].refresh_style)
		slots[i].slot_index = i
		slots[i].slot_type = SlotClass.SlotType.HOTBAR
	initialize_hotbar()



func initialize_hotbar():
	for i in range(slots.size()):
		if PlayerInventory.hotbar.has(i):
			slots[i].initialize_item(PlayerInventory.hotbar[i][0], PlayerInventory.hotbar[i][1])


func slot_gui_input(event: InputEvent, slot: SlotClass):
	if event is InputEventMouseButton:
		if event.button_index == MOUSE_BUTTON_LEFT && event.is_pressed():
			if find_parent("UserInterface").holding_item != null:
				if !slot.item:
					left_click_empty_slot(slot)
				else:
					if find_parent("UserInterface").holding_item.item_name != slot.item.item_name:
						left_click_different_item(event, slot)
					else:
						left_click_same_item(slot)
			elif slot.item:
				left_click_not_holding(slot)
			
func _input(_event):
	if find_parent("UserInterface").holding_item:
		find_parent("UserInterface").holding_item.global_position = get_global_mouse_position()


func left_click_empty_slot(slot: SlotClass):
	PlayerInventory.add_item_to_empty_slot(find_parent("UserInterface").holding_item, slot, true)
	slot.put_into_slot(find_parent("UserInterface").holding_item)
	find_parent("UserInterface").holding_item = null


func left_click_different_item(event, slot: SlotClass):
	PlayerInventory.remove_item(slot, true)
	PlayerInventory.add_item_to_empty_slot(find_parent("UserInterface").holding_item, slot, true)
	var temp_item = slot.item
	slot.pick_from_slot()
	temp_item.global_position = event.global_position
	slot.put_into_slot(find_parent("UserInterface").holding_item)
	find_parent("UserInterface").holding_item = temp_item


func left_click_same_item(slot: SlotClass):
	var stack_size = int(JsonData.item_data[slot.item.item_name]["StackSize"])
	var able_to_add = stack_size - slot.item.item_quantity
	if able_to_add >= find_parent("UserInterface").holding_item.item_quantity:
		PlayerInventory.add_item_quantity(slot, find_parent("UserInterface").holding_item.item_quantity, true)
		slot.item.add_item_quantity(find_parent("UserInterface").holding_item.item_quantity)
		find_parent("UserInterface").holding_item.queue_free()
		find_parent("UserInterface").holding_item = null
	else:
		PlayerInventory.add_item_quantity(slot, find_parent("UserInterface").holding_item.item_quantity, true)
		slot.item.add_item_quantity(able_to_add)
		find_parent("UserInterface").holding_item.decrease_item_quantity(able_to_add)


func left_click_not_holding(slot: SlotClass):
	PlayerInventory.remove_item(slot, true)
	find_parent("UserInterface").holding_item = slot.item
	slot.pick_from_slot()
	find_parent("UserInterface").holding_item.global_position = get_global_mouse_position()