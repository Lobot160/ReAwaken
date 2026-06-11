extends RichTextLabel

const TALKSPEED: float = 0.1 #time between text

var talkTime: float = 0
var currCharact: int = 0

var dialogue = "res://dialogue/test.json"
# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	visible_characters = 0
	var file = FileAccess.open(dialogue, FileAccess.READ)
	var data = JSON.parse_string((file.get_as_text()))
	#text = data.text
	install_effect(RichTextSudden.new())
	SignalBus.skipChar.connect(_skipChar)
	


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:

	talkTime += delta
	
	if talkTime >= TALKSPEED:
		currCharact += 1
		visible_characters = currCharact
		talkTime = 0

func _skipChar(count: int) -> void:
	currCharact += count
