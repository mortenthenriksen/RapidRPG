extends Node2D

const SlotClass = preload("res://Inventory/Slot.gd")
@onready var hotbar = $HotbarSlots
@onready var slots = hotbar.get_children()
@onready var active_item_label = $ActiveItemLabel


func _ready():
	for i in range(slots.size()):
		slots[i].gui_input.connect(slot_gui_input.bind(slots[i]))
		# old godot: 
		# PlayerInventory.Connect("active_item_updated, slots[i], "refresh_style)
		# new godot method for the same thing:
		# InventoryLogic.active_item_updated.connect(slots[i].refresh_style)
		slots[i].slot_index = i
		slots[i].slot_type = SlotClass.SlotType.HOTBAR

	InventoryLogic.active_item_updated.connect(update_active_item_label)
	initialize_hotbar()
	update_active_item_label()

func update_active_item_label():
	if slots[InventoryLogic.active_item_slot].item != null:
		active_item_label.text = slots[InventoryLogic.active_item_slot].item.item_name
	else:
		active_item_label.text = ""

func initialize_hotbar():
	for i in range(slots.size()):
		if InventoryLogic.hotbar.has(i):
			slots[i].initialize_item(InventoryLogic.hotbar[i][0], InventoryLogic.hotbar[i][1])


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
			update_active_item_label()


func _input(_event):
	if find_parent("UserInterface").holding_item:
		find_parent("UserInterface").holding_item.global_position = get_global_mouse_position()


func left_click_empty_slot(slot: SlotClass):
	InventoryLogic.add_item_to_empty_slot(find_parent("UserInterface").holding_item, slot)
	slot.put_into_slot(find_parent("UserInterface").holding_item)

	var item_value = find_parent("UserInterface").holding_item.item_name
	var item_quantity = find_parent("UserInterface").holding_item.item_quantity
	var item_key = slot.slot_index

	InventoryLogic.hotbar[item_key] = [item_value, item_quantity]

	find_parent("UserInterface").holding_item = null


func left_click_different_item(event, slot: SlotClass):
	InventoryLogic.remove_item(slot)
	InventoryLogic.add_item_to_empty_slot(find_parent("UserInterface").holding_item, slot)
	var temp_item = slot.item
	slot.pick_from_slot()
	temp_item.global_position = event.global_position
	slot.put_into_slot(find_parent("UserInterface").holding_item)
	find_parent("UserInterface").holding_item = temp_item


func left_click_same_item(slot: SlotClass):
	var stack_size = int(JsonData.item_data[slot.item.item_name]["StackSize"])
	var able_to_add = stack_size - slot.item.item_quantity
	if able_to_add >= find_parent("UserInterface").holding_item.item_quantity:
		InventoryLogic.add_item_quantity(slot, find_parent("UserInterface").holding_item.item_quantity)
		slot.item.add_item_quantity(find_parent("UserInterface").holding_item.item_quantity)
		find_parent("UserInterface").holding_item.queue_free()
		find_parent("UserInterface").holding_item = null
	else:
		InventoryLogic.add_item_quantity(slot, find_parent("UserInterface").holding_item.item_quantity)
		slot.item.add_item_quantity(able_to_add)
		find_parent("UserInterface").holding_item.decrease_item_quantity(able_to_add)


func left_click_not_holding(slot: SlotClass):
	InventoryLogic.remove_item(slot)
	find_parent("UserInterface").holding_item = slot.item
	InventoryLogic.hotbar.erase(slot.slot_index)
	slot.pick_from_slot()
	find_parent("UserInterface").holding_item.global_position = get_global_mouse_position()
