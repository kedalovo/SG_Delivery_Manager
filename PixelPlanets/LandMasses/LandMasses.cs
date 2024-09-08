using Godot;
using System;
using System.Linq;

public partial class LandMasses : Planet
{
    public override float Time {get; set;}
	public override bool OverrideTime {get; set;}
	public override Color[] OriginalColors {get; set;}

	[Export]
	public override float RelativeScale {get; set;}
	[Export]
	public override float GUIZoom {get; set;}

    private ColorRect WaterColorRect;
    private ColorRect LandColorRect;
    private ColorRect CloudColorRect;

    public override void _Ready()
	{
		WaterColorRect = GetNode<ColorRect>("Water");
        LandColorRect = GetNode<ColorRect>("Land");
        CloudColorRect = GetNode<ColorRect>("Cloud");
		base._Ready();
	}

	public override void SetPixels(int _amount)
	{
		ShaderMaterial _mat = (ShaderMaterial)WaterColorRect.Material;
		ShaderMaterial _mat2 = (ShaderMaterial)LandColorRect.Material;
		ShaderMaterial _mat3 = (ShaderMaterial)CloudColorRect.Material;

		_mat.SetShaderParameter("pixels", _amount);
		WaterColorRect.Material = _mat;
		WaterColorRect.Size = new Vector2(_amount, _amount);

		_mat2.SetShaderParameter("pixels", _amount);
		LandColorRect.Material = _mat2;
		LandColorRect.Size = new Vector2(_amount, _amount);

        _mat3.SetShaderParameter("pixels", _amount);
		CloudColorRect.Material = _mat3;
		CloudColorRect.Size = new Vector2(_amount, _amount);
	}

	public override void SetLight(Vector2 _pos)
	{
		ShaderMaterial _mat = (ShaderMaterial)WaterColorRect.Material;
		_mat.SetShaderParameter("light_origin", _pos);
		WaterColorRect.Material = _mat;

        ShaderMaterial _mat2 = (ShaderMaterial)LandColorRect.Material;
		_mat2.SetShaderParameter("light_origin", _pos);
		LandColorRect.Material = _mat2;

        ShaderMaterial _mat3 = (ShaderMaterial)CloudColorRect.Material;
		_mat3.SetShaderParameter("light_origin", _pos);
		CloudColorRect.Material = _mat3;
	}

	public override void SetSeed(uint _seed)
	{
		float converted_seed = _seed % 1000 / 100.0f;
		ShaderMaterial _mat = (ShaderMaterial)WaterColorRect.Material;
		_mat.SetShaderParameter("seed", converted_seed);
		WaterColorRect.Material = _mat;

        ShaderMaterial _mat2 = (ShaderMaterial)LandColorRect.Material;
		_mat2.SetShaderParameter("seed", converted_seed);
        LandColorRect.Material = _mat2;

        ShaderMaterial _mat3 = (ShaderMaterial)CloudColorRect.Material;
		_mat3.SetShaderParameter("seed", converted_seed);
        _mat3.SetShaderParameter("cloud_cover", new Random().NextSingle() / 3.0f + 0.35f);
        CloudColorRect.Material = _mat3;
	}

	public override void SetRotation(float _rotation)
	{
		ShaderMaterial _mat = (ShaderMaterial)WaterColorRect.Material;
		_mat.SetShaderParameter("rotation", _rotation);
		WaterColorRect.Material = _mat;

        ShaderMaterial _mat2 = (ShaderMaterial)LandColorRect.Material;
		_mat2.SetShaderParameter("rotation", _rotation);
		LandColorRect.Material = _mat2;

        ShaderMaterial _mat3 = (ShaderMaterial)CloudColorRect.Material;
		_mat3.SetShaderParameter("rotation", _rotation);
		CloudColorRect.Material = _mat3;
	}

	public override void UpdateTime(float _time)
	{
        ShaderMaterial _mat = (ShaderMaterial)WaterColorRect.Material;
		_mat.SetShaderParameter("time", _time * 0.02f);
		WaterColorRect.Material = _mat;

        ShaderMaterial _mat2 = (ShaderMaterial)LandColorRect.Material;
		_mat2.SetShaderParameter("time", _time * 0.02f);
		LandColorRect.Material = _mat2;

        ShaderMaterial _mat3 = (ShaderMaterial)CloudColorRect.Material;
		_mat3.SetShaderParameter("time", _time * 0.01f);
		CloudColorRect.Material = _mat3;
	}

	public override void SetCustomTime(float _time)
	{
        ShaderMaterial _mat = (ShaderMaterial)WaterColorRect.Material;
		_mat.SetShaderParameter("time", _time * GetMultiplier(_mat));
		WaterColorRect.Material = _mat;
        
		ShaderMaterial _mat2 = (ShaderMaterial)LandColorRect.Material;
		_mat2.SetShaderParameter("time", _time * GetMultiplier(_mat2));
		LandColorRect.Material = _mat2;

        ShaderMaterial _mat3 = (ShaderMaterial)CloudColorRect.Material;
		_mat3.SetShaderParameter("time", _time * GetMultiplier(_mat3));
		CloudColorRect.Material = _mat3;
	}

	public override Color[] GetColors()
	{
        Color[] _colors = Array.Empty<Color>();
        foreach (Color color in GetColorsFromShader((ShaderMaterial)WaterColorRect.Material)) _colors = _colors.Append(color).ToArray();
        foreach (Color color in GetColorsFromShader((ShaderMaterial)LandColorRect.Material)) _colors = _colors.Append(color).ToArray();
        foreach (Color color in GetColorsFromShader((ShaderMaterial)CloudColorRect.Material)) _colors = _colors.Append(color).ToArray();
		return _colors;
	}

	public override void SetColors(Color[] _colors)
	{
		Color[] cols1 = _colors[0..3];
        Color[] cols2 = _colors[3..7];
        Color[] cols3 = _colors[7..11];
        
        ShaderMaterial _mat = (ShaderMaterial)WaterColorRect.Material;
		ShaderMaterial _mat2 = (ShaderMaterial)LandColorRect.Material;
		ShaderMaterial _mat3 = (ShaderMaterial)CloudColorRect.Material;
        
        ShaderMaterial new_mat = SetColorsOnShader(_mat, cols1);
        ShaderMaterial new_mat2 = SetColorsOnShader(_mat2, cols2);
        ShaderMaterial new_mat3 = SetColorsOnShader(_mat3, cols3);

        WaterColorRect.Material = new_mat;
        LandColorRect.Material = new_mat2;
        CloudColorRect.Material = new_mat3;
	}
}
