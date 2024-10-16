using Godot;
using System;
using System.Linq;

public partial class IceWorld : Planet
{
	public override float Time {get; set;}
	public override bool OverrideTime {get; set;}
	public override Color[] OriginalColors {get; set;}

	[Export]
	public override float RelativeScale {get; set;}
	[Export]
	public override float GUIZoom {get; set;}

	private ColorRect LandColorRect;
	private ColorRect LakesColorRect;
	private ColorRect CloudsColorRect;

	public override void _Ready()
	{
		LandColorRect = GetNode<ColorRect>("Land");
		LakesColorRect = GetNode<ColorRect>("Lakes");
		CloudsColorRect = GetNode<ColorRect>("Clouds");
		LandColorRect.Material = (Material)LandColorRect.Material.Duplicate(true);
		LakesColorRect.Material = (Material)LakesColorRect.Material.Duplicate(true);
		CloudsColorRect.Material = (Material)CloudsColorRect.Material.Duplicate(true);
		base._Ready();
	}

	public override void SetPixels(int _amount)
	{
		ShaderMaterial _mat = (ShaderMaterial)LandColorRect.Material;
		ShaderMaterial _mat2 = (ShaderMaterial)LakesColorRect.Material;
		ShaderMaterial _mat3 = (ShaderMaterial)CloudsColorRect.Material;

		_mat.SetShaderParameter("pixels", _amount);
		LandColorRect.Material = _mat;
		LandColorRect.Size = new Vector2(_amount, _amount);

		_mat2.SetShaderParameter("pixels", _amount);
		LakesColorRect.Material = _mat2;
		LakesColorRect.Size = new Vector2(_amount, _amount);

		_mat3.SetShaderParameter("pixels", _amount);
		CloudsColorRect.Material = _mat3;
		CloudsColorRect.Size = new Vector2(_amount, _amount);
	}

	public override void SetLight(Vector2 _pos)
	{
		ShaderMaterial _mat = (ShaderMaterial)LandColorRect.Material;
		_mat.SetShaderParameter("light_origin", _pos);
		LandColorRect.Material = _mat;

		ShaderMaterial _mat2 = (ShaderMaterial)LakesColorRect.Material;
		_mat2.SetShaderParameter("light_origin", _pos);
		LakesColorRect.Material = _mat2;

		ShaderMaterial _mat3 = (ShaderMaterial)CloudsColorRect.Material;
		_mat3.SetShaderParameter("light_origin", _pos);
		CloudsColorRect.Material = _mat3;
	}

	public override void SetSeed(uint _seed)
	{
		float converted_seed = _seed % 1000 / 100.0f;
		ShaderMaterial _mat = (ShaderMaterial)LandColorRect.Material;
		_mat.SetShaderParameter("seed", converted_seed);
		LandColorRect.Material = _mat;

		ShaderMaterial _mat2 = (ShaderMaterial)LakesColorRect.Material;
		_mat2.SetShaderParameter("seed", converted_seed);
		LakesColorRect.Material = _mat2;

		ShaderMaterial _mat3 = (ShaderMaterial)CloudsColorRect.Material;
		_mat3.SetShaderParameter("seed", converted_seed);
		CloudsColorRect.Material = _mat3;
	}

	public override void SetPlanetRotation(float _rotation)
	{
		ShaderMaterial _mat = (ShaderMaterial)LandColorRect.Material;
		_mat.SetShaderParameter("rotation", _rotation);
		LandColorRect.Material = _mat;

		ShaderMaterial _mat2 = (ShaderMaterial)LakesColorRect.Material;
		_mat2.SetShaderParameter("rotation", _rotation);
		LakesColorRect.Material = _mat2;

		ShaderMaterial _mat3 = (ShaderMaterial)CloudsColorRect.Material;
		_mat3.SetShaderParameter("rotation", _rotation);
		CloudsColorRect.Material = _mat3;
	}

	public override void UpdateTime(float _time)
	{
		ShaderMaterial _mat = (ShaderMaterial)LandColorRect.Material;
		_mat.SetShaderParameter("time", _time * GetMultiplier(_mat) * 0.02f);
		LandColorRect.Material = _mat;

		ShaderMaterial _mat2 = (ShaderMaterial)LakesColorRect.Material;
		_mat2.SetShaderParameter("time", _time * GetMultiplier(_mat2) * 0.02f);
		LakesColorRect.Material = _mat2;

		ShaderMaterial _mat3 = (ShaderMaterial)CloudsColorRect.Material;
		_mat3.SetShaderParameter("time", _time * GetMultiplier(_mat3) * 0.01f);
		CloudsColorRect.Material = _mat3;
	}

	public override void SetCustomTime(float _time)
	{
		ShaderMaterial _mat = (ShaderMaterial)LandColorRect.Material;
		_mat.SetShaderParameter("time", _time * GetMultiplier(_mat));
		LandColorRect.Material = _mat;
		
		ShaderMaterial _mat2 = (ShaderMaterial)LakesColorRect.Material;
		_mat2.SetShaderParameter("time", _time * GetMultiplier(_mat2));
		LakesColorRect.Material = _mat2;

		ShaderMaterial _mat3 = (ShaderMaterial)CloudsColorRect.Material;
		_mat3.SetShaderParameter("time", _time * GetMultiplier(_mat3));
		CloudsColorRect.Material = _mat3;
	}

	public override Color[] GetColors()
	{
		Color[] _colors = Array.Empty<Color>();
		foreach (Color color in GetColorsFromShader((ShaderMaterial)LandColorRect.Material)) _colors = _colors.Append(color).ToArray();
		foreach (Color color in GetColorsFromShader((ShaderMaterial)LakesColorRect.Material)) _colors = _colors.Append(color).ToArray();
		foreach (Color color in GetColorsFromShader((ShaderMaterial)CloudsColorRect.Material)) _colors = _colors.Append(color).ToArray();
		return _colors;
	}

	public override void SetColors(Color[] _colors)
	{
		Color[] cols1 = _colors[0..3];
		Color[] cols2 = _colors[3..6];
		Color[] cols3 = _colors[6..10];
		
		ShaderMaterial _mat = (ShaderMaterial)LandColorRect.Material;
		ShaderMaterial _mat2 = (ShaderMaterial)LakesColorRect.Material;
		ShaderMaterial _mat3 = (ShaderMaterial)CloudsColorRect.Material;
		
		ShaderMaterial new_mat = SetColorsOnShader(_mat, cols1);
		ShaderMaterial new_mat2 = SetColorsOnShader(_mat2, cols2);
		ShaderMaterial new_mat3 = SetColorsOnShader(_mat3, cols3);

		LandColorRect.Material = new_mat;
		LakesColorRect.Material = new_mat2;
		CloudsColorRect.Material = new_mat3;
	}
}
