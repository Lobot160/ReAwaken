extends Node2D

const LOOT = ["Loot A","Loot B","Loot C","Loot D"]
var lootamt = {"Loot A": 0,"Loot B": 0,"Loot C": 0,"Loot D": 0}

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	randomize_loot()


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass

func try_loot() -> bool:
	for box in get_children():
		var loot_pool = LOOT.duplicate()
		loot_pool.shuffle()
		for i in range(3):
			var valid = false
			var choice = ""
			while not valid:
				choice = loot_pool.pop_front()
				if choice == null:
					return false
				if lootamt[choice] < 3:
					valid = true
					lootamt[choice] += 1
			box.loot[i] = choice
	return true

func randomize_loot():
	for i in range(5000):
		if try_loot():
			return
