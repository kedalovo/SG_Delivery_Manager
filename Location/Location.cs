using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Location : Node2D
{
	[Signal]
	public delegate void LocationPressedEventHandler(Location Itself);
	[Export]
	public string LocationName;
	[Export]
	public int ID;
	private Label NameLabel;
	private Sprite2D StarSprite;
	private Polygon2D QuestPolygon;
	private AnimationPlayer Animator;

	private int[] Quests = Array.Empty<int>();

	public string[] Hazards = Array.Empty<string>();
	public Dictionary<string, string>[] HazardsData = Array.Empty<Dictionary<string, string>>();
	private TextureRect[] HazardsIcons = Array.Empty<TextureRect>();

	private int FuelLevel = 10;
	private int MaxFuel = 10;

	private HBoxContainer HazardsHBox;

	public override void _Ready()
	{
		NameLabel = GetNode<Label>("VBox/Label");
		StarSprite = GetNode<Sprite2D>("StarSprite");
		QuestPolygon = GetNode<Polygon2D>("QuestPolygon");
		Animator = GetNode<AnimationPlayer>("Animator");
		HazardsHBox = GetNode<HBoxContainer>("VBox/HBox");
		ClearDestinations();
	}

	private void OnButtonPressed()
	{
		EmitSignal(SignalName.LocationPressed, this);
	}

	public void AddQuest(int newQuest) { Quests = Quests.Append(newQuest).ToArray(); UpdateQuests(); }

	public void RemoveQuest(int questID) { Quests = Quests.Where(val => val != questID).ToArray(); UpdateQuests(); }

	public int[] GetQuests() => Quests;

	private void UpdateQuests()
	{
		string AddedDeliveries = "";
		foreach (int quest in Quests) AddedDeliveries += "[" + quest.ToString() + "]";
		NameLabel.Text = LocationName + " " + AddedDeliveries;
	}

	public void ClearDestinations()
	{
		NameLabel.Text = LocationName;
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

	public void Highlight() { QuestPolygon.Show(); }

	public void ClearHighlight() { QuestPolygon.Hide(); }

	private void OnButtonMouseEntered() { StarSprite.Scale = new(1.5f, 1.5f); }

	private void OnButtonMouseExited() { StarSprite.Scale = new(1.0f, 1.0f); }

	public void Choosable() { Animator.Play("Choosable"); }

	public void ClearChoosable() { Animator.Play("RESET"); }

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
		FuelLevel = Math.Clamp(FuelLevel + Math.Abs(amount), 0, MaxFuel);
	}
}
