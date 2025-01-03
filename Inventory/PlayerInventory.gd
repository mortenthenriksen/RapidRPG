extends Node

const NUM_INVENTORY_SLOTS = 35
const SlotClass = preload("res://Inventory/Slot.gd")
const ItemClass = preload("res://Inventory/Item.gd")


# TODO: add a save data option, simply just save the dictionary in some json file along with multiplier, and level, skill tree etc
# shouldnt be too bad
var inventory = {
	3: ["Iron Helmet", 1],
	4: ["Legendary Sword", 1],
	1: ["Sword of Undying Flame", 1]
}

var equips = {
	0: ["Iron Helmet", 1],
	2: ["Iron Chestplate", 1],
	4: ["Iron Leggins", 1],
	5: ["Wooden Shield", 1],
	6: ["Iron Boots", 1],
	7: ["Legendary Sword", 1],
}

func get_inventory():
	return inventory

func get_equips():
	return equips

func get_NUM_INVENTORY_SLOTS():
	return NUM_INVENTORY_SLOTS


