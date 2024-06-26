extends Panel

var default_tex = preload("res://InventoryGD/Sprites/item_slot_default_background.png")
var empty_tex = preload("res://InventoryGD/Sprites/item_slot_empty_background.png")
var selected_tex = preload("res://InventoryGD/Sprites/item_slot_selected_background.png")

var default_style: StyleBoxTexture = null;
var empty_style: StyleBoxTexture = null;
var selected_style: StyleBoxTexture = null;

var ItemClass = preload("res://InventoryGD/Item.tscn")
var item = null;
var slot_index
var slot_type

enum SlotType {
	HOTBAR = 0, 
	INVENTORY,
}

func _ready():
	default_style = StyleBoxTexture.new()
	empty_style = StyleBoxTexture.new()
	selected_style = StyleBoxTexture.new()

	default_style.texture = default_tex
	empty_style.texture = empty_tex
	selected_style.texture = selected_tex

	refresh_style()

func refresh_style():
	if SlotType.HOTBAR == slot_type and PlayerInventory.active_item_slot == slot_index:
		set("theme_override_styles/panel", selected_style)
	elif item == null:
		set("theme_override_styles/panel", empty_style)
	else:
		set("theme_override_styles/panel", default_style)


func pick_from_slot():
	remove_child(item)
	var inventoryNode = find_parent("UserInterface")
	inventoryNode.add_child(item)
	item = null
	refresh_style()
	

func put_into_slot(new_item):
	item = new_item
	item.position = Vector2(0,0)
	var inventoryNode = find_parent("UserInterface")
	inventoryNode.remove_child(item)
	add_child(item)
	refresh_style()
	

func initialize_item(item_name, item_quantity):
	if item == null:
		item = ItemClass.instantiate();
		add_child(item)
		item.set_item(item_name, item_quantity)
	else:
		item.set_item(item_name, item_quantity)
	refresh_style()