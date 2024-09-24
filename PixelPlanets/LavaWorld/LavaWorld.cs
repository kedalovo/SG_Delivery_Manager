using Godot;
using System;
using System.Linq;

public partial class LavaWorld : Planet
{
    public override float Time {get; set;}
	public override bool OverrideTime {get; set;}
	public override Color[] OriginalColors {get; set;}

	[Export]
	public override float RelativeScale {get; set;}
	[Export]
	public override float GUIZoom {get; set;}

    private ColorRect LandColorRect;
    private ColorRect CratersColorRect;
    private ColorRect LavaRiversColorRect;

    public override void _Ready()
	{
		LandColorRect = GetNode<ColorRect>("Land");
        CratersColorRect = GetNode<ColorRect>("Craters");
        LavaRiversColorRect = GetNode<ColorRect>("LavaRivers");
		LandColorRect.Material = (Material)LandColorRect.Material.Duplicate(true);
		CratersColorRect.Material = (Material)CratersColorRect.Material.Duplicate(true);
		LavaRiversColorRect.Material = (Material)LavaRiversColorRect.Material.Duplicate(true);
		base._Ready();
	}

	public override void SetPixels(int _amount)
	{
		ShaderMaterial _mat = (ShaderMaterial)LandColorRect.Material;
		ShaderMaterial _mat2 = (ShaderMaterial)CratersColorRect.Material;
		ShaderMaterial _mat3 = (ShaderMaterial)LavaRiversColorRect.Material;

		_mat.SetShaderParameter("pixels", _amount);
		LandColorRect.Material = _mat;
		LandColorRect.Size = new Vector2(_amount, _amount);

		_mat2.SetShaderParameter("pixels", _amount);
		CratersColorRect.Material = _mat2;
		CratersColorRect.Size = new Vector2(_amount, _amount);

        _mat3.SetShaderParameter("pixels", _amount);
		LavaRiversColorRect.Material = _mat3;
		LavaRiversColorRect.Size = new Vector2(_amount, _amount);
	}

	public override void SetLight(Vector2 _pos)
	{
		ShaderMaterial _mat = (ShaderMaterial)LandColorRect.Material;
		_mat.SetShaderParameter("light_origin", _pos);
		LandColorRect.Material = _mat;

        ShaderMaterial _mat2 = (ShaderMaterial)CratersColorRect.Material;
		_mat2.SetShaderParameter("light_origin", _pos);
		CratersColorRect.Material = _mat2;

        ShaderMaterial _mat3 = (ShaderMaterial)LavaRiversColorRect.Material;
		_mat3.SetShaderParameter("light_origin", _pos);
		LavaRiversColorRect.Material = _mat3;
	}

	public override void SetSeed(uint _seed)
	{
		float converted_seed = _seed % 1000 / 100.0f;
		ShaderMaterial _mat = (ShaderMaterial)LandColorRect.Material;
		_mat.SetShaderParameter("seed", converted_seed);
		LandColorRect.Material = _mat;

        ShaderMaterial _mat2 = (ShaderMaterial)CratersColorRect.Material;
		_mat2.SetShaderParameter("seed", converted_seed);
        CratersColorRect.Material = _mat2;

        ShaderMaterial _mat3 = (ShaderMaterial)LavaRiversColorRect.Material;
		_mat3.SetShaderParameter("seed", converted_seed);
        LavaRiversColorRect.Material = _mat3;
	}

	public override void SetRotation(float _rotation)
	{
		ShaderMaterial _mat = (ShaderMaterial)LandColorRect.Material;
		_mat.SetShaderParameter("rotation", _rotation);
		LandColorRect.Material = _mat;

        ShaderMaterial _mat2 = (ShaderMaterial)CratersColorRect.Material;
		_mat2.SetShaderParameter("rotation", _rotation);
		CratersColorRect.Material = _mat2;

        ShaderMaterial _mat3 = (ShaderMaterial)LavaRiversColorRect.Material;
		_mat3.SetShaderParameter("rotation", _rotation);
		LavaRiversColorRect.Material = _mat3;
	}

	public override void UpdateTime(float _time)
	{
        ShaderMaterial _mat = (ShaderMaterial)LandColorRect.Material;
		_mat.SetShaderParameter("time", _time * GetMultiplier(_mat) * 0.02f);
		LandColorRect.Material = _mat;

        ShaderMaterial _mat2 = (ShaderMaterial)CratersColorRect.Material;
		_mat2.SetShaderParameter("time", _time * GetMultiplier(_mat2) * 0.02f);
		CratersColorRect.Material = _mat2;

        ShaderMaterial _mat3 = (ShaderMaterial)LavaRiversColorRect.Material;
		_mat3.SetShaderParameter("time", _time * GetMultiplier(_mat3) * 0.02f);
		LavaRiversColorRect.Material = _mat3;
	}

	public override void SetCustomTime(float _time)
	{
        ShaderMaterial _mat = (ShaderMaterial)LandColorRect.Material;
		_mat.SetShaderParameter("time", _time * GetMultiplier(_mat));
		LandColorRect.Material = _mat;
        
		ShaderMaterial _mat2 = (ShaderMaterial)CratersColorRect.Material;
		_mat2.SetShaderParameter("time", _time * GetMultiplier(_mat2));
		CratersColorRect.Material = _mat2;

        ShaderMaterial _mat3 = (ShaderMaterial)LavaRiversColorRect.Material;
		_mat3.SetShaderParameter("time", _time * GetMultiplier(_mat3));
		LavaRiversColorRect.Material = _mat3;
	}

	public override Color[] GetColors()
	{
        Color[] _colors = Array.Empty<Color>();
        foreach (Color color in GetColorsFromShader((ShaderMaterial)LandColorRect.Material)) _colors = _colors.Append(color).ToArray();
        foreach (Color color in GetColorsFromShader((ShaderMaterial)CratersColorRect.Material)) _colors = _colors.Append(color).ToArray();
        foreach (Color color in GetColorsFromShader((ShaderMaterial)LavaRiversColorRect.Material)) _colors = _colors.Append(color).ToArray();
		return _colors;
	}

	public override void SetColors(Color[] _colors)
	{
		Color[] cols1 = _colors[0..3];
        Color[] cols2 = _colors[3..5];
        Color[] cols3 = _colors[5..8];
        
        ShaderMaterial _mat = (ShaderMaterial)LandColorRect.Material;
		ShaderMaterial _mat2 = (ShaderMaterial)CratersColorRect.Material;
		ShaderMaterial _mat3 = (ShaderMaterial)LavaRiversColorRect.Material;
        
        ShaderMaterial new_mat = SetColorsOnShader(_mat, cols1);
        ShaderMaterial new_mat2 = SetColorsOnShader(_mat2, cols2);
        ShaderMaterial new_mat3 = SetColorsOnShader(_mat3, cols3);

        LandColorRect.Material = new_mat;
        CratersColorRect.Material = new_mat2;
        LavaRiversColorRect.Material = new_mat3;
	}
}
