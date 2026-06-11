@tool
class_name RichTextSudden extends RichTextEffect

var bbcode = "sudden"

var once = true


func _init() -> void:
	pass

func _process_custom_fx(char_fx: CharFXTransform) -> bool:
	if once:
		SignalBus.skipChar.emit(0)
		once=false

	
	return true
