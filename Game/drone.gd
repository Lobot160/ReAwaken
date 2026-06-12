extends CharacterBody2D


const SPEED = 300.0
const MOVE_ACC = 1000

const ROTATE = 20
const ROTATE_SPEED = 100

@onready var sprite: Sprite2D = $Sprite2D

func _physics_process(delta: float) -> void:
	var direction := Input.get_vector("drone_left", "drone_right", "drone_up", "drone_down")

	

	# Get the input direction and handle the movement/deceleration.
	# As good practice, you should replace UI actions with custom gameplay actions.
	
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

	move_and_slide()
