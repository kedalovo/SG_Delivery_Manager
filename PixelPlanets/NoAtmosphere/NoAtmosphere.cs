using Godot;
using System;
using System.Linq;

public partial class NoAtmosphere : Planet
{
    public override float Time {get; set;}
	public override bool OverrideTime {get; set;}
	public override Color[] OriginalColors {get; set;}

	[Export]
	public override float RelativeScale {get; set;}
	[Export]
	public override float GUIZoom {get; set;}

    private ColorRect GroundColorRect;
    private ColorRect CratersColorRect;

    public override void _Ready()
	{
		GroundColorRect = GetNode<ColorRect>("Ground");
        CratersColorRect = GetNode<ColorRect>("Craters");
		base._Ready();
	}

	public override void SetPixels(int _amount)
	{
		ShaderMaterial _mat = (ShaderMaterial)GroundColorRect.Material;
		ShaderMaterial _mat2 = (ShaderMaterial)CratersColorRect.Material;

		_mat.SetShaderParameter("pixels", _amount);
		GroundColorRect.Material = _mat;
		GroundColorRect.Size = new Vector2(_amount, _amount);

		_mat2.SetShaderParameter("pixels", _amount);
		CratersColorRect.Material = _mat2;
		CratersColorRect.Size = new Vector2(_amount, _amount);
	}

	public override void SetLight(Vector2 _pos)
	{
		ShaderMaterial _mat = (ShaderMaterial)GroundColorRect.Material;
		_mat.SetShaderParameter("light_origin", _pos);
		GroundColorRect.Material = _mat;

        ShaderMaterial _mat2 = (ShaderMaterial)CratersColorRect.Material;
		_mat2.SetShaderParameter("light_origin", _pos);
		CratersColorRect.Material = _mat2;
	}

	public override void SetSeed(uint _seed)
	{
		float converted_seed = _seed % 1000 / 100.0f;
		ShaderMaterial _mat = (ShaderMaterial)GroundColorRect.Material;
		_mat.SetShaderParameter("seed", converted_seed);
		GroundColorRect.Material = _mat;

        ShaderMaterial _mat2 = (ShaderMaterial)CratersColorRect.Material;
		_mat2.SetShaderParameter("seed", converted_seed);
        CratersColorRect.Material = _mat2;
	}

	public override void SetRotation(float _rotation)
	{
		ShaderMaterial _mat = (ShaderMaterial)GroundColorRect.Material;
		_mat.SetShaderParameter("rotation", _rotation);
		GroundColorRect.Material = _mat;

        ShaderMaterial _mat2 = (ShaderMaterial)CratersColorRect.Material;
		_mat2.SetShaderParameter("rotation", _rotation);
		CratersColorRect.Material = _mat2;
	}

	public override void UpdateTime(float _time)
	{
        ShaderMaterial _mat = (ShaderMaterial)GroundColorRect.Material;
		_mat.SetShaderParameter("time", _time * 0.02f);
		GroundColorRect.Material = _mat;

        ShaderMaterial _mat2 = (ShaderMaterial)CratersColorRect.Material;
		_mat2.SetShaderParameter("time", _time * 0.02f);
		CratersColorRect.Material = _mat2;
	}

	public override void SetCustomTime(float _time)
	{
        ShaderMaterial _mat = (ShaderMaterial)GroundColorRect.Material;
		_mat.SetShaderParameter("time", _time * GetMultiplier(_mat));
		GroundColorRect.Material = _mat;
        
		ShaderMaterial _mat2 = (ShaderMaterial)CratersColorRect.Material;
		_mat2.SetShaderParameter("time", _time * GetMultiplier(_mat2));
		CratersColorRect.Material = _mat2;
	}

	public override Color[] GetColors()
	{
        Color[] _colors = Array.Empty<Color>();
        foreach (Color color in GetColorsFromShader((ShaderMaterial)GroundColorRect.Material)) _colors = _colors.Append(color).ToArray();
        foreach (Color color in GetColorsFromShader((ShaderMaterial)CratersColorRect.Material)) _colors = _colors.Append(color).ToArray();
		return _colors;
	}

	public override void SetColors(Color[] _colors)
	{
		Color[] cols1 = _colors[0..3];
        Color[] cols2 = _colors[3..5];
        
        ShaderMaterial _mat = (ShaderMaterial)GroundColorRect.Material;
		ShaderMaterial _mat2 = (ShaderMaterial)CratersColorRect.Material;
        
        ShaderMaterial new_mat = SetColorsOnShader(_mat, cols1);
        ShaderMaterial new_mat2 = SetColorsOnShader(_mat2, cols2);

        GroundColorRect.Material = new_mat;
        CratersColorRect.Material = new_mat2;
	}
}
