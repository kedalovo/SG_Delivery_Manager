using Godot;
using System;
using System.Linq;

public partial class Rivers : Planet
{
	public override float Time {get; set;}
	public override bool OverrideTime {get; set;}
	public override Color[] OriginalColors {get; set;}

	[Export]
	public override float RelativeScale {get; set;}
	[Export]
	public override float GUIZoom {get; set;}

	private ColorRect LandColorRect;
	private ColorRect CloudColorRect;

	public override void _Ready()
	{
		LandColorRect = GetNode<ColorRect>("Land");
		CloudColorRect = GetNode<ColorRect>("Cloud");
		LandColorRect.Material = (Material)LandColorRect.Material.Duplicate(true);
		CloudColorRect.Material = (Material)CloudColorRect.Material.Duplicate(true);
		base._Ready();
	}

	public override void SetPixels(int _amount)
	{
		ShaderMaterial _mat = (ShaderMaterial)LandColorRect.Material;
		ShaderMaterial _mat2 = (ShaderMaterial)CloudColorRect.Material;

		_mat.SetShaderParameter("pixels", _amount);
		LandColorRect.Material = _mat;
		LandColorRect.Size = new Vector2(_amount, _amount);

		_mat2.SetShaderParameter("pixels", _amount);
		CloudColorRect.Material = _mat2;
		CloudColorRect.Size = new Vector2(_amount, _amount);
	}

	public override void SetLight(Vector2 _pos)
	{
		ShaderMaterial _mat = (ShaderMaterial)LandColorRect.Material;
		_mat.SetShaderParameter("light_origin", _pos);
		LandColorRect.Material = _mat;

		ShaderMaterial _mat2 = (ShaderMaterial)CloudColorRect.Material;
		_mat2.SetShaderParameter("light_origin", _pos);
		CloudColorRect.Material = _mat2;
	}

	public override void SetSeed(uint _seed)
	{
		float converted_seed = _seed % 1000 / 100.0f;
		ShaderMaterial _mat = (ShaderMaterial)LandColorRect.Material;
		_mat.SetShaderParameter("seed", converted_seed);
		LandColorRect.Material = _mat;

		ShaderMaterial _mat2 = (ShaderMaterial)CloudColorRect.Material;
		_mat2.SetShaderParameter("seed", converted_seed);
		_mat2.SetShaderParameter("cloud_cover", new Random().NextSingle() / 3.0f + 0.35f);
		CloudColorRect.Material = _mat2;
	}

	public override void SetPlanetRotation(float _rotation)
	{
		ShaderMaterial _mat = (ShaderMaterial)LandColorRect.Material;
		_mat.SetShaderParameter("rotation", _rotation);
		LandColorRect.Material = _mat;

		ShaderMaterial _mat2 = (ShaderMaterial)CloudColorRect.Material;
		_mat2.SetShaderParameter("rotation", _rotation);
		CloudColorRect.Material = _mat2;
	}

	public override void UpdateTime(float _time)
	{
		ShaderMaterial _mat = (ShaderMaterial)LandColorRect.Material;
		_mat.SetShaderParameter("time", _time * GetMultiplier(_mat) * 0.02f);
		LandColorRect.Material = _mat;

		ShaderMaterial _mat2 = (ShaderMaterial)CloudColorRect.Material;
		_mat2.SetShaderParameter("time", _time * GetMultiplier(_mat2) * 0.01f);
		CloudColorRect.Material = _mat2;
	}

	public override void SetCustomTime(float _time)
	{
		ShaderMaterial _mat = (ShaderMaterial)LandColorRect.Material;
		_mat.SetShaderParameter("time", _time * GetMultiplier(_mat));
		LandColorRect.Material = _mat;
		
		ShaderMaterial _mat2 = (ShaderMaterial)CloudColorRect.Material;
		_mat2.SetShaderParameter("time", _time * GetMultiplier(_mat2) * 0.5f);
		CloudColorRect.Material = _mat2;
	}

	public override Color[] GetColors()
	{
		Color[] _colors = Array.Empty<Color>();
		foreach (Color color in GetColorsFromShader((ShaderMaterial)LandColorRect.Material)) _colors = _colors.Append(color).ToArray();
		foreach (Color color in GetColorsFromShader((ShaderMaterial)CloudColorRect.Material)) _colors = _colors.Append(color).ToArray();
		return _colors;
	}

	public override void SetColors(Color[] _colors)
	{
		Color[] cols1 = _colors[0..6];
		Color[] cols2 = _colors[6..10];
		
		ShaderMaterial _mat = (ShaderMaterial)LandColorRect.Material;
		ShaderMaterial _mat2 = (ShaderMaterial)CloudColorRect.Material;
		
		ShaderMaterial new_mat = SetColorsOnShader(_mat, cols1);
		ShaderMaterial new_mat2 = SetColorsOnShader(_mat2, cols2);

		LandColorRect.Material = new_mat;
		CloudColorRect.Material = new_mat2;
	}
}
