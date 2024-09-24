using Godot;
using System;

public partial class Asteroid : Planet
{
	public override float Time {get; set;}
	public override bool OverrideTime {get; set;}
	public override Color[] OriginalColors {get; set;}

	[Export]
	public override float RelativeScale {get; set;}
	[Export]
	public override float GUIZoom {get; set;}

	private ColorRect AsteroidColorRect;

	public override void _Ready()
	{
		AsteroidColorRect = GetNode<ColorRect>("Asteroid");
		AsteroidColorRect.Material = (Material)AsteroidColorRect.Material.Duplicate(true);
		base._Ready();
	}

	public override void SetPixels(int _amount)
	{
		ShaderMaterial _mat = (ShaderMaterial)AsteroidColorRect.Material;
		_mat.SetShaderParameter("pixels", _amount);
		AsteroidColorRect.Material = _mat;
		AsteroidColorRect.Size = new Vector2(_amount, _amount);
	}

	public override void SetLight(Vector2 _pos)
	{
		ShaderMaterial _mat = (ShaderMaterial)AsteroidColorRect.Material;
		_mat.SetShaderParameter("light_origin", _pos);
		AsteroidColorRect.Material = _mat;
	}

	public override void SetSeed(uint _seed)
	{
		float converted_seed = _seed % 1000 / 100.0f;
		ShaderMaterial _mat = (ShaderMaterial)AsteroidColorRect.Material;
		_mat.SetShaderParameter("seed", converted_seed);
		AsteroidColorRect.Material = _mat;
	}

	public override void SetRotation(float _rotation)
	{
		ShaderMaterial _mat = (ShaderMaterial)AsteroidColorRect.Material;
		_mat.SetShaderParameter("rotation", _rotation);
		AsteroidColorRect.Material = _mat;
	}

	public override void UpdateTime(float _time)
	{

	}

	public override void SetCustomTime(float _time)
	{
		ShaderMaterial _mat = (ShaderMaterial)AsteroidColorRect.Material;
		_mat.SetShaderParameter("rotation", _time * MathF.PI * 2.0f);
		AsteroidColorRect.Material = _mat;
	}

	public override Color[] GetColors()
	{
		return GetColorsFromShader((ShaderMaterial)AsteroidColorRect.Material);
	}

	public override void SetColors(Color[] _colors)
	{
		ShaderMaterial new_mat = SetColorsOnShader((ShaderMaterial)AsteroidColorRect.Material, _colors);
		AsteroidColorRect.Material = new_mat;
	}
}
