using Godot;
using System;
using System.Linq;

public partial class Star : Planet
{
    public override float Time {get; set;}
	public override bool OverrideTime {get; set;}
	public override Color[] OriginalColors {get; set;}

	[Export]
	public override float RelativeScale {get; set;}
	[Export]
	public override float GUIZoom {get; set;}

    private ColorRect BlobsColorRect;
    private ColorRect StarColorRect;
    private ColorRect StarFlaresColorRect;

    private Gradient StarColor1;
    private Gradient StarColor2;
    private Gradient StarFlareColor1;
    private Gradient StarFlareColor2;

    public override void _Ready()
	{
		BlobsColorRect = GetNode<ColorRect>("Blobs");
        StarColorRect = GetNode<ColorRect>("Star");
        StarFlaresColorRect = GetNode<ColorRect>("StarFlares");
		base._Ready();

        StarColor1 = new Gradient() {
            Offsets = new float[] {0.0f, 0.33f, 0.66f, 1.0f},
            Colors = new Color[] {new("f5ffe8"), new("ffd832"), new("ff823b"), new("7c191a")}
        };
        StarColor2 = new Gradient() {
            Offsets = new float[] {0.0f, 0.33f, 0.66f, 1.0f},
            Colors = new Color[] {new("f5ffe8"), new("77d6c1"), new("1c92a7"), new("033e5e")}
        };
        
        StarFlareColor1 = new Gradient() {
            Offsets = new float[] {0.0f, 1.0f},
            Colors = new Color[] {new("ffd832"), new("f5ffe8")}
        };
        StarFlareColor2 = new Gradient() {
            Offsets = new float[] {0.0f, 1.0f},
            Colors = new Color[] {new("77d6c1"), new("f5ffe8")}
        };
	}

	public override void SetPixels(int _amount)
	{
		ShaderMaterial _mat = (ShaderMaterial)BlobsColorRect.Material;
		ShaderMaterial _mat2 = (ShaderMaterial)StarColorRect.Material;
		ShaderMaterial _mat3 = (ShaderMaterial)StarFlaresColorRect.Material;

		_mat.SetShaderParameter("pixels", _amount * RelativeScale);
		BlobsColorRect.Material = _mat;
		BlobsColorRect.Size = new Vector2(_amount, _amount) * RelativeScale;

		_mat2.SetShaderParameter("pixels", _amount);
		StarColorRect.Material = _mat2;
		StarColorRect.Size = new Vector2(_amount, _amount);

        _mat3.SetShaderParameter("pixels", _amount * RelativeScale);
		StarFlaresColorRect.Material = _mat3;
		StarFlaresColorRect.Size = new Vector2(_amount, _amount) * RelativeScale;
	}

	public override void SetLight(Vector2 _pos)
	{

	}

	public override void SetSeed(uint _seed)
	{
		float converted_seed = _seed % 1000 / 100.0f;
		ShaderMaterial _mat = (ShaderMaterial)BlobsColorRect.Material;
		_mat.SetShaderParameter("seed", converted_seed);
		BlobsColorRect.Material = _mat;

        ShaderMaterial _mat2 = (ShaderMaterial)StarColorRect.Material;
		_mat2.SetShaderParameter("seed", converted_seed);
        StarColorRect.Material = _mat2;

        ShaderMaterial _mat3 = (ShaderMaterial)StarFlaresColorRect.Material;
		_mat3.SetShaderParameter("seed", converted_seed);
        StarFlaresColorRect.Material = _mat3;
	}

	public override void SetRotation(float _rotation)
	{
		ShaderMaterial _mat = (ShaderMaterial)BlobsColorRect.Material;
		_mat.SetShaderParameter("rotation", _rotation);
		BlobsColorRect.Material = _mat;

        ShaderMaterial _mat2 = (ShaderMaterial)StarColorRect.Material;
		_mat2.SetShaderParameter("rotation", _rotation);
		StarColorRect.Material = _mat2;

        ShaderMaterial _mat3 = (ShaderMaterial)StarFlaresColorRect.Material;
		_mat3.SetShaderParameter("rotation", _rotation);
		StarFlaresColorRect.Material = _mat3;
	}

	public override void UpdateTime(float _time)
	{
        ShaderMaterial _mat = (ShaderMaterial)BlobsColorRect.Material;
		_mat.SetShaderParameter("time", _time * GetMultiplier(_mat) * 0.01f);
		BlobsColorRect.Material = _mat;

        ShaderMaterial _mat2 = (ShaderMaterial)StarColorRect.Material;
		_mat2.SetShaderParameter("time", _time * GetMultiplier(_mat2) * 0.005f);
		StarColorRect.Material = _mat2;

        ShaderMaterial _mat3 = (ShaderMaterial)StarFlaresColorRect.Material;
		_mat3.SetShaderParameter("time", _time * GetMultiplier(_mat3) * 0.015f);
		StarFlaresColorRect.Material = _mat3;
	}

	public override void SetCustomTime(float _time)
	{
        ShaderMaterial _mat = (ShaderMaterial)BlobsColorRect.Material;
		_mat.SetShaderParameter("time", _time * GetMultiplier(_mat));
		BlobsColorRect.Material = _mat;
        
		ShaderMaterial _mat2 = (ShaderMaterial)StarColorRect.Material;
		_mat2.SetShaderParameter("time", _time * (1.0f / (float)_mat2.GetShaderParameter("time_speed")));
		StarColorRect.Material = _mat2;

        ShaderMaterial _mat3 = (ShaderMaterial)StarFlaresColorRect.Material;
		_mat3.SetShaderParameter("time", _time * GetMultiplier(_mat3));
		StarFlaresColorRect.Material = _mat3;
	}

	public override Color[] GetColors()
	{
        Color[] _colors = Array.Empty<Color>();
        foreach (Color color in GetColorsFromShader((ShaderMaterial)BlobsColorRect.Material)) _colors = _colors.Append(color).ToArray();
        foreach (Color color in GetColorsFromShader((ShaderMaterial)StarColorRect.Material)) _colors = _colors.Append(color).ToArray();
        foreach (Color color in GetColorsFromShader((ShaderMaterial)StarFlaresColorRect.Material)) _colors = _colors.Append(color).ToArray();
		return _colors;
	}

	public override void SetColors(Color[] _colors)
	{
		Color[] cols1 = _colors[0..1];
        Color[] cols2 = _colors[1..5];
        Color[] cols3 = _colors[5..7];
        
        ShaderMaterial _mat = (ShaderMaterial)BlobsColorRect.Material;
		ShaderMaterial _mat2 = (ShaderMaterial)StarColorRect.Material;
		ShaderMaterial _mat3 = (ShaderMaterial)StarFlaresColorRect.Material;
        
        ShaderMaterial new_mat = SetColorsOnShader(_mat, cols1);
        ShaderMaterial new_mat2 = SetColorsOnShader(_mat2, cols2);
        ShaderMaterial new_mat3 = SetColorsOnShader(_mat3, cols3);

        BlobsColorRect.Material = new_mat;
        StarColorRect.Material = new_mat2;
        StarFlaresColorRect.Material = new_mat3;
	}
}
