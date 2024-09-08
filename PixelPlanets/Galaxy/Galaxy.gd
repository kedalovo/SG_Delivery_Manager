extends "res://PixelPlanets/Planet.gd"

func set_pixels(amount):
	$Galaxy.material.set_shader_parameter("pixels", amount)
	$Galaxy.size = Vector2(amount, amount) 

func set_light(_pos):
	pass

func set_seed(sd):
	var converted_seed = sd%1000/100.0
	$Galaxy.material.set_shader_parameter("seed", converted_seed)

func set_rotates(r):
	$Galaxy.material.set_shader_parameter("rotation", r)

func update_time(t):
	$Galaxy.material.set_shader_parameter("time", t * get_multiplier($Galaxy.material) * 0.04)

func set_custom_time(t):
	$Galaxy.material.set_shader_parameter("time", t * PI * 2 * $Galaxy.material.get_shader_parameter("time_speed"))

func set_dither(d):
	$Galaxy.material.set_shader_parameter("should_dither", d)

func get_dither():
	return $Galaxy.material.get_shader_parameter("should_dither")

func get_colors():
	return get_colors_from_shader($Galaxy.material)

func set_colors(colors):
	set_colors_on_shader($Galaxy.material, colors)

func randomize_colors():
	var seed_colors = _generate_new_colorscheme(6 , randf_range(0.5,0.8), 1.4)
	var cols = []
	for i in 6:
		var new_col = seed_colors[i].darkened(i/7.0)
		new_col = new_col.lightened((1.0 - (i/6.0)) * 0.6)
		cols.append(new_col)

	set_colors(cols)
