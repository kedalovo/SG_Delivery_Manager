using Godot;
using System;
using System.Linq;

public partial class GasPlanetLayers : Planet
{
    public override float Time {get; set;}
	public override bool OverrideTime {get; set;}
	public override Color[] OriginalColors {get; set;}

	[Export]
	public override float RelativeScale {get; set;}
	[Export]
	public override float GUIZoom {get; set;}

    private ColorRect GasLayersColorRect;
    private ColorRect RingColorRect;

    public override void _Ready()
	{
		GasLayersColorRect = GetNode<ColorRect>("GasLayers");
        RingColorRect = GetNode<ColorRect>("Ring");
		base._Ready();
	}

	public override void SetPixels(int _amount)
	{
		ShaderMaterial _mat = (ShaderMaterial)GasLayersColorRect.Material;
		ShaderMaterial _mat2 = (ShaderMaterial)RingColorRect.Material;

		_mat.SetShaderParameter("pixels", _amount);
		GasLayersColorRect.Material = _mat;
		GasLayersColorRect.Size = new Vector2(_amount, _amount);

		_mat2.SetShaderParameter("pixels", _amount * 3.0f);
		RingColorRect.Material = _mat2;
        RingColorRect.Position = new Vector2(-_amount, -_amount);
		RingColorRect.Size = new Vector2(_amount, _amount) * 3.0f;
	}

	public override void SetLight(Vector2 _pos)
	{
		ShaderMaterial _mat = (ShaderMaterial)GasLayersColorRect.Material;
		ShaderMaterial _mat2 = (ShaderMaterial)RingColorRect.Material;

		_mat.SetShaderParameter("light_origin", _pos);
		GasLayersColorRect.Material = _mat;

		_mat2.SetShaderParameter("light_origin", _pos);
		RingColorRect.Material = _mat2;
	}

	public override void SetSeed(uint _seed)
	{
		float converted_seed = _seed % 1000 / 100.0f;
		ShaderMaterial _mat = (ShaderMaterial)GasLayersColorRect.Material;
		_mat.SetShaderParameter("seed", converted_seed);
		GasLayersColorRect.Material = _mat;

        ShaderMaterial _mat2 = (ShaderMaterial)RingColorRect.Material;
		_mat2.SetShaderParameter("seed", converted_seed);
		RingColorRect.Material = _mat2;
	}

	public override void SetRotation(float _rotation)
	{
        ShaderMaterial _mat = (ShaderMaterial)GasLayersColorRect.Material;
		_mat.SetShaderParameter("rotation", _rotation);
		RingColorRect.Material = _mat;

		ShaderMaterial _mat2 = (ShaderMaterial)RingColorRect.Material;
		_mat2.SetShaderParameter("rotation", _rotation + 0.7f);
		RingColorRect.Material = _mat2;
	}

	public override void UpdateTime(float _time)
	{
        ShaderMaterial _mat = (ShaderMaterial)GasLayersColorRect.Material;
		_mat.SetShaderParameter("time", _time * GetMultiplier(_mat) * 0.004f);
		GasLayersColorRect.Material = _mat;

        ShaderMaterial _mat2 = (ShaderMaterial)RingColorRect.Material;
		_mat2.SetShaderParameter("time", _time * 314.15f * 0.004f);
		RingColorRect.Material = _mat2;
	}

	public override void SetCustomTime(float _time)
	{
		ShaderMaterial _mat = (ShaderMaterial)GasLayersColorRect.Material;
		_mat.SetShaderParameter("time", _time * GetMultiplier(_mat));
		GasLayersColorRect.Material = _mat;

        ShaderMaterial _mat2 = (ShaderMaterial)RingColorRect.Material;
		_mat2.SetShaderParameter("time", _time * 314.15f * (float)_mat.GetShaderParameter("time_speed") * 0.5f);
		RingColorRect.Material = _mat2;
	}

	public override Color[] GetColors()
	{
        Color[] _colors = Array.Empty<Color>();
        foreach (Color color in GetColorsFromShader((ShaderMaterial)GasLayersColorRect.Material)) _colors = _colors.Append(color).ToArray();
        foreach (Color color in GetColorsFromShader((ShaderMaterial)RingColorRect.Material)) _colors = _colors.Append(color).ToArray();
		return _colors;
	}

	public override void SetColors(Color[] _colors)
	{
		Color[] cols1 = _colors[0..3];
        Color[] cols2 = _colors[3..6];
        
        ShaderMaterial _mat = (ShaderMaterial)GasLayersColorRect.Material;
		ShaderMaterial _mat2 = (ShaderMaterial)RingColorRect.Material;
        
        ShaderMaterial new_mat = SetColorsOnShader(_mat, cols1);
        ShaderMaterial new_mat2 = SetColorsOnShader(_mat2, cols2);

        GasLayersColorRect.Material = new_mat;
        RingColorRect.Material = new_mat2;
	}
}
