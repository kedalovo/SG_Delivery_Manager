extends "res://PixelPlanets/Planet.gd"

func set_pixels(amount):
	$GasLayers.material.set_shader_parameter("pixels", amount)
	 # times 3 here because in this case ring is 3 times larger than planet
	$Ring.material.set_shader_parameter("pixels", amount*3.0)
	
	$GasLayers.size = Vector2(amount, amount)
	$Ring.position = Vector2(-amount, -amount)
	$Ring.size = Vector2(amount, amount)*3.0

func set_light(pos):
	$GasLayers.material.set_shader_parameter("light_origin", pos)
	$Ring.material.set_shader_parameter("light_origin", pos)

func set_seed(sd):
	var converted_seed = sd%1000/100.0
	$GasLayers.material.set_shader_parameter("seed", converted_seed)
	$Ring.material.set_shader_parameter("seed", converted_seed)

func set_rotates(r):
	$GasLayers.material.set_shader_parameter("rotation", r)
	$Ring.material.set_shader_parameter("rotation", r+0.7)

func update_time(t):
	$GasLayers.material.set_shader_parameter("time", t * get_multiplier($GasLayers.material) * 0.004)
	$Ring.material.set_shader_parameter("time", t * 314.15 * 0.004)

func set_custom_time(t):
	$GasLayers.material.set_shader_parameter("time", t * get_multiplier($GasLayers.material))
	$Ring.material.set_shader_parameter("time", t * 314.15 * $Ring.material.get_shader_parameter("time_speed") * 0.5)

func set_dither(d):
	$GasLayers.material.set_shader_parameter("should_dither", d)

func get_dither():
	return $GasLayers.material.get_shader_parameter("should_dither")


func get_colors():
	return get_colors_from_shader($GasLayers.material) + get_colors_from_shader($GasLayers.material, "dark_colors") + get_colors_from_shader($Ring.material) + get_colors_from_shader($Ring.material, "dark_colors")

func set_colors(colors):
	var cols1 = colors.slice(0, 3)
	var cols2 = colors.slice(3, 6)
	
	set_colors_on_shader($GasLayers.material, cols1)
	set_colors_on_shader($Ring.material, cols1)
	
	set_colors_on_shader($GasLayers.material, cols2, "dark_colors")
	set_colors_on_shader($Ring.material, cols2, "dark_colors")

func randomize_colors():
	var seed_colors = _generate_new_colorscheme(6 + randi() % 4, randf_range(0.3,0.55), 1.4)
	var cols = []
	for i in 6:
		var new_col = seed_colors[i].darkened(i/7.0)
		new_col = new_col.lightened((1.0 - (i/6.0)) * 0.3)
		cols.append(new_col)

	set_colors(cols)
