using Godot;
using System;
using System.Linq;

public partial class GasPlanet : Planet
{
	public override float Time {get; set;}
	public override bool OverrideTime {get; set;}
	public override Color[] OriginalColors {get; set;}

	[Export]
	public override float RelativeScale {get; set;}
	[Export]
	public override float GUIZoom {get; set;}

	private ColorRect CloudColorRect;
	private ColorRect Cloud2ColorRect;

	public override void _Ready()
	{
		CloudColorRect = GetNode<ColorRect>("Cloud");
		Cloud2ColorRect = GetNode<ColorRect>("Cloud2");
		CloudColorRect.Material = (Material)CloudColorRect.Material.Duplicate(true);
		Cloud2ColorRect.Material = (Material)Cloud2ColorRect.Material.Duplicate(true);
		base._Ready();
	}

	public override void SetPixels(int _amount)
	{
		ShaderMaterial _mat = (ShaderMaterial)CloudColorRect.Material;
		ShaderMaterial _mat2 = (ShaderMaterial)Cloud2ColorRect.Material;

		_mat.SetShaderParameter("pixels", _amount);
		CloudColorRect.Material = _mat;
		CloudColorRect.Size = new Vector2(_amount, _amount);

		_mat2.SetShaderParameter("pixels", _amount);
		Cloud2ColorRect.Material = _mat2;
		Cloud2ColorRect.Size = new Vector2(_amount, _amount);
	}

	public override void SetLight(Vector2 _pos)
	{
		ShaderMaterial _mat = (ShaderMaterial)CloudColorRect.Material;
		_mat.SetShaderParameter("light_origin", _pos);
		CloudColorRect.Material = _mat;

		ShaderMaterial _mat2 = (ShaderMaterial)Cloud2ColorRect.Material;
		_mat2.SetShaderParameter("light_origin", _pos);
		Cloud2ColorRect.Material = _mat2;
	}

	public override void SetSeed(uint _seed)
	{
		float converted_seed = _seed % 1000 / 100.0f;
		ShaderMaterial _mat = (ShaderMaterial)CloudColorRect.Material;
		_mat.SetShaderParameter("seed", converted_seed);
		CloudColorRect.Material = _mat;

		ShaderMaterial _mat2 = (ShaderMaterial)Cloud2ColorRect.Material;
		_mat2.SetShaderParameter("seed", converted_seed);
		_mat2.SetShaderParameter("cloud_cover", new Random().NextSingle() / 4.0f + 0.25f);
		Cloud2ColorRect.Material = _mat2;
	}

	public override void SetPlanetRotation(float _rotation)
	{
		ShaderMaterial _mat = (ShaderMaterial)CloudColorRect.Material;
		_mat.SetShaderParameter("rotation", _rotation);
		CloudColorRect.Material = _mat;

		ShaderMaterial _mat2 = (ShaderMaterial)Cloud2ColorRect.Material;
		_mat2.SetShaderParameter("rotation", _rotation);
		Cloud2ColorRect.Material = _mat2;
	}

	public override void UpdateTime(float _time)
	{
		ShaderMaterial _mat = (ShaderMaterial)CloudColorRect.Material;
		_mat.SetShaderParameter("time", _time * GetMultiplier(_mat) * 0.005f);
		CloudColorRect.Material = _mat;

		ShaderMaterial _mat2 = (ShaderMaterial)Cloud2ColorRect.Material;
		_mat2.SetShaderParameter("time", _time * GetMultiplier(_mat2) * 0.005f);
		Cloud2ColorRect.Material = _mat2;
	}

	public override void SetCustomTime(float _time)
	{
		ShaderMaterial _mat = (ShaderMaterial)CloudColorRect.Material;
		_mat.SetShaderParameter("time", _time * GetMultiplier(_mat));
		CloudColorRect.Material = _mat;
		
		ShaderMaterial _mat2 = (ShaderMaterial)Cloud2ColorRect.Material;
		_mat2.SetShaderParameter("time", _time * GetMultiplier(_mat2));
		Cloud2ColorRect.Material = _mat2;
	}

	public override Color[] GetColors()
	{
		Color[] _colors = Array.Empty<Color>();
		foreach (Color color in GetColorsFromShader((ShaderMaterial)CloudColorRect.Material)) _colors = _colors.Append(color).ToArray();
		foreach (Color color in GetColorsFromShader((ShaderMaterial)Cloud2ColorRect.Material)) _colors = _colors.Append(color).ToArray();
		return _colors;
	}

	public override void SetColors(Color[] _colors)
	{
		Color[] cols1 = _colors[0..4];
		Color[] cols2 = _colors[4..8];
		
		ShaderMaterial _mat = (ShaderMaterial)CloudColorRect.Material;
		ShaderMaterial _mat2 = (ShaderMaterial)Cloud2ColorRect.Material;
		
		ShaderMaterial new_mat = SetColorsOnShader(_mat, cols1);
		ShaderMaterial new_mat2 = SetColorsOnShader(_mat2, cols2);

		CloudColorRect.Material = new_mat;
		Cloud2ColorRect.Material = new_mat2;
	}
}
