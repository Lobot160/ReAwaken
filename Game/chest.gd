extends Node2D

class_name chest
var active = false
var selecting = false
var target = 0

var loot = ["Loot A", "Loot B", "Loot C"]

@onready var input_label: Label = $"Input Label"
const LOOT_CHOICE = preload("res://Game/loot_choice.tscn")


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass

#change
# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	if Input.is_action_just_pressed("select") && active:
		var screen = LOOT_CHOICE.instantiate()
		screen.choices = loot
		input_label.visible = false
		add_child(screen)
		get_tree().paused = true



func _on_player_area_entered(body: Node2D) -> void:
	active = true
	input_label.visible = true


func _on_player_area_exited(body: Node2D) -> void:
	active = false
	input_label.visible = false
