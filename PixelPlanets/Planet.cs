using System;
using Godot;

[GlobalClass]
public partial class Planet : Control
{
	public virtual float Time {get; set;}
	public virtual bool OverrideTime {get; set;}
	public virtual Color[] OriginalColors {get; set;}

	[Export]
	public virtual float RelativeScale {get; set;}
	[Export]
	public virtual float GUIZoom {get; set;}

	public override void _Ready()
	{
		Time = 1000.0f;
		OverrideTime = false;
		RelativeScale = 1.0f;
		GUIZoom = 1.0f;
	}

	public virtual void SetPixels(int _amount) {}
	public virtual void SetLight(Vector2 _pos) {}
	public virtual void SetSeed(uint _seed) {}
	public virtual void SetRotation(float _rotation) {}
	public virtual void UpdateTime(float _time) {}
	public virtual void SetCustomTime(float _time) {}

	public float GetMultiplier(ShaderMaterial _mat)
	{
		return Mathf.Round((float)_mat.GetShaderParameter("size")) * 2.0f / (float)_mat.GetShaderParameter("time_speed");
	}

	public override void _Process(double delta)
	{
		Time += (float)delta;
		if (!OverrideTime)
		{
			UpdateTime(Time);
		}
	}

	public virtual Color[] GetColors()
	{
		return Array.Empty<Color>();
	}

	public Color[] GetColorsFromShader(ShaderMaterial _mat, string _uniform_name = "colors")
	{
		return (Color[])_mat.GetShaderParameter(_uniform_name);
	}

	public virtual void SetColors(Color[] _colors) {}

	public ShaderMaterial SetColorsOnShader(ShaderMaterial _mat, Color[] _colors, string _uniform_name = "colors")
	{
		_mat.SetShaderParameter(_uniform_name, _colors);
		return _mat;
	}
}
