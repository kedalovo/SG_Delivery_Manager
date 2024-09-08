extends "res://PixelPlanets/Planet.gd"

func set_pixels(amount):
	$Land.material.set_shader_parameter("pixels", amount)
	$Lakes.material.set_shader_parameter("pixels", amount)
	$Clouds.material.set_shader_parameter("pixels", amount)
	
	$Land.size = Vector2(amount, amount)
	$Lakes.size = Vector2(amount, amount)
	$Clouds.size = Vector2(amount, amount)

func set_light(pos):
	$Land.material.set_shader_parameter("light_origin", pos)
	$Lakes.material.set_shader_parameter("light_origin", pos)
	$Clouds.material.set_shader_parameter("light_origin", pos)

func set_seed(sd):
	var converted_seed = sd%1000/100.0
	$Land.material.set_shader_parameter("seed", converted_seed)
	$Lakes.material.set_shader_parameter("seed", converted_seed)
	$Clouds.material.set_shader_parameter("seed", converted_seed)

func set_rotates(r):
	$Land.material.set_shader_parameter("rotation", r)
	$Lakes.material.set_shader_parameter("rotation", r)
	$Clouds.material.set_shader_parameter("rotation", r)

func update_time(t):
	$Land.material.set_shader_parameter("time", t * get_multiplier($Land.material) * 0.02)
	$Lakes.material.set_shader_parameter("time", t * get_multiplier($Lakes.material) * 0.02)
	$Clouds.material.set_shader_parameter("time", t * get_multiplier($Clouds.material) * 0.01)

func set_custom_time(t):
	$Land.material.set_shader_parameter("time", t * get_multiplier($Land.material))
	$Lakes.material.set_shader_parameter("time", t * get_multiplier($Lakes.material))
	$Clouds.material.set_shader_parameter("time", t * get_multiplier($Clouds.material))

func set_dither(d):
	$Land.material.set_shader_parameter("should_dither", d)

func get_dither():
	return $Land.material.get_shader_parameter("should_dither")

func get_colors():
	return get_colors_from_shader($Land.material) + get_colors_from_shader($Lakes.material) + get_colors_from_shader($Clouds.material)

func set_colors(colors):
	set_colors_on_shader($Land.material, colors.slice(0, 3))
	set_colors_on_shader($Lakes.material, colors.slice(3, 6))
	set_colors_on_shader($Clouds.material, colors.slice(6, 10))

func randomize_colors():
	var seed_colors = _generate_new_colorscheme(randi()%2+3, randf_range(0.7, 1.0), randf_range(0.45, 0.55))
	var land_colors = []
	var lake_colors = []
	var cloud_colors = []
	for i in 3:
		var new_col = seed_colors[0].darkened(i/3.0)
		land_colors.append(Color.from_hsv(new_col.h + (0.2 * (i/4.0)), new_col.s, new_col.v))
	
	for i in 3:
		var new_col = seed_colors[1].darkened(i/3.0)
		lake_colors.append(Color.from_hsv(new_col.h + (0.2 * (i/3.0)), new_col.s, new_col.v))
	
	for i in 4:
		var new_col = seed_colors[2].lightened((1.0 - (i/4.0)) * 0.8)
		cloud_colors.append(Color.from_hsv(new_col.h + (0.2 * (i/4.0)), new_col.s, new_col.v))

	set_colors(land_colors + lake_colors + cloud_colors)
