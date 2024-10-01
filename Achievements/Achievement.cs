using Godot;

public partial class Achievement : HBoxContainer
{
	private CheckBox Check;

	private string description;

	[Export(PropertyHint.MultilineText)]
	public string Description
	{
		get => description;
		set { DescriptionLabel ??= GetNode<Label>("DescriptionLabel"); DescriptionLabel.Text = value; description = value; }
	}
	[Export]
	public bool Enabled
	{
		get => Check.ButtonPressed;
		set
		{
			Check ??= GetNode<CheckBox>("Check"); Check.ButtonPressed = value;
		}
	}

	private Label DescriptionLabel;

	public override void _Ready()
	{
		DescriptionLabel = GetNode<Label>("DescriptionLabel");
		Check = GetNode<CheckBox>("Check");

		DescriptionLabel.Text = description;
		Check.ButtonPressed = Enabled;
	}
}
