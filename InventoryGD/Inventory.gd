extends Node2D

const SlotClass = preload("res://InventoryGD/Slot.gd")
const ItemClass = preload("res://InventoryGD/Item.gd")

@onready var inventory_slots = $Panel/TextureRect/GridContainer

var inventory = InventoryLogic.get_inventory()
var equips = InventoryLogic.get_equips()



func _ready():
	var slots = inventory_slots.get_children()
	for i in range(slots.size()):
		slots[i].gui_input.connect(slot_gui_input.bind(slots[i]))
		slots[i].slot_index = i
		slots[i].slot_type = SlotClass.SlotType.INVENTORY

	initialize_inventory()
	
func initialize_inventory():
	var slots = inventory_slots.get_children()
	for i in range(slots.size()):
		if InventoryLogic.inventory.has(i):
			slots[i].initialize_item(InventoryLogic.inventory[i][0], InventoryLogic.inventory[i][1])


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
	

	# elif event is InputEventMouseMotion:
	# 	if slot.item:
	# 		var item_stats = JsonData.item_data[slot.item.item_name]
	# 		var keys = item_stats.keys()
	# 		print(keys)
	# 		# tooltip_instance.ShowToolTip()
	# 		if keys.size() > 1:
	# 			tooltip_instance.UpdateToolTip(slot.item.item_name)
	# 			for key in keys:
	# 				# var stat_key = key
	# 				# var stat_value = item_stats[stat_key]
	# 				# tooltip_instance.UpdateStats(stat_key, str(stat_value))
	# 	else:
	# 		_on_slot_1_mouse_exited()
	# else:
	# 	_on_slot_1_mouse_exited()		

func _input(_event):
	if find_parent("UserInterface").holding_item:
		find_parent("UserInterface").holding_item.global_position = get_global_mouse_position()



func moving_items_inside_inventory(slot: SlotClass):
	var item_value = find_parent("UserInterface").holding_item.item_name
	var item_quantity = find_parent("UserInterface").holding_item.item_quantity
	var item_key = slot.slot_index

	inventory[item_key] = [item_value, item_quantity]


func left_click_empty_slot(slot: SlotClass):
	InventoryLogic.add_item_to_empty_slot(find_parent("UserInterface").holding_item, slot)
	moving_items_inside_inventory(slot)
	slot.put_into_slot(find_parent("UserInterface").holding_item)
	find_parent("UserInterface").holding_item = null


func left_click_different_item(event, slot: SlotClass):
	InventoryLogic.remove_item(slot)
	InventoryLogic.add_item_to_empty_slot(find_parent("UserInterface").holding_item, slot)
	var temp_item = slot.item
	slot.pick_from_slot()

	moving_items_inside_inventory(slot)
	
	temp_item.global_position = event.global_position
	slot.put_into_slot(find_parent("UserInterface").holding_item)
	find_parent("UserInterface").holding_item = temp_item


func left_click_same_item(slot: SlotClass):
	var stack_size = int(JsonData.item_data[slot.item.item_name]["StackSize"])
	var able_to_add = stack_size - slot.item.item_quantity
	var item_quantity = find_parent("UserInterface").holding_item.item_quantity
	var item_key = slot.slot_index

	if able_to_add >= find_parent("UserInterface").holding_item.item_quantity:
		InventoryLogic.add_item_quantity(slot, find_parent("UserInterface").holding_item.item_quantity)
		slot.item.add_item_quantity(find_parent("UserInterface").holding_item.item_quantity)
		
		inventory[item_key] = [slot.item.item_name, slot.item.item_quantity]

		find_parent("UserInterface").holding_item.queue_free()
		find_parent("UserInterface").holding_item = null
	else:
		InventoryLogic.add_item_quantity(slot, find_parent("UserInterface").holding_item.item_quantity)
		slot.item.add_item_quantity(able_to_add)
		find_parent("UserInterface").holding_item.decrease_item_quantity(able_to_add)

		inventory[item_key] = [slot.item.item_name, slot.item.item_quantity]


func left_click_not_holding(slot: SlotClass):
	InventoryLogic.remove_item(slot)

	find_parent("UserInterface").holding_item = slot.item
	InventoryLogic.get_inventory().erase(slot.slot_index)
	
	slot.pick_from_slot()
	find_parent("UserInterface").holding_item.global_position = get_global_mouse_position()