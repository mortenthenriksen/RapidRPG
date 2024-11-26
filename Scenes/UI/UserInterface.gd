extends CanvasLayer

var holding_item = null
@onready var audio_player = $AudioStreamPlayer2D

func _input(event):
	if event.is_action_pressed("open_inventory"):
		$Inventory.visible = !$Inventory.visible
		audio_player.play()	

		
