extends Node


const NUM_INVENTORY_SLOTS = 35

var inventory = {
    0: ["Iron Sword", 1]
}


func add_item(item_name, item_quantity):
    for item in inventory:
        if inventory[item][0] == item_name:
            inventory[item][1] += item_quantity
            return

    for i in range(NUM_INVENTORY_SLOTS):
        if inventory.has(i) == false:
            inventory[i] = [item_name, item_quantity]
            return