extends Node2D

const loopChance = 0.5
var enabled_edges = []

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	gen_graph()
	random_graph()


func gen_graph() -> void:
	for verticy in $Vertices.get_children():
		for edge in verticy.get_meta("Connections"):
			gen_edge(verticy.name, edge)

func gen_edge(a: String, b: String) -> void:
	name = gen_edge_name(a,b)
	if check_dupe(name):
		return
	var edge = Line2D.new()
	$Edges.add_child(edge)
	var vert_a = $Vertices.get_child(int(a))
	var vert_b = $Vertices.get_child(int(b))
	
	edge.name = name
	edge.add_point(vert_a.position)
	edge.add_point(vert_b.position)

func gen_edge_name(a: String, b: String) -> String:
	return str(min(int(a),int(b))) + "-" + str(max(int(a),int(b)))

func check_dupe(name: String) -> bool:
	var edges = $Edges.get_children()
	for edge in edges:
		if edge.name == name:
			return true
	return false

func random_graph():
	const graph_tries = 5000
	for i in range(graph_tries):
		for verticy in $Vertices.get_children():
			verticy.set_meta("degree", 0)
			verticy.set_meta("parent", verticy)
		enabled_edges = []
		var result = try_random_graph()
		
		if result:
			print("it took " + str(i) + " Tries to generate the graph!")
			for edge in $Edges.get_children():
				if edge in enabled_edges:
					edge.visible = true
				else:
					edge.visible = false
			return
	print("graph did not generate")

func try_random_graph() -> bool:
	
	var rand_edges = $Edges.get_children()
	rand_edges.shuffle()
	
	for edge in rand_edges:
		var verticies = get_verticies(edge.name)
		var vert_a = verticies[0]
		var vert_b = verticies[1]
		
		if find(vert_a) != find(vert_b) && is_edge_valid(edge):
			enable_edge(edge)
			union(vert_a,vert_b)
	
	#check min
	for node in $Vertices.get_children():
		var valid_edges = node.get_meta("Connections")
		valid_edges.shuffle()
		var i = 0
		while node.get_meta("degree") < node.get_meta("min_degree"):
			if i >= valid_edges.size():
				return false
				
			var edge = get_edge_2(valid_edges[i], node.name)
			if is_edge_valid(edge):
				enable_edge(edge)
			i += 1
	
	#add loops
	rand_edges.shuffle()
	for edge in rand_edges:
		
		if is_edge_valid(edge) && randf() < loopChance:
			enable_edge(edge)
	return true

func enable_edge(edge: Line2D):
	var verticies = get_verticies(edge.name)
	var vert_a = verticies[0]
	var vert_b = verticies[1]
	enabled_edges.append(edge)
	vert_a.set_meta("degree", vert_a.get_meta("degree")+1)
	vert_b.set_meta("degree", vert_b.get_meta("degree")+1)

func get_edge(edge: String) -> Line2D:
	for line in $Edges.get_children():
		if line.name == edge:
			return line
	return null

func get_edge_2(a: String, b: String) -> Line2D:
	return get_edge(gen_edge_name(a,b))

func is_edge_valid(edge: Line2D) -> bool:
	var verticies = get_verticies(edge.name)
	var vert_a = verticies[0]
	var vert_b = verticies[1]
	return edge not in enabled_edges && vert_a.get_meta("degree") < vert_a.get_meta("Max_edges") && vert_b.get_meta("degree") < vert_b.get_meta("Max_edges") && not check_edge_collision(edge,enabled_edges)

func get_verticies(edge: String) -> Array:
	var vertex = []
	vertex.append($Vertices.find_child(edge[0]))
	vertex.append($Vertices.find_child(edge[2]))
	return vertex

#checks if a edge intersects with a given set of edges
func check_edge_collision(edge: Node, list) -> bool:
	for item in list:
		var line2 = item
		var intersection = Geometry2D.segment_intersects_segment(edge.get_point_position(0),edge.get_point_position(1),line2.get_point_position(0),line2.get_point_position(1))
		if intersection:
			#if they are linked
			if intersection not in [edge.get_point_position(0),edge.get_point_position(1),line2.get_point_position(0),line2.get_point_position(1)]:
				return true
	return false

func find(x: Node2D) -> Node2D:
	var parent = x.get_meta("parent")
	if parent != x:
		parent = find(parent)
	return parent

func union(a: Node2D, b: Node2D):
	var root_a = find(a)
	var root_b = find(b)
	if root_a == root_b:
		return false
	
	if int(str(root_a.name)) < int(str(root_b.name)):
		root_a.set_meta("parent", root_b)
	else:
		root_b.set_meta("parent", root_a)
	return true

func _input(event: InputEvent) -> void:
	if Input.is_action_just_pressed("debug map"):
		random_graph()
