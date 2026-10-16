extends CharacterBody2D

const MOVE_ACC = 1000
const MAX_VEL = 750
const JUMP = 350
const FLOAT = 0.5 #Gravity factor when jumping
const COYOTE = 0.1 #Jump Grace Time

@onready var wall_sensor: Area2D = $"Wall Sensor"
@onready var floor_sensor: ShapeCast2D = $"Floor Sensor"


#Movement Variables
var extraJumps: int = 1
var jumpCount: int = 0
var timeInAir: float = 0
var onFloor: bool = false

#Acceleration Penalty Variables

var accPenalty: float = 1
var accPenaltyWall: float = 1 

#Camera Variables
var movement_camOffset = Vector2.ZERO
var input_camOffset = Vector2.ZERO


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass
	
func _physics_process(delta: float) -> void:
	detectFloor(delta)
	calcInput(delta)
	calcGravity(delta)
	calcAccPenalty(delta)
	moveCamera(delta)
	move_and_slide()

#Movement
func calcInput(delta: float) -> void:
	if Input.is_action_pressed("left"):
		if velocity.x > 0: #Helps turning
			velocity.x *= exp(-global.DRAG * delta) * (exp(-global.FRICTION * delta) if is_on_floor() else 1)
			velocity.x = move_toward(velocity.x, -MAX_VEL, MOVE_ACC * delta * accPenalty)
		elif velocity.x > -MAX_VEL:
			velocity.x = move_toward(velocity.x, -MAX_VEL, MOVE_ACC * delta * accPenalty)
	if Input.is_action_pressed("right"):
		if velocity.x < 0: #Helps turning
			velocity.x *= exp(-global.DRAG * delta) * (exp(-global.FRICTION * delta) if is_on_floor() else 1)
			velocity.x = move_toward(velocity.x, MAX_VEL, MOVE_ACC * delta * accPenalty)
		elif velocity.x < MAX_VEL:
			velocity.x = move_toward(velocity.x, MAX_VEL, MOVE_ACC * delta * accPenalty)
	
	if -MAX_VEL > velocity.x or velocity.x > MAX_VEL or (Input.is_action_pressed("left") == Input.is_action_pressed("right")):
		velocity.x *= exp(-global.DRAG * delta) * (exp(-global.FRICTION * delta) if is_on_floor() else 1)
		velocity.x = move_toward(velocity.x,0,global.DRAG_LNR * delta)
	
	if canJump(delta) and Input.is_action_just_pressed("jump"):
		velocity.y = -JUMP

func calcGravity(delta: float) -> void:
	if onFloor:
		pass
	elif Input.is_action_pressed("jump"):
		velocity.y += global.GRAVITY * delta * FLOAT
	else:
		velocity.y += global.GRAVITY * delta

func canJump(delta: float) -> bool:
	if onFloor:
		timeInAir = 0
		jumpCount = 0 
	else:
		timeInAir += delta
	if timeInAir < COYOTE:
		return true
	elif jumpCount < extraJumps:
		if Input.is_action_just_pressed("jump"):
			jumpCount += 1
		return true
	else:
		return false

func detectFloor(delta: float):
	if floor_sensor.is_colliding():
		#Reinventing Physics
		var hitPos = floor_sensor.get_collision_point(0)
		var sensorPos = floor_sensor.global_position
		var hitNorm = floor_sensor.get_collision_normal(0)
		var rayLength = floor_sensor.target_position.length()
		
		var hitDist = hitPos.distance_to(sensorPos)
		var hitDepth = rayLength - hitDist
		
		#Smooth force
		if hitDepth > 0:
			position = smoothMove(position, position + Vector2.UP * hitDepth, 10.0, 20.0, delta)
			velocity.y = clampf(velocity.y, -99999, 500)
			velocity = smoothMove(velocity, velocity * Vector2(1,0), 0, 2000.0, delta)
		onFloor = true
	else:
		onFloor = false

#Accelleration Penalties
func calcAccPenalty(delta) -> void:
	accPenaltyWall = move_toward(accPenaltyWall, 1, delta)
	accPenalty = accPenaltyWall

func _on_wall_body_entered(body: Node2D) -> void:
	accPenaltyWall = 0.25
	velocity.y *= 0.5



#Camera Stuff
func moveCamera(delta: float) -> void:
	var move_target_offset = Vector2.ZERO
																#Max speed       Max Offset
	move_target_offset.x = sign(velocity.x) * ease(abs(velocity.x)/10000, 0.2) * 450 #Diminishing returns of speed to cam
	move_target_offset.y = sign(velocity.y) * ease(abs(velocity.y)/10000, 0.4) * 250 #also effectively clamps max offset
	
	var input_target_offset = Vector2.ZERO
	input_target_offset = Vector2(Input.get_axis("look left","look right"),Input.get_axis("look up","look down")).normalized()
	input_target_offset *= 100
	
	movement_camOffset = smoothMove(movement_camOffset, move_target_offset, 2.0, 10.0, delta)
	input_camOffset = smoothMove(input_camOffset, input_target_offset, 3.0, 10.0, delta)
	
	$Camera2D.offset = movement_camOffset + input_camOffset

func smoothMove(pos: Vector2, target: Vector2, damp: float, linear: float, delta: float) -> Vector2:
	var newPos = pos
	newPos = newPos.lerp(target, 1.0 - exp(-damp * delta))
	newPos = newPos.move_toward(target, linear * delta)
	return newPos
