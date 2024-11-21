extends Panel

var ItemClass = preload("res://Inventory/Item.tscn")
var item = null
var slot_type = null
var slot_index

enum SlotType {
	HOTBAR = 0, 
	INVENTORY,
	HELMET, #2
	AMULET, #3 
	TORSO, #4
	RING, #5
	PANTS, #6
	OFFHAND, #7
	SHOES, #8
	WEAPON, #9
}

var SlotTypeNames = {
	SlotType.HELMET: "Helmet",
	SlotType.AMULET: "Amulet",
    SlotType.TORSO: "Torso",
	SlotType.RING: "Ring",
    SlotType.PANTS: "Pants",
	SlotType.OFFHAND: "Offhand",
	SlotType.SHOES: "Shoes",
	SlotType.WEAPON: "Weapon"
}

func pick_from_slot():
	remove_child(item)
	var inventoryNode = find_parent("UserInterface")
	inventoryNode.add_child(item)
	item = null

func put_into_slot(new_item):
	item = new_item
	item.position = Vector2(0, 0)
	var inventoryNode = find_parent("UserInterface")
	if inventoryNode:
		inventoryNode.remove_child(item)
	add_child(item)

func initialize_item(item_name, item_quantity):
	if item == null:
		item = ItemClass.instantiate();
		add_child(item)
		item.set_item(item_name, item_quantity)
	else:
		item.set_item(item_name, item_quantity)


func get_SlotTypeNames():
	return SlotTypeNames