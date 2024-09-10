extends Node2D

const SlotClass = preload("res://InventoryGD/Slot.gd")
@onready var inventory_slots = $Panel/TextureRect/GridContainer
@onready var equip_slots = $Panel/TextureRect2/EquipSlots

var inventory = PlayerInventory.get_inventory()
var equips = PlayerInventory.get_equips()

func _ready():
	var slots = inventory_slots.get_children()
	for i in range(slots.size()):
		slots[i].gui_input.connect(slot_gui_input.bind(slots[i]))
		slots[i].slot_index = i
		slots[i].slot_type = SlotClass.SlotType.INVENTORY

	var slotsEquip = equip_slots.get_children()
	for i in range(slotsEquip.size()):
		slotsEquip[i].gui_input.connect(slot_gui_input.bind(slotsEquip[i]))
		slotsEquip[i].slot_index = i
	slotsEquip[1].slot_type = SlotClass.SlotType.SHIRT
	slotsEquip[1].slot_type = SlotClass.SlotType.PANTS
	slotsEquip[2].slot_type = SlotClass.SlotType.SHOES
	
	initialize_inventory()
	initialize_equips()
	
func initialize_inventory():
	var slots = inventory_slots.get_children()
	for i in range(slots.size()):
		if PlayerInventory.inventory.has(i):
			slots[i].initialize_item(PlayerInventory.inventory[i][0], PlayerInventory.inventory[i][1])


func initialize_equips():
	var slotsEquip = equip_slots.get_children()
	for i in range(slotsEquip.size()):
		if PlayerInventory.equips.has(i):
			slotsEquip[i].initialize_item(PlayerInventory.equips[i][0], PlayerInventory.equips[i][1])


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


func moving_items_inside_inventory(slot: SlotClass):
	var item_value = find_parent("UserInterface").holding_item.item_name
	var item_quantity = find_parent("UserInterface").holding_item.item_quantity
	var item_key = str(slot.slot_type) + "_" + str(slot.slot_index)

	inventory[item_key] = [item_value, item_quantity]
	print(inventory)
	print(equips)


func _input(_event):
	if find_parent("UserInterface").holding_item:
		find_parent("UserInterface").holding_item.global_position = get_global_mouse_position()


func is_correct_slot_type(slot: SlotClass):
	var holding_item = find_parent("UserInterface").holding_item
	if holding_item == null:
		return true
	var holding_item_category = JsonData.item_data[holding_item.item_name]["ItemCategory"]

	if slot.slotType == SlotClass.SlotType.SHIRT:
		return holding_item_category == "Shirt"
	elif slot.slotType == SlotClass.SlotType.PANTS:
		return holding_item_category == "Pants"
	elif slot.slotType == SlotClass.SlotType.SHOES:
		return holding_item_category == "Shoes"
	return true


func left_click_empty_slot(slot: SlotClass):
	# if is_correct_slot_type(slot):
		PlayerInventory.add_item_to_empty_slot(find_parent("UserInterface").holding_item, slot)
		moving_items_inside_inventory(slot)
		slot.put_into_slot(find_parent("UserInterface").holding_item)
		find_parent("UserInterface").holding_item = null


func left_click_different_item(event, slot: SlotClass):
	#if is_correct_slot_type(slot):
		PlayerInventory.remove_item(slot)
		PlayerInventory.add_item_to_empty_slot(find_parent("UserInterface").holding_item, slot)
		var temp_item = slot.item
		slot.pick_from_slot()

		moving_items_inside_inventory(slot)
		
		temp_item.global_position = event.global_position
		slot.put_into_slot(find_parent("UserInterface").holding_item)
		find_parent("UserInterface").holding_item = temp_item


func left_click_same_item(slot: SlotClass):
	#if is_correct_slot_type(slot):
		var stack_size = int(JsonData.item_data[slot.item.item_name]["StackSize"])
		var able_to_add = stack_size - slot.item.item_quantity
		var item_quantity = find_parent("UserInterface").holding_item.item_quantity
		var item_key = slot.slot_index

		if able_to_add >= find_parent("UserInterface").holding_item.item_quantity:
			PlayerInventory.add_item_quantity(slot, find_parent("UserInterface").holding_item.item_quantity)
			slot.item.add_item_quantity(find_parent("UserInterface").holding_item.item_quantity)
			
			inventory[item_key] = [slot.item.item_name, slot.item.item_quantity]

			find_parent("UserInterface").holding_item.queue_free()
			find_parent("UserInterface").holding_item = null
		else:
			PlayerInventory.add_item_quantity(slot, find_parent("UserInterface").holding_item.item_quantity)
			slot.item.add_item_quantity(able_to_add)
			find_parent("UserInterface").holding_item.decrease_item_quantity(able_to_add)

			inventory[item_key] = [slot.item.item_name, slot.item.item_quantity]


func left_click_not_holding(slot: SlotClass):
	PlayerInventory.remove_item(slot)

	find_parent("UserInterface").holding_item = slot.item
	PlayerInventory.get_inventory().erase(slot.slot_index)
	
	slot.pick_from_slot()
	find_parent("UserInterface").holding_item.global_position = get_global_mouse_position()
