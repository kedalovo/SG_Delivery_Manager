using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Location : Node2D
{
	[Signal]
	public delegate void LocationPressedEventHandler(int id);
	[Signal]
	public delegate void LocationAvailableEventHandler(int id);
	[Export]
	public string LocationName;
	[Export]
	public int ID;
	private Label NameLabel;
	private Sprite2D StarSprite;
	// private Polygon2D QuestPolygon;
	private AnimationPlayer HighlightAnimator;
	private AnimationPlayer DeliveryHighlightAnimator;
	private AnimationPlayer Animator;

	private TextureRect DeliveryHighlighter;

	private int[] Quests;

	public string[] Hazards;
	public Dictionary<string, string>[] HazardsData;
	private TextureRect[] HazardsIcons;

	private int FuelCost;

	private int FuelLevel;
	private int MaxFuel;

	private int QuestCompleted;

	private HBoxContainer HazardsHBox;

	public Control Visuals;

	public string PlanetType = "";
	public string PlanetPreset = "";

	public Color LocationColor;

	public override void _Ready()
	{
		Quests = Array.Empty<int>();
		Hazards = Array.Empty<string>();
		HazardsData = Array.Empty<Dictionary<string, string>>();
		HazardsIcons = Array.Empty<TextureRect>();

		FuelCost = 50;
		FuelLevel = 5;
		MaxFuel = 5;
		QuestCompleted = 0;

		NameLabel = GetNode<Label>("Visuals/VBox/Label");
		// StarSprite = GetNode<Sprite2D>("StarSprite");
		// QuestPolygon = GetNode<Polygon2D>("QuestPolygon");
		HighlightAnimator = GetNode<AnimationPlayer>("HighlightAnimator");
		DeliveryHighlightAnimator = GetNode<AnimationPlayer>("DeliveryHighlightAnimator");
		Animator = GetNode<AnimationPlayer>("Animator");
		HazardsHBox = GetNode<HBoxContainer>("Visuals/VBox/HBox");
		Visuals = GetNode<Control>("Visuals");
		DeliveryHighlighter = GetNode<TextureRect>("Visuals/DeliveryHighlighter");

		NameLabel.Text = LocationName;
	}

    private void OnButtonPressed()
	{
		EmitSignal(SignalName.LocationPressed, ID);
	}

	public void SetMovement(float scaleX, float scaleY, Color newLocationColor)
	{
		AnimationPlayer AnimatorX = GetNode<AnimationPlayer>("Visuals/AnimatorX");
		AnimationPlayer AnimatorY = GetNode<AnimationPlayer>("Visuals/AnimatorY");
		AnimatorX.SpeedScale = scaleX;
		AnimatorY.SpeedScale = scaleY;
		LocationColor = newLocationColor;
		DeliveryHighlighter.SelfModulate = LocationColor;
		if (scaleY > 0.5)
		{
			AnimatorX.PlayBackwards("idle");
			AnimatorY.PlayBackwards("idle");
		}
		else
		{
			AnimatorX.Play("idle");
			AnimatorY.Play("idle");
		}
	}

	public void AddQuest(int newQuest) { Quests = Quests.Append(newQuest).ToArray(); UpdateQuests(); }

	public void RemoveQuest(int questID) { Quests = Quests.Where(val => val != questID).ToArray(); UpdateQuests(); }

	public void SetCompleteQuest()
	{
		QuestCompleted = 4;
	}

	public int[] GetQuests() => Quests;

	private void UpdateQuests()
	{
		if (Quests.Length > 0)
		{
			DeliveryHighlightAnimator.Play("highlight");
		}
		else
		{
			DeliveryHighlightAnimator.PlayBackwards("highlight");
		}
	}

	public void AddHazard(string newHazard, Dictionary<string, string> newHazardData, Texture2D newIcon)
	{
		if (!Hazards.Contains(newHazard))
		{
			Hazards = Hazards.Append(newHazard).ToArray();
			HazardsData = HazardsData.Append(newHazardData).ToArray();
			TextureRect newRect = new() { Texture = newIcon };
			HazardsHBox.AddChild(newRect);
			HazardsIcons = HazardsIcons.Append(newRect).ToArray();
			switch (newHazard)
			{
				case "MarketCrash":
					newRect.TooltipText = "Market crash! \nFuel now costs a bit more than usual. \nBut you can stabilize the market by buying 10 fuel from other places.";
					break;
				case "Disentery":
					newRect.TooltipText = "Disentery! \nThe sanitation on this station is not working properly. \nHeat leeches attach to your ship to drain your fuel. \nNext jump costs twice as much.";
					break;
				case "Rum":
					newRect.TooltipText = "The legal department of rum connoisseurs! \nSome pirates are around and it's time to pay your taxes. \nPay 50% if your balance is above $300. \nPay 70% if your balance is above $700.";
					break;
				case "Hectic":
					newRect.TooltipText = "Hectic field! \nSomeone set up a device that makes everything around more hectic! \nEvery delivery is now timed.";
					break;
				case "Bacteria":
					newRect.TooltipText = "Omnivorous bacteria! \nSome dangerous bacteria got into your cargo and is eating through it. \nYou have to deliver everything or it will go bad.";
					break;
				case "Shutdown":
					newRect.TooltipText = "Shutdown! \nThe station is malfuntioning and it won't be available for a while. \nShouldn't take too long.";
					break;
				case "Faulty":
					newRect.TooltipText = "Faulty equipment! \nDue to this station's faulty equipment your cargo will be damaged a bit more than usual.";
					break;
			}
		}
		else GD.Print("Hazard ", newHazard, " already exists at ", LocationName);
	}

	public Dictionary<string, string> GetHazardData(string hazard)
	{
		Dictionary<string, string> requestedHazardData = new();
		for (int i = 0; i < Hazards.Length; i++) if (Hazards[i] == hazard) requestedHazardData = HazardsData[i];
		return requestedHazardData;
	}

	public void RemoveHazard(string hazardName)
	{
		string[] newHazards = Array.Empty<string>();
		Dictionary<string, string>[] newHazardsData = Array.Empty<Dictionary<string, string>>();
		TextureRect[] newHazardsIcons = Array.Empty<TextureRect>();
		for (int i = 0; i < Hazards.Length; i++)
		{
			if (Hazards[i] != hazardName)
			{
				newHazards = newHazards.Append(Hazards[i]).ToArray();
				newHazardsData = newHazardsData.Append(HazardsData[i]).ToArray();
				newHazardsIcons = newHazardsIcons.Append(HazardsIcons[i]).ToArray();
			}
			else HazardsIcons[i].QueueFree();
		}
		Hazards = newHazards;
		HazardsData = newHazardsData;
		HazardsIcons = newHazardsIcons;
	}

	public void MarketCrashUpdate()
	{
		bool removing = false;
		for (int i = 0; i < Hazards.Length; i++)
		{
			if (Hazards[i] == "MarketCrash")
			{
				HazardsData[i]["fuelLeft"] = (int.Parse(HazardsData[i]["fuelLeft"]) - 1).ToString();
				if (HazardsData[i]["fuelLeft"] == "0") removing = true;
				break;
			}
		}
		if (removing) RemoveHazard("MarketCrash");
	}

	public void Highlight() { HighlightAnimator.Play("highlight"); }

	public void ClearHighlight() { HighlightAnimator.PlayBackwards("highlight"); }

	private void OnButtonMouseEntered()
	{
		Animator.Play("select");
	}

	private void OnButtonMouseExited()
	{
		Animator.PlayBackwards("select");
	}

	public void Jump()
	{
		for (int i = 0; i < Hazards.Length; i++)
		{
			if (Hazards[i] == "Shutdown")
			{
				HazardsData[i]["jumps"] = (int.Parse(HazardsData[i]["jumps"]) - 1).ToString();
				if (HazardsData[i]["jumps"] == "0")
				{
					RemoveHazard("Shutdown");
					EmitSignal(SignalName.LocationAvailable, ID);
					Modulate = new Color(1.0f, 1.0f, 1.0f, 1.0f);
					break;
				}
			}
			else if (Hazards[i] == "Faulty")
			{
				HazardsData[i]["jumps"] = (int.Parse(HazardsData[i]["jumps"]) - 1).ToString();
				if (HazardsData[i]["jumps"] == "0")
				{
					RemoveHazard("Faulty");
					break;
				}
			}
		}
	}

	public int GetFuelCost()
	{
		int Cost = FuelCost;
		if (Hazards.Contains("MarketCrash")) Cost = FuelCost * int.Parse(GetHazardData("MarketCrash")["fuelCost"]);
		return Cost;
	}

	public bool BuyFuel(int amount)
	{
		if (FuelLevel < Math.Abs(amount)) return false;
		else
		{
			FuelLevel -= Math.Abs(amount);
			return true;
		}
	}

	public void AddFuel(int amount)
	{
		if (QuestCompleted == 0) FuelLevel = Math.Clamp(FuelLevel + Math.Abs(amount), 0, MaxFuel);
		else QuestCompleted--;
	}

	public int GetFuelLevel()
	{
		return FuelLevel;
	}
}
