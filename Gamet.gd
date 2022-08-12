extends Node2D


# Declare member variables here. Examples:
var Offset = 0.11
var size  = 0.11
var table
var font

func create_2d_array(width, height):
	var a = []

	for y in range(height):
		a.append([])
		a[y].resize(width)

		for x in range(width):
			a[y][x] = 2 ^ (y + x)

	return a

# Called when the node enters the scene tree for the first time.
func _ready():
	table  = create_2d_array(4,4)
	font = DynamicFont.new()
	font.font_data = load("res://Hack.ttf")
	font.size = 20
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	update() 

func _draw():
	draw_string(font,Vector2(10,10),"Hey",Color.black)
