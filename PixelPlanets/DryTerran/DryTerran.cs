using Godot;
using System;

public partial class DryTerran : Planet
{
	public override float Time {get; set;}
	public override bool OverrideTime {get; set;}
	public override Color[] OriginalColors {get; set;}

	[Export]
	public override float RelativeScale {get; set;}
	[Export]
	public override float GUIZoom {get; set;}

	private ColorRect LandColorRect;

	public override void _Ready()
	{
		LandColorRect = GetNode<ColorRect>("Land");
		LandColorRect.Material = (Material)LandColorRect.Material.Duplicate(true);
		base._Ready();
	}

	public override void SetPixels(int _amount)
	{
		ShaderMaterial _mat = (ShaderMaterial)LandColorRect.Material;
		_mat.SetShaderParameter("pixels", _amount);
		LandColorRect.Material = _mat;
		LandColorRect.Size = new Vector2(_amount, _amount);
	}

	public override void SetLight(Vector2 _pos)
	{
		ShaderMaterial _mat = (ShaderMaterial)LandColorRect.Material;
		_mat.SetShaderParameter("light_origin", _pos);
		LandColorRect.Material = _mat;
	}

	public override void SetSeed(uint _seed)
	{
		float converted_seed = _seed % 1000 / 100.0f;
		ShaderMaterial _mat = (ShaderMaterial)LandColorRect.Material;
		_mat.SetShaderParameter("seed", converted_seed);
		LandColorRect.Material = _mat;
	}

	public override void SetPlanetRotation(float _rotation)
	{
		ShaderMaterial _mat = (ShaderMaterial)LandColorRect.Material;
		_mat.SetShaderParameter("rotation", _rotation);
		LandColorRect.Material = _mat;
	}

	public override void UpdateTime(float _time)
	{
		ShaderMaterial _mat = (ShaderMaterial)LandColorRect.Material;
		_mat.SetShaderParameter("time", _time * GetMultiplier(_mat) * 0.02f);
		LandColorRect.Material = _mat;
	}

	public override void SetCustomTime(float _time)
	{
		ShaderMaterial _mat = (ShaderMaterial)LandColorRect.Material;
		_mat.SetShaderParameter("time", _time * GetMultiplier(_mat));
		LandColorRect.Material = _mat;
	}

	public override Color[] GetColors()
	{
		return GetColorsFromShader((ShaderMaterial)LandColorRect.Material);
	}

	public override void SetColors(Color[] _colors)
	{
		ShaderMaterial new_mat = SetColorsOnShader((ShaderMaterial)LandColorRect.Material, _colors);
		LandColorRect.Material = new_mat;
	}
}
