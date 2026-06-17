extends CharacterBody2D


const SPEED = 300.0
const MOVE_ACC = 1000
const FOLLOW_SPEED = 100

const ROTATE = 20
const ROTATE_SPEED = 100

@onready var sprite: Sprite2D = $Sprite2D
@onready var player: CharacterBody2D = $"../Player"

var mode = "follow"

func _physics_process(delta: float) -> void:
	# Get the input direction and handle the movement/deceleration.
	var direction = Input.get_vector("drone_left", "drone_right", "drone_up", "drone_down")
	
	# Change drone mode to manual
	if direction:
		mode = "manual"
	
	# Do drone mode
	match mode:
		"manual": 
			manual_mode(delta, direction)
		"follow": 
			follow_mode(delta)
		_: 
			print("Invalid Mode")
	
	move_and_slide()

func manual_mode(delta: float, direction: Vector2) -> void:
	if direction.x:
		velocity.x = move_toward(velocity.x, SPEED * direction.x, MOVE_ACC * delta)
		rotation_degrees = move_toward(rotation_degrees, ROTATE * direction.x, ROTATE_SPEED * delta)
	else:
		velocity.x = move_toward(velocity.x, 0, MOVE_ACC * delta)
		rotation_degrees = move_toward(rotation_degrees, 0, ROTATE_SPEED * delta)
	if direction.y:
		velocity.y = move_toward(velocity.y, SPEED * direction.y, MOVE_ACC * delta)
	else:
		velocity.y = move_toward(velocity.y, 0, MOVE_ACC * delta)

func follow_mode(delta):
	if $NavigationAgent2D.target_position != player.global_position:
		$NavigationAgent2D.target_position = player.global_position
	print($NavigationAgent2D.target_position)
	if !$NavigationAgent2D.is_target_reached():
		var nav_point_location = to_local($NavigationAgent2D.get_next_path_position()).normalized()
		var distance_to_target = $NavigationAgent2D.distance_to_target() + 1
		print(nav_point_location)
		velocity = velocity.move_toward(nav_point_location * distance_to_target * delta * SPEED, MOVE_ACC * delta)
		print(velocity)
	else:
		velocity = velocity.move_toward(Vector2.ZERO, MOVE_ACC * delta)

func _on_player_range_exited(body: Node2D) -> void:
	if body == player:
		mode = "follow"
	
