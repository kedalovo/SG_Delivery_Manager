using Godot;
using System;

public partial class Galaxy : Planet
{
    public override float Time {get; set;}
	public override bool OverrideTime {get; set;}
	public override Color[] OriginalColors {get; set;}

	[Export]
	public override float RelativeScale {get; set;}
	[Export]
	public override float GUIZoom {get; set;}

	private ColorRect GalaxyColorRect;

	public override void _Ready()
	{
		GalaxyColorRect = GetNode<ColorRect>("Galaxy");
		base._Ready();
	}

	public override void SetPixels(int _amount)
	{
		ShaderMaterial _mat = (ShaderMaterial)GalaxyColorRect.Material;
		_mat.SetShaderParameter("pixels", _amount);
		GalaxyColorRect.Material = _mat;
		GalaxyColorRect.Size = new Vector2(_amount, _amount);
	}

	public override void SetLight(Vector2 _pos)
	{
		
	}

	public override void SetSeed(uint _seed)
	{
		float converted_seed = _seed % 1000 / 100.0f;
		ShaderMaterial _mat = (ShaderMaterial)GalaxyColorRect.Material;
		_mat.SetShaderParameter("seed", converted_seed);
		GalaxyColorRect.Material = _mat;
	}

	public override void SetRotation(float _rotation)
	{
		ShaderMaterial _mat = (ShaderMaterial)GalaxyColorRect.Material;
		_mat.SetShaderParameter("rotation", _rotation);
		GalaxyColorRect.Material = _mat;
	}

	public override void UpdateTime(float _time)
	{
        ShaderMaterial _mat = (ShaderMaterial)GalaxyColorRect.Material;
		_mat.SetShaderParameter("time", _time * GetMultiplier(_mat) * 0.04f);
		GalaxyColorRect.Material = _mat;
	}

	public override void SetCustomTime(float _time)
	{
		ShaderMaterial _mat = (ShaderMaterial)GalaxyColorRect.Material;
		_mat.SetShaderParameter("time", _time * (float)_mat.GetShaderParameter("time_speed") * MathF.PI * 2.0f);
		GalaxyColorRect.Material = _mat;
	}

	public override Color[] GetColors()
	{
		return GetColorsFromShader((ShaderMaterial)GalaxyColorRect.Material);
	}

	public override void SetColors(Color[] _colors)
    {
		ShaderMaterial new_mat = SetColorsOnShader((ShaderMaterial)GalaxyColorRect.Material, _colors);
		GalaxyColorRect.Material = new_mat;
	}
}
