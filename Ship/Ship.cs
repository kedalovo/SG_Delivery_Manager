using Godot;

public partial class Ship : Node2D
{
	[Signal]
	public delegate void OnShipArrivedEventHandler();

	private Node2D Pivot;
	private Sprite2D ShipSprite;
	private ColorRect Target;

	private States CurrentState = States.PARKED;
	private enum States {
		FLYING,
		PARKED,
		ROTATING,
	}
	
	private Label Label1;
	private Label Label2;
	private Label Label3;

	private AnimationPlayer Animator;

	private Vector2 MoveTarget;
	private float RotationTarget;
	private float MoveDuration;
	private bool IsOverTarget;
	private bool HasLooped = false;
	private float LastRotation;

	public override void _Ready()
	{
		Pivot = GetNode<Node2D>("Pivot");
		ShipSprite = GetNode<Sprite2D>("Pivot/ShipSprite");
		Target = GetNode<ColorRect>("RectPivot/ColorRect");
		Label1 = GetNode<Label>("VBoxContainer/Label");
		Label2 = GetNode<Label>("VBoxContainer/Label2");
		Label3 = GetNode<Label>("VBoxContainer/Label3");
		Animator = GetNode<AnimationPlayer>("Animator");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (CurrentState == States.ROTATING && ShipSprite.GlobalPosition.DistanceTo(Target.GlobalPosition) < 15.0f)
		{
			Move(MoveTarget, MoveDuration);
		}
	}

	public void SetMoveTarget(Vector2 newPosition, float duration)
	{
		MoveTarget = newPosition;
		MoveDuration = duration;
		CurrentState = States.ROTATING;
		Animator.SpeedScale = 4.0f;
		RotationTarget = Mathf.RadToDeg((Pivot.GlobalPosition - MoveTarget).Angle()) + 90.0f;
		if (RotationTarget < 0) RotationTarget += 360.0f;
		else if (RotationTarget > 360.0f) RotationTarget -= 360.0f;
		IsOverTarget = RotationTarget < Mathf.RadToDeg((Pivot.GlobalPosition - ShipSprite.GlobalPosition).Angle()) + 180.0f;
		GetNode<Node2D>("RectPivot").RotationDegrees = RotationTarget - 180.0f;
	}

	private void Move(Vector2 newPosition, float duration)
	{
		CurrentState = States.FLYING;
		Animator.SpeedScale = 1.0f;
		Animator.Pause();
		Tween NewTween = GetTree().CreateTween();
		NewTween.TweenProperty(this, "position", newPosition, duration).SetTrans(Tween.TransitionType.Circ);
		NewTween.Finished += Arrived;
	}

	private void Arrived()
	{
		Animator.Play();
		CurrentState = States.PARKED;
		EmitSignal(SignalName.OnShipArrived);
	}

	public float GetShipRotation()
	{
		return Mathf.RadToDeg((Pivot.GlobalPosition - ShipSprite.GlobalPosition).Angle()) + 180.0f;
	}

	public Vector2 GetShipPosition()
	{
		return ShipSprite.GlobalPosition;
	}

	public bool IsAvailable()
	{
		return CurrentState == States.PARKED;
	}
}
