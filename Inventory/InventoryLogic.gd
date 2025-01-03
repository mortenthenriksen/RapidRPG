extends Node

const SlotClass = preload("res://Inventory/Slot.gd")
const ItemClass = preload("res://Inventory/Item.gd")

var inventory = PlayerInventory.get_inventory()
var equips = PlayerInventory.get_equips()

var NUM_INVENTORY_SLOTS = PlayerInventory.get_NUM_INVENTORY_SLOTS()

var active_item_slot = 0

func add_item(item_name, item_quantity):
	for item in inventory:
		if inventory[item][0] == item_name:
			var stack_size = int(JsonData.item_data[item_name]["StackSize"])
			var able_to_add = stack_size - inventory[item][1] 
			if able_to_add >= item_quantity:
				inventory[item][1] += item_quantity
				update_slot_visual(item, inventory[item][0], inventory[item][1])
				return
			else:
				inventory[item][1] += able_to_add
				update_slot_visual(item, inventory[item][0], inventory[item][1])
				item_quantity = item_quantity - able_to_add

	for i in range(NUM_INVENTORY_SLOTS):
		if inventory.has(i) == false:
			inventory[i] = [item_name, item_quantity]
			update_slot_visual(i, inventory[i][0], inventory[i][1])
			return


func update_slot_visual(slot_index, item_name, new_quantity):
	var slot = get_tree().root.get_node("/root/Main/Dave/UserInterface/Inventory/InventoryPanel/TextureRect/InventorySlots/Slot" + str(slot_index + 1))
	if slot.item != null:
		slot.item.set_item(item_name, new_quantity)
	else:
		slot.initialize_item(item_name, new_quantity)


func add_item_to_empty_slot(item: ItemClass, slot: SlotClass):
	match slot.SlotType: 
		SlotClass.SlotType.INVENTORY:
			inventory[slot.slot_index] = [item.item_name, item.item_quantity]
		_: 
			equips[slot.slot_index] = [item.item_name, item.item_quantity]


func remove_item(slot: SlotClass):
	match slot.SlotType: 
		SlotClass.SlotType.INVENTORY:
			inventory.erase(slot.slot_index)
		_: 
			equips.erase(slot.slot_index)


func add_item_quantity(slot: SlotClass, quantity_to_add: int):
	match slot.SlotType: 
		SlotClass.SlotType.INVENTORY:
			inventory[slot.slot_index][1] += quantity_to_add


func get_inventory():
	return inventory

func get_equips():
	return equips
