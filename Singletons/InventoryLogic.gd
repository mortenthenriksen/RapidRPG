extends Node

signal active_item_updated

const SlotClass = preload("res://InventoryGD/Slot.gd")
const ItemClass = preload("res://InventoryGD/Item.gd")
@onready var hotbar_slots = get_node("/root/Game/UserInterface/Hotbar/HotbarSlots")
@onready var active_item_label = get_node("/root/Game/UserInterface/Hotbar/ActiveItemLabel")
@onready var equip_slots = ("/root/Game/UserInterface/InventoryPanel/TextureRect2/EquipSlots")

var inventory = PlayerInventory.get_inventory()
var hotbar = PlayerInventory.get_hotbar()
var equips = PlayerInventory.get_equips()

var NUM_INVENTORY_SLOTS = PlayerInventory.get_NUM_INVENTORY_SLOTS()
var NUM_HOTBAR_SLOTS = PlayerInventory.get_NUM_HOTBAR_SLOTS()

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


func is_correct_item_type(slot: SlotClass):
    if find_parent("UserInterface").holding_item == null:
        return true
    var holding_item_category = JsonData.item_data[find_parent("UserInterface").holding_item.item_name]["ItemCategory"]

    if slot.slotType == SlotClass.SlotType.SHIRT:
        return holding_item_category == "Shirt"
    elif slot.slotType == SlotClass.SlotType.PANTS:
        return holding_item_category == "Pants"
    elif slot.slotType == SlotClass.SlotType.SHOES:
        return holding_item_category == "Shoes"
    return true


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


func active_item_scroll_up():
    active_item_slot = (active_item_slot + 1) % NUM_HOTBAR_SLOTS
    emit_signal("active_item_updated")


func active_item_scroll_down():
    if active_item_slot == 0:
        active_item_slot = NUM_HOTBAR_SLOTS - 1
    else:
        active_item_slot -= 1 
    emit_signal("active_item_updated")


func get_inventory():
    return inventory

func get_equips():
    return equips