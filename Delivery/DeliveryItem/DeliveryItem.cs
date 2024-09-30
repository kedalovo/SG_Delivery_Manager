using System.Collections.Generic;
using Godot;

public partial class DeliveryItem : VBoxContainer
{
	[Signal]
	public delegate void OnFailEventHandler(string Reason);
	[Signal]
	public delegate void OnMiddleManMetEventHandler(int middleManId);

	private string Tag = "";
	private Dictionary<string, string> TagData;

	private TextureRect Texture;
	private Label DataLabel;
	private Control PlanetControl;
	private ProgressBar FailStateProgressBar;
	private Timer DeliveryTimer;

	public override void _Ready()
	{
		Texture = GetNode<TextureRect>("DataHBox/Texture");
		DataLabel = GetNode<Label>("DataHBox/DataLabel");
		PlanetControl = GetNode<Control>("DataHBox/PlanetSpace");
		FailStateProgressBar = GetNode<ProgressBar>("ProgressBar");
		DeliveryTimer = GetNode<Timer>("Timer");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Tag == "Timed")
		{
			DataLabel.Text = Mathf.RoundToInt(DeliveryTimer.TimeLeft).ToString();
			// GD.PrintS(FailStateProgressBar.Value, DeliveryTimer.TimeLeft);
			FailStateProgressBar.Value = DeliveryTimer.TimeLeft;
			FailStateProgressBar.Modulate = Colors.Green.Lerp(Colors.Red, 1.0f - (float)(DeliveryTimer.TimeLeft / DeliveryTimer.WaitTime));
		}
	}

	public void SetData(string newTag, Dictionary<string, string> newTagData)
	{
		if (Tag != "")
		{
			GD.PrintErr("Tag ", newTag, " already exists!");
		}
		
		GD.Print("\tTrying to load texture...");
		Texture.Texture = GD.Load<Texture2D>(newTagData["texture_path"]);
		GD.Print("\tTexture loaded...");

		Tag = newTag;
		TagData = newTagData;
		
		switch (Tag)
		{
			case "Timed":
				TagData["time"] = (20 - int.Parse(TagData["tier"])*5).ToString();
				DeliveryTimer.WaitTime = 20 - int.Parse(TagData["tier"])*5;
				FailStateProgressBar.MaxValue = DeliveryTimer.WaitTime;
				DeliveryTimer.Start();
				break;
			case "Fragile":
				TagData["jumps_left"] = TagData["jumps"];
				DataLabel.Text = TagData["jumps"];
				FailStateProgressBar.MaxValue = int.Parse(TagData["jumps"]);
				break;
			case "Segmented":
				FailStateProgressBar.Hide();
				break;
		}
	}

	public void SetPlanet(Planet newPlanet)
	{
		newPlanet.Reparent(PlanetControl);
		newPlanet.Position = new Vector2(-34, 17);
	}

	public void SetEmpty()
	{
		Texture.Hide();
		DataLabel.AutowrapMode = TextServer.AutowrapMode.Word;
		DataLabel.SizeFlagsHorizontal = SizeFlags.ExpandFill;
		DataLabel.Text = "No data";
		PlanetControl.Hide();
		FailStateProgressBar.Hide();
	}

	public void Jump(int jumpedToId)
	{
		switch (Tag)
		{
			case "Timed":
				break;
			case "Fragile":
				TagData["jumps_left"] = (int.Parse(TagData["jumps_left"]) - 1).ToString();
				DataLabel.Text = TagData["jumps_left"];
				if (TagData["jumps_left"] == "0")
				{
					EmitSignal(SignalName.OnFail, "No more jumps left");
				}
				FailStateProgressBar.Value = int.Parse(TagData["jumps_left"]);
				FailStateProgressBar.Modulate = Colors.Green.Lerp(Colors.Red, 1.0f - int.Parse(TagData["jumps_left"]) / (float)int.Parse(TagData["jumps"]));
				break;
			case "Segmented":
				if (jumpedToId.ToString() == TagData["middle-man-id"])
				{
					TagData["middle-man-met"] = "true";
					EmitSignal(SignalName.OnMiddleManMet, jumpedToId);
					Hide();
				}
				break;
		}
	}

	public Dictionary<string, string> GetTagData()
	{
		return TagData;
	}

	public void OnDeliveryTimerTimeout()
	{
		EmitSignal(SignalName.OnFail, "Timer ran out");
	}
}
