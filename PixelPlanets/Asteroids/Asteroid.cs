using Godot;
using System;

public partial class Asteroid : Planet
{
	public override float Time {get; set;}
	public override bool OverrideTime {get; set;}
	public override Color[] OriginalColors {get; set;}

	private float RotationSpeed;
	private Vector2 Velocity;
	private Timer StartTimer;

	[Export]
	public override float RelativeScale {get; set;}
	[Export]
	public override float GUIZoom {get; set;}

	private ColorRect AsteroidColorRect;

	public override void _Ready()
	{
		AsteroidColorRect = GetNode<ColorRect>("Asteroid");
		AsteroidColorRect.Material = (Material)AsteroidColorRect.Material.Duplicate(true);
		StartTimer = GetNode<Timer>("Timer");
		base._Ready();
	}

	public override void _PhysicsProcess(double delta)
	{
		RotationDegrees += RotationSpeed;
		Position += Velocity;
		ShaderMaterial mat = (ShaderMaterial)AsteroidColorRect.Material;
		mat.SetShaderParameter("alpha", 1.0f - StartTimer.TimeLeft / StartTimer.WaitTime);
	}

	public void SetMovementData(float newRotationSpeed, Vector2 newVelocity)
	{
		RotationSpeed = newRotationSpeed;
		Velocity = newVelocity;
		GetTree().CreateTimer(20).Timeout += QueueFree;
		ShaderMaterial mat = (ShaderMaterial)AsteroidColorRect.Material;
		mat.SetShaderParameter("alpha", 0.0f);
		SetPhysicsProcess(true);
	}

	public override void SetPixels(int _amount)
	{
		ShaderMaterial _mat = (ShaderMaterial)AsteroidColorRect.Material;
		_mat.SetShaderParameter("pixels", _amount);
		AsteroidColorRect.Material = _mat;
		AsteroidColorRect.Size = new Vector2(_amount, _amount);
		PivotOffset = AsteroidColorRect.Size/2;
	}

	public override void SetLight(Vector2 _pos)
	{
		ShaderMaterial _mat = (ShaderMaterial)AsteroidColorRect.Material;
		_mat.SetShaderParameter("light_origin", _pos);
		AsteroidColorRect.Material = _mat;
	}

	public override void SetSeed(uint _seed)
	{
		float converted_seed = _seed % 1000 / 100.0f;
		ShaderMaterial _mat = (ShaderMaterial)AsteroidColorRect.Material;
		_mat.SetShaderParameter("seed", converted_seed);
		AsteroidColorRect.Material = _mat;
	}

	public override void SetPlanetRotation(float _rotation)
	{
		ShaderMaterial _mat = (ShaderMaterial)AsteroidColorRect.Material;
		_mat.SetShaderParameter("rotation", _rotation);
		AsteroidColorRect.Material = _mat;
	}

	public override void UpdateTime(float _time)
	{

	}

	public override void SetCustomTime(float _time)
	{
		ShaderMaterial _mat = (ShaderMaterial)AsteroidColorRect.Material;
		_mat.SetShaderParameter("rotation", _time * MathF.PI * 2.0f);
		AsteroidColorRect.Material = _mat;
	}

	public override Color[] GetColors()
	{
		return GetColorsFromShader((ShaderMaterial)AsteroidColorRect.Material);
	}

	public override void SetColors(Color[] _colors)
	{
		ShaderMaterial new_mat = SetColorsOnShader((ShaderMaterial)AsteroidColorRect.Material, _colors);
		AsteroidColorRect.Material = new_mat;
	}
}
