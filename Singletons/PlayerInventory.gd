extends Node

signal active_item_updated

const NUM_INVENTORY_SLOTS = 35
const NUM_HOTBAR_SLOTS = 8
const SlotClass = preload("res://InventoryGD/Slot.gd")
const ItemClass = preload("res://InventoryGD/Item.gd")


var inventory = {
    0: ["Iron Sword", 1],
    1: ["Tree Branch", 6],
    2: ["Slime Potion", 2],
    3: ["Slime Potion", 6],
    4: ["Slime Potion", 10]
}

var hotbar = {
    0: ["Iron Sword", 1],
    1: ["Iron Sword", 1],
    2: ["Tree Branch", 6],
    3: ["Slime Potion", 10]
}

var equips = {
    0: ["Brown Shirt", 1],
    1: ["Blue Jeans", 1],
    2: ["Brown Boots", 1],
}


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
    var slot = get_tree().root.get_node("/root/Game/UserInterface/Inventory/Panel/TextureRect/GridContainer/Slot" + str(slot_index + 1))
    if slot.item != null:
        slot.item.set_item(item_name, new_quantity)
    else:
        slot.initialize_item(item_name, new_quantity)


func add_item_to_empty_slot(item: ItemClass, slot: SlotClass):
    match slot.SlotType: 
        SlotClass.SlotType.HOTBAR:
            hotbar[slot.slot_index] = [item.item_name, item.item_quantity]
        SlotClass.SlotType.INVENTORY:
            inventory[slot.slot_index] = [item.item_name, item.item_quantity]
        _: 
            equips[slot.slot_index] = [item.item_name, item.item_quantity]


func remove_item(slot: SlotClass):
    match slot.SlotType: 
        SlotClass.SlotType.HOTBAR:
            hotbar.erase(slot.slot_index)
        SlotClass.SlotType.INVENTORY:
            inventory.erase(slot.slot_index)
        _: 
            equips.erase(slot.slot_index)


func add_item_quantity(slot: SlotClass, quantity_to_add: int):
    match slot.SlotType: 
        SlotClass.SlotType.HOTBAR:
            hotbar[slot.slot_index][1] += quantity_to_add
        SlotClass.SlotType.INVENTORY:
            inventory[slot.slot_index][1] += quantity_to_add
        _: 
            equips[slot.slot_index][1] += quantity_to_add


func get_inventory():
    return inventory

func get_equips():
    return equips

func active_item_scroll_up():
    active_item_slot = (active_item_slot + 1) % NUM_HOTBAR_SLOTS
    emit_signal("active_item_updated")

func active_item_scroll_down():
    if active_item_slot == 0:
        active_item_slot = NUM_HOTBAR_SLOTS - 1
    else:
        active_item_slot -= 1
    emit_signal("active_item_updated")
