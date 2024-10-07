extends CanvasLayer

var holding_item = null
@onready var audio_player = $AudioStreamPlayer2D
# @export var player_path = "/root/Main/Knight"

# func _ready():
	# player = get_node(player_path)

func _input(event):
	if event.is_action_pressed("open_inventory"):
		$Inventory.visible = !$Inventory.visible
		# audio_player.position = player.position
		audio_player.play()	

		
