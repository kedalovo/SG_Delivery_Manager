using Godot;
using System;

public partial class TestScene : Node2D
{
	Node2D Loc1;
	Node2D Loc2;
	Node2D Loc3;

	public override void _Ready()
	{
		Loc1 = GetNode<Node2D>("NewLocations/Location6");
		Loc2 = GetNode<Node2D>("NewLocations/Location7");
		Loc3 = GetNode<Node2D>("NewLocations/Location10");
	}

	public override void _Draw()
	{
		DrawConnection(Loc1, Loc3, 3);
		DrawConnection(Loc1, Loc2);
	}

	private void DrawConnection(Node2D From, Node2D To, int Distance = 1)
	{
		Vector2 Diff = (From.GlobalPosition - To.GlobalPosition).Normalized() * 70;
		DrawLine(From.GlobalPosition - Diff, To.GlobalPosition + Diff, Colors.White, 1.0f);
		for (int i = 0; i < Distance; i++)
		{
			DrawTriangle(From, To, i);
			DrawTriangle(To, From, i);
		}
	}

	private void DrawTriangle(Node2D From, Node2D To, int Offset = 0)
	{
		Vector2 Direction = (From.GlobalPosition - To.GlobalPosition).Normalized();
		Vector2 Base = Direction * 70;
		Vector2 p1 = From.GlobalPosition - Base * 1.25f - Offset * Direction * 17.5f;
		Vector2 p2 = From.GlobalPosition - Base - Offset * Direction * 17.5f + new Vector2(Base.Y, -Base.X) * 0.075f;
		Vector2 p3 = From.GlobalPosition - Base - Offset * Direction * 17.5f + new Vector2(-Base.Y, Base.X) * 0.075f;
		DrawPolygon(new Vector2[] {p1, p2, p3}, new Color[] {Colors.White, Colors.White, Colors.White});
	}

	public override void _PhysicsProcess(double delta)
	{
		QueueRedraw();
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("esc"))
		{
			GetTree().Quit();
		}
	}
}