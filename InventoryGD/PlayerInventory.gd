extends Node

const NUM_INVENTORY_SLOTS = 35
const NUM_HOTBAR_SLOTS = 8
const SlotClass = preload("res://InventoryGD/Slot.gd")
const ItemClass = preload("res://InventoryGD/Item.gd")


var inventory = {
	14: ["Iron Sword", 1],
	1: ["Tree Branch", 6],
	2: ["Slime Potion", 2],
}

var hotbar = {
	0: ["Iron Sword", 1],
	2: ["Tree Branch", 6],
	3: ["Slime Potion", 10]
}

var equips = {
	0: ["Iron Helmet", 1],
	2: ["Iron Chestplate", 1],
	4: ["Iron Leggins", 1],
	6: ["Iron Boots", 1],
}


func get_inventory():
	return inventory

func get_equips():
	return equips

func get_hotbar():
	return hotbar

func get_NUM_INVENTORY_SLOTS():
	return NUM_INVENTORY_SLOTS

func get_NUM_HOTBAR_SLOTS():
	return NUM_HOTBAR_SLOTS

