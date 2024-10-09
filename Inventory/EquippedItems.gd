extends GridContainer

signal slot_interacted
signal custom_mouse_entered(item_name)
signal custom_mouse_exited()

const SlotClass = preload("res://Inventory/Slot.gd")
@onready var equip_slots = get_node("/root/Main/UserInterface/Inventory/InventoryPanel/TextureRect2/EquipSlots")

var inventory = InventoryLogic.get_inventory()
var equips = InventoryLogic.get_equips()
var last_mouse_position = Vector2()


func _ready():
	var slotsEquip = equip_slots.get_children()
	for i in range(slotsEquip.size()):
		slotsEquip[i].gui_input.connect(slot_gui_input.bind(slotsEquip[i]))
		slotsEquip[i].mouse_exited.connect(on_slot_mouse_exited)
		slotsEquip[i].slot_index = i
	slotsEquip[0].slot_type = SlotClass.SlotType.HELMET
	slotsEquip[1].slot_type = SlotClass.SlotType.AMULET
	slotsEquip[2].slot_type = SlotClass.SlotType.TORSO
	slotsEquip[3].slot_type = SlotClass.SlotType.RING
	slotsEquip[4].slot_type = SlotClass.SlotType.PANTS
	slotsEquip[5].slot_type = SlotClass.SlotType.OFFHAND
	slotsEquip[6].slot_type = SlotClass.SlotType.SHOES
	slotsEquip[7].slot_type = SlotClass.SlotType.WEAPON

	initialize_equips()


func initialize_equips():
	var slotsEquip = equip_slots.get_children()
	for i in range(slotsEquip.size()):
		if InventoryLogic.equips.has(i):
			slotsEquip[i].initialize_item(InventoryLogic.equips[i][0], InventoryLogic.equips[i][1])
	emit_signal("slot_interacted")


func slot_gui_input(event: InputEvent, slot: SlotClass):
	if slot.item && !find_parent("UserInterface").holding_item:
		emit_signal("custom_mouse_entered", slot.item.item_name)
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
			emit_signal("slot_interacted")


func on_slot_mouse_exited():
	emit_signal("custom_mouse_exited")


func _input(_event):
	if find_parent("UserInterface").holding_item:
		find_parent("UserInterface").holding_item.global_position = get_global_mouse_position()


func is_correct_slot_type(slot: SlotClass):
	var holding_item = find_parent("UserInterface").holding_item
	var item_category_enum = JsonData.item_data[holding_item.item_name]["ItemCategory"]
	var item_category = slot.SlotTypeNames

	if item_category_enum == item_category.get(2) and slot.slot_type == 2:
		return true
	elif item_category_enum == item_category.get(3) and slot.slot_type == 3:
		return true
	elif item_category_enum == item_category.get(4) and slot.slot_type == 4:
		return true
	elif item_category_enum == item_category.get(5) and slot.slot_type == 5:
		return true
	elif item_category_enum == item_category.get(6) and slot.slot_type == 6:
		return true
	elif item_category_enum == item_category.get(7) and slot.slot_type == 7:
		return true
	elif item_category_enum == item_category.get(8) and slot.slot_type == 8:
		return true
	elif item_category_enum == item_category.get(9) and slot.slot_type == 9:
		return true
	
	else: 
		# you're a god, Morten :)
		pass

func left_click_empty_slot(slot: SlotClass):
	var holding_item = find_parent("UserInterface").holding_item
	if is_correct_slot_type(slot):
		InventoryLogic.add_item_to_empty_slot(holding_item, slot)
		slot.put_into_slot(holding_item)
		find_parent("UserInterface").holding_item = null


func left_click_different_item(event, slot: SlotClass):
	if is_correct_slot_type(slot):
		InventoryLogic.remove_item(slot)
		InventoryLogic.add_item_to_empty_slot(find_parent("UserInterface").holding_item, slot)
		var temp_item = slot.item
		slot.pick_from_slot()
		temp_item.global_position = event.global_position
		slot.put_into_slot(find_parent("UserInterface").holding_item)
		find_parent("UserInterface").holding_item = temp_item


func left_click_same_item(slot: SlotClass):
	if is_correct_slot_type(slot):
		var stack_size = int(JsonData.item_data[slot.item.item_name]["StackSize"])
		var able_to_add = stack_size - slot.item.item_quantity
		var item_quantity = find_parent("UserInterface").holding_item.item_quantity
		var item_key = slot.slot_index

		if able_to_add >= find_parent("UserInterface").holding_item.item_quantity:
			InventoryLogic.add_item_quantity(slot, find_parent("UserInterface").holding_item.item_quantity)
			slot.item.add_item_quantity(find_parent("UserInterface").holding_item.item_quantity)
			
			equips[item_key] = [slot.item.item_name, slot.item.item_quantity]

			find_parent("UserInterface").holding_item.queue_free()
			find_parent("UserInterface").holding_item = null
		else:
			InventoryLogic.add_item_quantity(slot, find_parent("UserInterface").holding_item.item_quantity)
			slot.item.add_item_quantity(able_to_add)
			find_parent("UserInterface").holding_item.decrease_item_quantity(able_to_add)

			equips[item_key] = [slot.item.item_name, slot.item.item_quantity]


func left_click_not_holding(slot: SlotClass):
	InventoryLogic.remove_item(slot)
	find_parent("UserInterface").holding_item = slot.item
	slot.pick_from_slot()
	find_parent("UserInterface").holding_item.global_position = get_global_mouse_position()
	InventoryLogic.get_equips().erase(slot.slot_index)



func get_equipped_items():
	var item_names = []
	for i in range(equip_slots.get_child_count()):
		var slot = equip_slots.get_child(i)
		if slot.slot_type in [SlotClass.SlotType.HELMET, SlotClass.SlotType.TORSO, SlotClass.SlotType.PANTS, SlotClass.SlotType.SHOES, SlotClass.SlotType.AMULET, SlotClass.SlotType.OFFHAND, SlotClass.SlotType.RING, SlotClass.SlotType.WEAPON]:
			if slot.item:
				item_names.append(slot.item.item_name)
	return item_names
