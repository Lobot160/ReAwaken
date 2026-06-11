extends CanvasLayer

class_name choose_loot

@onready var panel1: Panel = $MarginContainer/HBoxContainer/Panel
@onready var panel2: Panel = $MarginContainer/HBoxContainer/Panel2
@onready var panel3: Panel = $MarginContainer/HBoxContainer/Panel3

@onready var label1: Label = $MarginContainer/HBoxContainer/Panel/Label
@onready var label2: Label = $MarginContainer/HBoxContainer/Panel2/Label2
@onready var label3: Label = $MarginContainer/HBoxContainer/Panel3/Label3


var targets = []
var target = 0

var choices = []

const DARK = preload("res://Themes/dark_panel.tres")
const HIGHLIGHTED = preload("res://Themes/highlighted_panel.tres")

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	targets = [panel1, panel2, panel3]
	change_loot()
	hide_loot()


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	if Input.is_action_just_pressed("left") or Input.is_action_just_pressed("right"):
		targets[target].theme = DARK
		move_target()
		targets[target].theme = HIGHLIGHTED
	elif Input.is_action_just_pressed("jump"):
		print(choices[target] + " Was chosen")
		Inventory.add(choices[target])
		get_tree().paused = false
		self.visible = false
		print(Inventory.items)
		queue_free()

func change_loot() -> void:
	label1.text = choices[0]
	label2.text = choices[1]
	label3.text = choices[2]

func move_target() -> void:
	target = (target + int(Input.get_axis("left", "right"))) % 3
	while not targets[target].is_visible_in_tree():
		target = (target + int(Input.get_axis("left", "right"))) % 3


func hide_loot() -> void:
	for i in range(3):
		if choices[i] in Inventory.items:
			targets[i].visible = false
			if i == target:
				target += 1
