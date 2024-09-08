extends "res://PixelPlanets/Planet.gd"

func set_pixels(amount):	
	$Water.material.set_shader_parameter("pixels", amount)
	$Land.material.set_shader_parameter("pixels", amount)
	$Cloud.material.set_shader_parameter("pixels", amount)
	
	$Water.size = Vector2(amount, amount)
	$Land.size = Vector2(amount, amount)
	$Cloud.size = Vector2(amount, amount)

func set_light(pos):
	$Cloud.material.set_shader_parameter("light_origin", pos)
	$Water.material.set_shader_parameter("light_origin", pos)
	$Land.material.set_shader_parameter("light_origin", pos)

func set_seed(sd):
	var converted_seed = sd%1000/100.0
	$Cloud.material.set_shader_parameter("seed", converted_seed)
	$Water.material.set_shader_parameter("seed", converted_seed)
	$Land.material.set_shader_parameter("seed", converted_seed)
	$Cloud.material.set_shader_parameter("cloud_cover", randf_range(0.35, 0.6))

func set_rotates(r):
	$Cloud.material.set_shader_parameter("rotation", r)
	$Water.material.set_shader_parameter("rotation", r)
	$Land.material.set_shader_parameter("rotation", r)

func update_time(t):
	$Cloud.material.set_shader_parameter("time", t * get_multiplier($Cloud.material) * 0.01)
	$Water.material.set_shader_parameter("time", t * get_multiplier($Water.material) * 0.02)
	$Land.material.set_shader_parameter("time", t * get_multiplier($Land.material) * 0.02)

func set_custom_time(t):
	$Cloud.material.set_shader_parameter("time", t * get_multiplier($Cloud.material))
	$Water.material.set_shader_parameter("time", t * get_multiplier($Water.material))
	$Land.material.set_shader_parameter("time", t * get_multiplier($Land.material))

func set_dither(d):
	$Water.material.set_shader_parameter("should_dither", d)

func get_dither():
	return $Water.material.get_shader_parameter("should_dither")


func get_colors():
	return get_colors_from_shader($Water.material) + get_colors_from_shader($Land.material) + get_colors_from_shader($Cloud.material)

func set_colors(colors):
	set_colors_on_shader($Water.material, colors.slice(0, 3))
	set_colors_on_shader($Land.material, colors.slice(3, 7))
	set_colors_on_shader($Cloud.material, colors.slice(7, 11))

func randomize_colors():
	var seed_colors = _generate_new_colorscheme(randi()%2+3, randf_range(0.7, 1.0), randf_range(0.45, 0.55))
	var land_colors = []
	var water_colors = []
	var cloud_colors = []
	for i in 4:
		var new_col = seed_colors[0].darkened(i/4.0)
		land_colors.append(Color.from_hsv(new_col.h + (0.2 * (i/4.0)), new_col.s, new_col.v))
	
	for i in 3:
		var new_col = seed_colors[1].darkened(i/5.0)
		water_colors.append(Color.from_hsv(new_col.h + (0.1 * (i/2.0)), new_col.s, new_col.v))
	
	for i in 4:
		var new_col = seed_colors[2].lightened((1.0 - (i/4.0)) * 0.8)
		cloud_colors.append(Color.from_hsv(new_col.h + (0.2 * (i/4.0)), new_col.s, new_col.v))

	set_colors(water_colors + land_colors + cloud_colors)
