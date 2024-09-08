using Godot;
using System;
using System.Linq;

public partial class BlackHole : Planet
{
    public override float Time {get; set;}
	public override bool OverrideTime {get; set;}
	public override Color[] OriginalColors {get; set;}

	[Export]
	public override float RelativeScale {get; set;}
	[Export]
	public override float GUIZoom {get; set;}

    private ColorRect BlackHoleColorRect;
    private ColorRect DiskColorRect;

    public override void _Ready()
	{
		BlackHoleColorRect = GetNode<ColorRect>("BlackHole");
        DiskColorRect = GetNode<ColorRect>("Disk");
		base._Ready();
	}

	public override void SetPixels(int _amount)
	{
		ShaderMaterial _mat = (ShaderMaterial)BlackHoleColorRect.Material;
		ShaderMaterial _mat2 = (ShaderMaterial)DiskColorRect.Material;

		_mat.SetShaderParameter("pixels", _amount);
		BlackHoleColorRect.Material = _mat;
		BlackHoleColorRect.Size = new Vector2(_amount, _amount);

		_mat2.SetShaderParameter("pixels", _amount * 3.0f);
		DiskColorRect.Material = _mat2;
        DiskColorRect.Position = new Vector2(-_amount, -_amount);
		DiskColorRect.Size = new Vector2(_amount, _amount) * 3.0f;
	}

	public override void SetLight(Vector2 _pos)
	{
		
	}

	public override void SetSeed(uint _seed)
	{
		float converted_seed = _seed % 1000 / 100.0f;
		ShaderMaterial _mat = (ShaderMaterial)DiskColorRect.Material;
		_mat.SetShaderParameter("seed", converted_seed);
		DiskColorRect.Material = _mat;
	}

	public override void SetRotation(float _rotation)
	{
		ShaderMaterial _mat = (ShaderMaterial)DiskColorRect.Material;
		_mat.SetShaderParameter("rotation", _rotation + 0.7f);
		DiskColorRect.Material = _mat;
	}

	public override void UpdateTime(float _time)
	{
        ShaderMaterial _mat = (ShaderMaterial)DiskColorRect.Material;
		_mat.SetShaderParameter("time", _time * 314.15f * 0.004f);
		DiskColorRect.Material = _mat;
	}

	public override void SetCustomTime(float _time)
	{
		ShaderMaterial _mat = (ShaderMaterial)DiskColorRect.Material;
		_mat.SetShaderParameter("time", _time * 314.15f * (float)_mat.GetShaderParameter("time_speed") * 0.5f);
		DiskColorRect.Material = _mat;
	}

	public override Color[] GetColors()
	{
        Color[] _colors = Array.Empty<Color>();
        foreach (Color color in GetColorsFromShader((ShaderMaterial)BlackHoleColorRect.Material)) _colors = _colors.Append(color).ToArray();
        foreach (Color color in GetColorsFromShader((ShaderMaterial)DiskColorRect.Material)) _colors = _colors.Append(color).ToArray();
		return _colors;
	}

	public override void SetColors(Color[] _colors)
	{
		Color[] cols1 = _colors[0..3];
        Color[] cols2 = _colors[3..8];
        
        ShaderMaterial _mat = (ShaderMaterial)BlackHoleColorRect.Material;
		ShaderMaterial _mat2 = (ShaderMaterial)DiskColorRect.Material;
        
        ShaderMaterial new_mat = SetColorsOnShader(_mat, cols1);
        ShaderMaterial new_mat2 = SetColorsOnShader(_mat2, cols2);

        BlackHoleColorRect.Material = new_mat;
        DiskColorRect.Material = new_mat2;
	}
}
