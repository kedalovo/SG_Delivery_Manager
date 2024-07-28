using Godot;
using GodotArray = Godot.Collections.Array;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Main : Node2D
{
	#region Initialization

	private Location CurrentLocation;
	private Dictionary<int, Location> AllLocations = new();
	private Dictionary<int[], Dictionary<string, string>> DrawingConnections = new();
	[Export(PropertyHint.Range, "1, 16,")]
	private int ConSegmentMargin = 1;
	[Export(PropertyHint.Range, "1, 16,")]
	private int ConLineWidth = 1;
	[Export]
	private Color ConLineColor = new();

	private AStar2D StarMap = new();

	private Node2D Ship;
	private Label CurrentLocationLabel;

	private string[] NPCFirstNames = {"Jeremy", "Ashley", "Buck", "Zeta", "Froleen", "John", "Kasma", "Amber", "Rapier", "Sultan", "Sorrow", "Hattempati", "Jeelaz"};
	private string[] NPCLastNames = {"Black", "Threes", "Meener", "Fungal", "Spadow", "Fraulein", "Stix", "Pisstorn", "Jarbin", "Maystone", "Yu", "Suntor", "Lephyn"};

	private Sprite2D NPCBodySprite;
	private Sprite2D NPCFaceSprite;
	private Label NPCNameLabel;

	private Label DestinationLabel;
	private RichTextLabel DeliveryContentsLabel;
	// A list of items to deliver with additional info about them. Key is a string: Name, value is GodotArray: Item texture, durability.
	private ItemData[] DeliveryItems = Array.Empty<ItemData>();
	private int QuestCounter = 1;
	// Displayed quest is a quest that is currently displayed on the quest screen.
	private Quest DisplayedQuest;
	// Accepted quests are formerly displayed quests that have been accepted
	private Dictionary<int, Quest> AcceptedQuests = new();
	private Button AcceptButton;
	private Button DeclineButton;
	private Button FuelButton;

	private VBoxContainer CargoVBox;
	private PackedScene DeliveryScene;

	private Delivery[] Deliveries = Array.Empty<Delivery>();

	private Label MoneyLabel;
	private int balance;
	private int Balance { get => balance; set { MoneyLabel.Text = "$" + value; balance = value; } }

	private int fuel;
	private int FuelLevel { get => fuel; set { FuelLabel.Text = "Fuel:" + value; fuel = value; } } 
	private Label FuelLabel;
	private Label FuelLevelLabel;

	private HBoxContainer ModifiersHBox;
	private PackedScene ModifierIconScene;

	private SpriteFrames TimedFrames;
	private SpriteFrames FragileFrames;
	private SpriteFrames SegmentedFrames;

	private Dictionary<string, SpriteFrames> TagFrames = new();

	private int HazardCounter = 0;
	private Texture2D MarketCrashIcon;
	private Texture2D DisenteryIcon;
	private Texture2D RumIcon;
	private Texture2D HecticIcon;
	private Texture2D BacteriaIcon;
	private Texture2D ShutdownIcon;
	private Texture2D FaultyIcon;

	private bool LeapDay = false;

	private readonly Random Rnd = new();

	#endregion Initialization

	public override void _Ready()
	{
		#region ConnectionsDeclaration
		Location l1 = GetNode<Location>("Locations/Location");
		Location l2 = GetNode<Location>("Locations/Location2");
		Location l3 = GetNode<Location>("Locations/Location3");
		Location l4 = GetNode<Location>("Locations/Location4");
		Location l5 = GetNode<Location>("Locations/Location5");
		Location l6 = GetNode<Location>("Locations/Location6");
		Location l7 = GetNode<Location>("Locations/Location7");
		Location l8 = GetNode<Location>("Locations/Location8");
		Location l9 = GetNode<Location>("Locations/Location9");
		Location l10 = GetNode<Location>("Locations/Location10");
		Location l11 = GetNode<Location>("Locations/Location11");
		Location l12 = GetNode<Location>("Locations/Location12");
		Location l13 = GetNode<Location>("Locations/Location13");
		Location l14 = GetNode<Location>("Locations/Location14");
		Location l15 = GetNode<Location>("Locations/Location15");
		Location l16 = GetNode<Location>("Locations/Location16");

		AllLocations[l1.ID] = l1;
		AllLocations[l2.ID] = l2;
		AllLocations[l3.ID] = l3;
		AllLocations[l4.ID] = l4;
		AllLocations[l5.ID] = l5;
		AllLocations[l6.ID] = l6;
		AllLocations[l7.ID] = l7;
		AllLocations[l8.ID] = l8;
		AllLocations[l9.ID] = l9;
		AllLocations[l10.ID] = l10;
		AllLocations[l11.ID] = l11;
		AllLocations[l12.ID] = l12;
		AllLocations[l13.ID] = l13;
		AllLocations[l14.ID] = l14;
		AllLocations[l15.ID] = l15;
		AllLocations[l16.ID] = l16;

		StarMap.AddPoint(l1.ID, Godot.Vector2.Zero);
		StarMap.AddPoint(l2.ID, new(3f, 0f));
		StarMap.AddPoint(l3.ID, new(4f, 0f));
		StarMap.AddPoint(l4.ID, new(6f, 0f));
		StarMap.AddPoint(l5.ID, new(7f, 0f));
		StarMap.AddPoint(l6.ID, new(7f, 2f));
		StarMap.AddPoint(l7.ID, new(7f, 3f));
		StarMap.AddPoint(l8.ID, new(7f, 6f));
		StarMap.AddPoint(l9.ID, new(4f, 3f));
		StarMap.AddPoint(l10.ID, new(4f, 2f));
		StarMap.AddPoint(l11.ID, new(3f, 2f));
		StarMap.AddPoint(l12.ID, new(2f, 2f));
		StarMap.AddPoint(l13.ID, new(2f, 3f));
		StarMap.AddPoint(l14.ID, new(0f, 3f));
		StarMap.AddPoint(l15.ID, new(2f, 1f));
		StarMap.AddPoint(l16.ID, new(3f, 1f));

		StarMap.ConnectPoints(0, 1);
		StarMap.ConnectPoints(1, 2);
		StarMap.ConnectPoints(2, 3);
		StarMap.ConnectPoints(3, 4);
		StarMap.ConnectPoints(4, 5);
		StarMap.ConnectPoints(5, 6);
		StarMap.ConnectPoints(6, 7);
		StarMap.ConnectPoints(6, 8);
		StarMap.ConnectPoints(8, 9);
		StarMap.ConnectPoints(9, 5);
		StarMap.ConnectPoints(9, 2);
		StarMap.ConnectPoints(9, 10);
		StarMap.ConnectPoints(10, 11);
		StarMap.ConnectPoints(11, 12);
		StarMap.ConnectPoints(8, 12);
		StarMap.ConnectPoints(12, 13);
		StarMap.ConnectPoints(13, 0);
		StarMap.ConnectPoints(11, 14);
		StarMap.ConnectPoints(14, 15);
		StarMap.ConnectPoints(15, 1);

		DrawingConnections[new [] {0, 1}] = new Dictionary<string, string> {{"distance", "3"}, {"color", "2ba69a"}, {"segment_margin", "4"}, {"line_width", "3"}};
		DrawingConnections[new [] {1, 2}] = new Dictionary<string, string> {{"distance", "1"}, {"color", "2ba69a"}, {"segment_margin", "4"}, {"line_width", "3"}};
		DrawingConnections[new [] {2, 3}] = new Dictionary<string, string> {{"distance", "2"}, {"color", "2ba69a"}, {"segment_margin", "4"}, {"line_width", "3"}};
		DrawingConnections[new [] {3, 4}] = new Dictionary<string, string> {{"distance", "1"}, {"color", "2ba69a"}, {"segment_margin", "4"}, {"line_width", "3"}};
		DrawingConnections[new [] {4, 5}] = new Dictionary<string, string> {{"distance", "2"}, {"color", "2ba69a"}, {"segment_margin", "4"}, {"line_width", "3"}};
		DrawingConnections[new [] {5, 6}] = new Dictionary<string, string> {{"distance", "1"}, {"color", "2ba69a"}, {"segment_margin", "4"}, {"line_width", "3"}};
		DrawingConnections[new [] {6, 7}] = new Dictionary<string, string> {{"distance", "3"}, {"color", "2ba69a"}, {"segment_margin", "4"}, {"line_width", "3"}};
		DrawingConnections[new [] {6, 8}] = new Dictionary<string, string> {{"distance", "3"}, {"color", "2ba69a"}, {"segment_margin", "4"}, {"line_width", "3"}};
		DrawingConnections[new [] {8, 9}] = new Dictionary<string, string> {{"distance", "1"}, {"color", "2ba69a"}, {"segment_margin", "4"}, {"line_width", "3"}};
		DrawingConnections[new [] {9, 5}] = new Dictionary<string, string> {{"distance", "3"}, {"color", "2ba69a"}, {"segment_margin", "4"}, {"line_width", "3"}};
		DrawingConnections[new [] {9, 2}] = new Dictionary<string, string> {{"distance", "2"}, {"color", "2ba69a"}, {"segment_margin", "4"}, {"line_width", "3"}};
		DrawingConnections[new [] {9, 10}] = new Dictionary<string, string> {{"distance", "1"}, {"color", "2ba69a"}, {"segment_margin", "4"}, {"line_width", "3"}};
		DrawingConnections[new [] {10, 11}] = new Dictionary<string, string> {{"distance", "1"}, {"color", "2ba69a"}, {"segment_margin", "4"}, {"line_width", "3"}};
		DrawingConnections[new [] {11, 12}] = new Dictionary<string, string> {{"distance", "1"}, {"color", "2ba69a"}, {"segment_margin", "4"}, {"line_width", "3"}};
		DrawingConnections[new [] {8, 12}] = new Dictionary<string, string> {{"distance", "2"}, {"color", "2ba69a"}, {"segment_margin", "4"}, {"line_width", "3"}};
		DrawingConnections[new [] {12, 13}] = new Dictionary<string, string> {{"distance", "2"}, {"color", "2ba69a"}, {"segment_margin", "4"}, {"line_width", "3"}};
		DrawingConnections[new [] {13, 0}] = new Dictionary<string, string> {{"distance", "3"}, {"color", "2ba69a"}, {"segment_margin", "4"}, {"line_width", "3"}};
		DrawingConnections[new [] {11, 14}] = new Dictionary<string, string> {{"distance", "1"}, {"color", "2ba69a"}, {"segment_margin", "4"}, {"line_width", "3"}};
		DrawingConnections[new [] {14, 15}] = new Dictionary<string, string> {{"distance", "1"}, {"color", "2ba69a"}, {"segment_margin", "4"}, {"line_width", "3"}};
		DrawingConnections[new [] {15, 1}] = new Dictionary<string, string> {{"distance", "1"}, {"color", "2ba69a"}, {"segment_margin", "4"}, {"line_width", "3"}};

		#endregion ConnectionsDeclaration

		#region OtherDeclaration

		string HBoxPath = "BottomMenu/PanelContainer/MarginContainer/HBox/";

		Ship = GetNode<Node2D>("Ship");
		CurrentLocationLabel = GetNode<Label>("LeftMenu/PanelContainer/VBox/CurrentLocationLabel");

		NPCBodySprite = GetNode<Sprite2D>("NPC/BodySprite");
		NPCFaceSprite = GetNode<Sprite2D>("NPC/FaceSprite");
		NPCNameLabel = GetNode<Label>("LeftMenu/PanelContainer/VBox/NPCNameLabel");

		DestinationLabel = GetNode<Label>(HBoxPath + "DeliveryVBox/HBox/DestinationLabel");
		DeliveryContentsLabel = GetNode<RichTextLabel>(HBoxPath + "DeliveryVBox/DeliveryContentsLabel");
		AcceptButton = GetNode<Button>(HBoxPath + "DeliveryVBox/HBox/AcceptButton");
		DeclineButton = GetNode<Button>(HBoxPath + "DeliveryVBox/HBox/DeclineButton");
		FuelButton = GetNode<Button>("LeftMenu/PanelContainer/VBox/CenterContainer/VBox/HBox/FuelButton");
		
		DeliveryItems = DeliveryItems.Append(new ItemData() { ItemName = "Wood", Texture = GD.Load<Texture2D>("res://Item Sprites/Wood.png"), Fragility = 1}).ToArray();
		DeliveryItems = DeliveryItems.Append(new ItemData() { ItemName = "Cosmic propaganda", Texture = GD.Load<Texture2D>("res://Item Sprites/Cosmic propaganda.png"), Fragility = 2}).ToArray();
		DeliveryItems = DeliveryItems.Append(new ItemData() { ItemName = "Asteroid samples", Texture = GD.Load<Texture2D>("res://Item Sprites/Cosmic propaganda.png"), Fragility = 3}).ToArray();
		DeliveryItems = DeliveryItems.Append(new ItemData() { ItemName = "Pencils", Texture = GD.Load<Texture2D>("res://Item Sprites/Pencils.png"), Fragility = 4}).ToArray();
		DeliveryItems = DeliveryItems.Append(new ItemData() { ItemName = "Lab vials", Texture = GD.Load<Texture2D>("res://Item Sprites/Lab vials.png"), Fragility = 5}).ToArray();
		DeliveryItems = DeliveryItems.Append(new ItemData() { ItemName = "Miniature Glass City", Texture = GD.Load<Texture2D>("res://Item Sprites/Miniature glass city.png"), Fragility = 6}).ToArray();
		DeliveryItems = DeliveryItems.Append(new ItemData() { ItemName = "Cheesecake", Texture = GD.Load<Texture2D>("res://Item Sprites/Cheesecake.png"), Fragility = 7}).ToArray();
		DeliveryItems = DeliveryItems.Append(new ItemData() { ItemName = "Stuffed Crust Pizza", Texture = GD.Load<Texture2D>("res://Item Sprites/Stuffed crust pizza.png"), Fragility = 8}).ToArray();
		DeliveryItems = DeliveryItems.Append(new ItemData() { ItemName = "Archeology Findings", Texture = GD.Load<Texture2D>("res://Item Sprites/Archeology findings.png"), Fragility = 9}).ToArray();
		DeliveryItems = DeliveryItems.Append(new ItemData() { ItemName = "Old explosives", Texture = GD.Load<Texture2D>("res://Item Sprites/Old explosives.png"), Fragility = 10}).ToArray();
		
		CargoVBox = GetNode<VBoxContainer>(HBoxPath + "CargoVBox/Scroll/CargoVBox");
		DeliveryScene = GD.Load<PackedScene>("res://Delivery/Delivery.tscn");

		MoneyLabel = GetNode<Label>("Control/InfoVBox/MoneyLabel");

		FuelLabel = GetNode<Label>("Control/InfoVBox/FuelLabel");
		FuelLevelLabel = GetNode<Label>("LeftMenu/PanelContainer/VBox/CenterContainer/VBox/HBox/FuelLevelLabel");

		ModifiersHBox = GetNode<HBoxContainer>(HBoxPath + "DeliveryVBox/ModifiersHBox");
		ModifierIconScene = GD.Load<PackedScene>("res://Modifiers/ModifierIcon.tscn");

		TimedFrames = GD.Load<SpriteFrames>("res://Modifiers/SpriteFrames/Timed.tres");
		FragileFrames = GD.Load<SpriteFrames>("res://Modifiers/SpriteFrames/Fragile.tres");
		SegmentedFrames = GD.Load<SpriteFrames>("res://Modifiers/SpriteFrames/Segmented.tres");

		TagFrames["Timed"] = TimedFrames;
		TagFrames["Fragile"] = FragileFrames;
		TagFrames["Segmented"] = SegmentedFrames;

		MarketCrashIcon = GD.Load<Texture2D>("res://Location/Hazards/FuelCrash.png");
		DisenteryIcon = GD.Load<Texture2D>("res://Location/Hazards/HeatLeeches.png");
		RumIcon = GD.Load<Texture2D>("res://Location/Hazards/Pirates.png");
		HecticIcon = GD.Load<Texture2D>("res://Location/Hazards/Timed.png");
		BacteriaIcon = GD.Load<Texture2D>("res://Location/Hazards/Fragile.png");
		ShutdownIcon = GD.Load<Texture2D>("res://Location/Hazards/Shutdown.png");
		FaultyIcon = GD.Load<Texture2D>("res://Location/Hazards/Faulty.png");

		#endregion OtherDeclaration
		
		CurrentLocationLabel.Text = l1.LocationName;
		CurrentLocation = l2;
		UpdateFuelLevelLabel();

		Balance = 0;
		FuelLevel = 10;

		CreateNewNPC();
		DisplayQuest(CreateNewQuest(3));
		// HighlightNeighbours();
	}

    public override void _Process(double delta)
    {
		QueueRedraw();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("test"))
		{
			GD.Print("PRESSED TEST");
			CreateNewHazard();
		}
    }

    private void ChangeConnection(int idFrom, int idTo, int distance, string color = "2ba69a", int segment_margin = 4, int line_width = 3)
	{
		Dictionary<string, string> newData = new() {{"color", color}, {"distance", distance.ToString()}, {"segment_margin", segment_margin.ToString()}, {"line_width", line_width.ToString()}};
		foreach (KeyValuePair<int[], Dictionary<string, string>> Con in DrawingConnections)
		{
			if (Con.Key.Contains(idFrom) && Con.Key.Contains(idTo)) DrawingConnections[Con.Key] = newData;
		}
	}

	private void HighlightSingleConnection(int idFrom, int idTo)
	{
		foreach (KeyValuePair<int[], Dictionary<string, string>> Con in DrawingConnections)
		{
			if (Con.Key.Contains(idFrom) && Con.Key.Contains(idTo))
			{
				DrawingConnections[Con.Key]["color"] = "2c9eca";
			}
		}
	}

	private void StopHighlightSingleConnection(int idFrom, int idTo)
	{
		foreach (KeyValuePair<int[], Dictionary<string, string>> Con in DrawingConnections)
		{
			if (Con.Key.Contains(idFrom) && Con.Key.Contains(idTo))
			{
				DrawingConnections[Con.Key]["color"] = "2ba69a";
			}
		}
	}

	private void HighlightPath(int idFrom, int idTo, string color = "2ba69a")
	{
		long[] pathID = StarMap.GetIdPath(idFrom, idTo);
		Godot.Vector2[] pathPoint = StarMap.GetPointPath(idFrom, idTo);
		for (int i = 0; i < pathID.Length - 1; i++)
		{
			ChangeConnection((int)pathID[i], (int)pathID[i+1], Mathf.FloorToInt(Math.Abs(pathPoint[i].DistanceTo(pathPoint[i+1]))), color, 3, 4);
		}
	}

	private void StopHighlightPath(int idFrom, int idTo)
	{
		long[] pathID = StarMap.GetIdPath(idFrom, idTo);
		Godot.Vector2[] pathPoint = StarMap.GetPointPath(idFrom, idTo);
		for (int i = 0; i < pathID.Length - 1; i++)
		{
			ChangeConnection((int)pathID[i], (int)pathID[i+1], Mathf.FloorToInt(Math.Abs(pathPoint[i].DistanceTo(pathPoint[i+1]))));
		}
	}

    public override void _Draw()
    {
		//int distance, string color, int segment_margin, int line_width
		foreach (KeyValuePair<int[], Dictionary<string, string>> Con in DrawingConnections)
		{
			Godot.Vector2 From = AllLocations[Con.Key[0]].Position;
			Godot.Vector2 To = AllLocations[Con.Key[1]].Position;
			DrawSegmentedLine(From, To, new Color(Con.Value["color"]), int.Parse(Con.Value["distance"]), int.Parse(Con.Value["segment_margin"]), int.Parse(Con.Value["line_width"]));
		}
    }

	private void OnDeliveryHoverStart(int deliveryId)
	{
		HighlightPath(CurrentLocation.ID, AcceptedQuests[deliveryId].Destination.ID, "35ee45");
	}

	private void OnDeliveryHoverFinish(int deliveryId)
	{
		StopHighlightPath(CurrentLocation.ID, AcceptedQuests[deliveryId].Destination.ID);
	}

    private void DrawSegmentedLine(Godot.Vector2 From, Godot.Vector2 To, Color LineColor, int Segments = 1, int SegmentMargin = 1, int LineWidth = 1)
	{
		Godot.Vector2[] LineDrawQueue = Array.Empty<Godot.Vector2>();
		foreach (int i in Enumerable.Range(0, Segments)) LineDrawQueue = LineDrawQueue.Append((To-From)/Segments*i).ToArray();
		LineDrawQueue = LineDrawQueue.Append(To-From).ToArray();

		foreach (int idx in Enumerable.Range(0, LineDrawQueue.Length - 1))
		{
			Godot.Vector2 Fin = (To - From).Normalized();
			Godot.Vector2 P1 = LineDrawQueue[idx] + From + Fin * SegmentMargin;
			Godot.Vector2 P2 = LineDrawQueue[idx+1] + From - Fin * SegmentMargin;
			DrawLine(P1, P2, LineColor, LineWidth);
			DrawCircle(P1, LineWidth/2, LineColor);
			DrawCircle(P2, LineWidth/2, LineColor);
		}
	}

    private Quest CreateNewQuest(int NumOfItems = 1)
	{
		NumOfItems = Math.Clamp(NumOfItems, 1, 10);
		string[] PossibleItems = Array.Empty<string>();
		foreach (ItemData itemData in DeliveryItems) PossibleItems = PossibleItems.Append(itemData.ItemName).ToArray();
		Quest NewQuest = new();
		int i = 0;
		ItemData[] QuestItems = Array.Empty<ItemData>();
		while (i < NumOfItems)
		{
			ItemData NewItem = DeliveryItems[Rnd.Next(DeliveryItems.Length)];
			if (!QuestItems.Contains(NewItem))
			{
				QuestItems = QuestItems.Append(NewItem).ToArray();
				i++;
			}
		}
		NewQuest.Items = QuestItems;

		Location[] PossibleLocations = AllLocations.Values.ToArray();
		Location ChosenLocation = CurrentLocation;
		// Choosing a Location that is not a Current Location? is far enough and isn't disabled
		while
		(
			ChosenLocation.LocationName == CurrentLocation.LocationName
			|
			StarMap.GetIdPath(CurrentLocation.ID, ChosenLocation.ID).Length < 3
			|
			ChosenLocation.Hazards.Contains("Shutdown")
		)
		ChosenLocation = PossibleLocations[Rnd.Next(PossibleLocations.Length)];

		// Adding a modifier
		switch (Rnd.Next(3))
		{
			case 0:
				string newTag = "";
				Dictionary<string, string> newTagData = new();
				switch (Rnd.Next(3))
				{
					case 0:
						newTag = "Timed";
						newTagData["tier"] = (Rnd.Next(3) + 1).ToString();
						break;
					case 1:
						newTag = "Fragile";
						newTagData["jumps"] = (StarMap.GetIdPath(CurrentLocation.ID, ChosenLocation.ID).Length + Rnd.Next(2)).ToString();
						break;
					case 2:
						newTag = "Segmented";
						newTagData["middle-man-id"] = StarMap.GetIdPath(CurrentLocation.ID, ChosenLocation.ID)[Rnd.Next(1, StarMap.GetIdPath(CurrentLocation.ID, ChosenLocation.ID).Length-1)].ToString();
						newTagData["middle-man-name"] = AllLocations[int.Parse(newTagData["middle-man-id"])].LocationName;
						newTagData["middle-man-met"] = "false";
						break;
				}
				NewQuest.AddTag(newTag, newTagData);
				break;
			case 1:
                NewQuest.AddTag("Timed", new Dictionary<string, string>() { {"tier", (Rnd.Next(3) + 1).ToString()} });
				NewQuest.AddTag("Fragile", new Dictionary<string, string>() { {"jumps", (StarMap.GetIdPath(CurrentLocation.ID, ChosenLocation.ID).Length + Rnd.Next(2)).ToString()} });
				GD.Print("COMBO");
				break;
		}

		NewQuest.Destination = ChosenLocation;
		NewQuest.ID = QuestCounter;
		return NewQuest;
	}

	private void OnLocationPressed(int pressedID)
	{
		Location PressedLocation = AllLocations[pressedID];
		if (CurrentLocation != PressedLocation && StarMap.ArePointsConnected(CurrentLocation.ID, PressedLocation.ID) && !StarMap.IsPointDisabled(pressedID))
		{
			int JumpDistance = GetJumpDistance(CurrentLocation, PressedLocation);
			if (CurrentLocation.Hazards.Contains("Disentery")) JumpDistance *= int.Parse(CurrentLocation.GetHazardData("Disentery")["jumpCost"]);
			if (JumpDistance <= FuelLevel)
			{
				if (CurrentLocation.Hazards.Contains("Disentery")) CurrentLocation.RemoveHazard("Disentery");
				StopHighlightPath(CurrentLocation.ID, DisplayedQuest.Destination.ID);
				ClearQuestHighlight();
				// ClearHighlightNeighbours();
				FuelLevel -= JumpDistance;
				CreateNewNPC();
				if (PressedLocation.Hazards.Contains("Faulty"))
				{
					UpdateDeliveries(JumpDistance * 2);
					PressedLocation.RemoveHazard("Faulty");
				}
				else UpdateDeliveries(JumpDistance);
				CurrentLocation = PressedLocation;
				// HighlightNeighbours();
				DisplayQuest(CreateNewQuest(Rnd.Next(11)));
				CurrentLocationLabel.Text = CurrentLocation.LocationName;
				Tween NewTween = GetTree().CreateTween();
				NewTween.TweenProperty(Ship, "position", PressedLocation.Position, .5f).SetTrans(Tween.TransitionType.Circ);
				int[] CompletedQuestIDs = CurrentLocation.GetQuests();
				foreach (int id in CompletedQuestIDs)
				{
					Delivery delivery = GetDeliveryByID(id);
					if (!(delivery.HasTag("Segmented") && delivery.GetTagData("Segmented")["middle-man-met"] == "false"))
					{
						Deliveries = Deliveries.Where(val => val != delivery).ToArray();
						delivery.OnDeliveryFailed -= DeliveryFailed;
						AcceptedQuests.Remove(id);
						CurrentLocation.RemoveQuest(id);
						int Payout = delivery.GetPayout();
						PopupBalance(Payout);
						Balance += Payout;
						delivery.QueueFree();
					}
				}
				if (CompletedQuestIDs.Length > 0) CurrentLocation.SetCompleteQuest();
				foreach (Delivery delivery in Deliveries) delivery.Jump(CurrentLocation.ID);
				// Creating new hazard every 3'rd jump
				if (HazardCounter == 2)
				{
					HazardCounter = 0;
					CreateNewHazard();
				}
				else HazardCounter++;
				// Hazard functionality for arriving at the Location
				if (CurrentLocation.Hazards.Contains("MarketCrash")) FuelButton.Text = "$" + (50 * int.Parse(CurrentLocation.GetHazardData("MarketCrash")["fuelCost"])).ToString();
				else FuelButton.Text = "$50";
				if (CurrentLocation.Hazards.Contains("Rum"))
				{
					int tax = 0;
					Dictionary<string, string> data = CurrentLocation.GetHazardData("Rum");
					if (Balance > 700) tax = Mathf.CeilToInt(int.Parse(data["secondThreshold"]) / 100f * Balance);
					else if (Balance > 300) tax = Mathf.CeilToInt(int.Parse(data["firstThreshold"]) / 100f * Balance);
					Balance -= tax;
					CurrentLocation.RemoveHazard("Rum");
				}
				if (CurrentLocation.Hazards.Contains("Hectic"))
				{
					foreach (Delivery delivery in Deliveries)
					{
						Dictionary<string, string> newTagData = new() { { "tier", CurrentLocation.GetHazardData("Hectic")["tier"] } };
						delivery.AddTag("Timed", newTagData, GetNewModifierIcon("Timed", newTagData));
					}
					CurrentLocation.RemoveHazard("Hectic");
				}
				if (CurrentLocation.Hazards.Contains("Bacteria"))
				{
					foreach (Delivery delivery in Deliveries)
					{
						Dictionary<string, string> newTagData = new();
						Location destination = AcceptedQuests[delivery.ID].Destination;
						int distance = (StarMap.GetIdPath(CurrentLocation.ID, destination.ID).Length + Rnd.Next(2)) * int.Parse(CurrentLocation.GetHazardData("Bacteria")["multiplier"]);
						newTagData["jumps"] = distance.ToString();
						delivery.AddTag("Fragile", newTagData, GetNewModifierIcon("Fragile", newTagData));
					}
					CurrentLocation.RemoveHazard("Bacteria");
				}
				if (LeapDay)
				{
					LeapDay = false;
					foreach (Location location in AllLocations.Values) location.AddFuel(1);
				}
				else LeapDay = true;
				UpdateFuelLevelLabel();
				foreach (Location location in AllLocations.Values) location.Jump();
			}
		}
	}

	private void OnLocationAvailable(int locationID)
	{
		StarMap.SetPointDisabled(locationID, false);
	}

	private void CreateNewNPC()
	{
		NPCNameLabel.Text = string.Format("{0} {1}", NPCFirstNames[Rnd.Next(NPCFirstNames.Length)], NPCLastNames[Rnd.Next(NPCLastNames.Length)]);
		NPCBodySprite.RegionRect = new Rect2(new Godot.Vector2(Rnd.Next(10)*48, 0), new Godot.Vector2(48, 48));
		NPCFaceSprite.RegionRect = new Rect2(new Godot.Vector2(Rnd.Next(10)*32, 0), new Godot.Vector2(32, 32));
	}

	private void CreateNewHazard()
	{
		Location location = CurrentLocation;
		string newHazard = "";
		Dictionary<string, string> newHazardData = new();
		Texture2D newIcon = new();
		switch (Rnd.Next(7))
		{
			case 0: // Fuel market crash. Fuel now costs 2x, goes away by buying 10 fuel elsewhere.
				newHazard = "MarketCrash";
				newHazardData["fuelCost"] = "2";
				newHazardData["fuelLeft"] = "10";
				newIcon = MarketCrashIcon;
				break;
			case 1: // Disentery. Sanitation is not working, which is why heat leeches attach to your ship and aren't disposed of. Next jump is 2x fuel (one time).
				newHazard = "Disentery";
				newHazardData["jumpCost"] = "2";
				newIcon = DisenteryIcon;
				break;
			case 2: // The legal department of rum connoisseurs. You pay 50% of your balance if it's above 300, 70% above 700 (one time).
				newHazard = "Rum";
				newHazardData["firstThreshold"] = "50";
				newHazardData["secondThreshold"] = "70";
				newIcon = RumIcon;
				break;
			case 3: // Hectic field. Every delivery gains tag "Timed" at tier 1 (one time).
				newHazard = "Hectic";
				newHazardData["tier"] = "1";
				newIcon = HecticIcon;
				break;
			case 4: // Omnivorous bacteria. Every delivery gains tag "Fragile" of 2x distance (one time).
				newHazard = "Bacteria";
				newHazardData["multiplier"] = "2";
				newIcon = BacteriaIcon;
				break;
			case 5: // Shutdown. A location is not available for several jumps.
				newHazard = "Shutdown";
				newHazardData["jumps"] = Rnd.Next(2, 5).ToString();
				newIcon = ShutdownIcon;
				break;
			case 6: // Faulty equipment. Jump to this station does 2x durability damage to cargo. Goes away once used of after 10 jumps.
				newHazard = "Faulty";
				newHazardData["jumps"] = "10";
				newIcon = FaultyIcon;
				break;
		}
		if (newHazard == "Shutdown")
		{
			for (int i = 0; i < 50; i++)
			{
				location = AllLocations[Rnd.Next(AllLocations.Count)];
				if (location != CurrentLocation && location.GetQuests().Length == 0) break;
				location = CurrentLocation;
			}
			StarMap.SetPointDisabled(location.ID);
			location.Modulate = new Color(1f, 1f, 1f, 0.5f);
		}
		else while (location == CurrentLocation) location = AllLocations[Rnd.Next(AllLocations.Count)];
		location.AddHazard(newHazard, newHazardData, newIcon);
		GD.Print("Created hazard ", newHazard, " at ", location.LocationName);
	}

	private void DisplayQuest(Quest NewQuest)
	{
		DeliveryContentsLabel.Clear();
		foreach (Node child in ModifiersHBox.GetChildren()) child.QueueFree();
		EnableQuestButtons();
		DisplayedQuest = NewQuest;
		Location DeliveryLocation = NewQuest.Destination;
		DeliveryLocation.Highlight();
		DestinationLabel.Text = DeliveryLocation.LocationName;
		foreach (ItemData item in NewQuest.Items)
		{
			DeliveryContentsLabel.AddText(string.Format("{0} ({1})\n", item.ItemName, item.Fragility));
		}
		for (int i = 0; i < NewQuest.Tags.Length; i++)
		{
			string tag = NewQuest.Tags[i];
			Dictionary<string, string> tagData = NewQuest.TagsData[i];
			ModifiersHBox.AddChild(GetNewModifierIcon(tag, tagData));
		}
		HighlightPath(CurrentLocation.ID, DeliveryLocation.ID, "00b025");
	}

	private void EnableQuestButtons()
	{
		AcceptButton.Disabled = false;
		AcceptButton.Visible = true;
		DeclineButton.Disabled = false;
		DeclineButton.Visible = true;
	}

	private void DisableQuestButtons()
	{
		AcceptButton.Disabled = true;
		AcceptButton.Visible = false;
		DeclineButton.Disabled = true;
		DeclineButton.Visible = false;
	}

	private void OnAcceptQuestButtonPressed()
	{
		StopHighlightPath(CurrentLocation.ID, DisplayedQuest.Destination.ID);
		ClearQuestHighlight();
		DisableQuestButtons();
		AcceptDisplayedQuest();
		QuestCounter++;
	}

	private void OnDeclineQuestButtonPressed()
	{
		StopHighlightPath(CurrentLocation.ID, DisplayedQuest.Destination.ID);
		DisableQuestButtons();
		ClearQuestHighlight();
	}

	private void AcceptDisplayedQuest()
	{
		// Creating entry for currently displayed quest like: [ID]:[DisplayedQuest]
		AcceptedQuests[DisplayedQuest.ID] = DisplayedQuest;
		// Adding a new destination to display on the map
		DisplayedQuest.Destination.AddQuest(DisplayedQuest.ID);
		Delivery NewDelivery = DeliveryScene.Instantiate<Delivery>();
		CargoVBox.AddChild(NewDelivery);
		GodotArray[] Result = Array.Empty<GodotArray>();
		// Adding a delivery in the cargo hold
		foreach (int idx in Enumerable.Range(0, DisplayedQuest.Tags.Length))
		{
			string tag = DisplayedQuest.Tags[idx];
			Dictionary<string, string> tagData = DisplayedQuest.TagsData[idx];
			NewDelivery.AddTag(tag, tagData, GetNewModifierIcon(tag, tagData));
		}
		NewDelivery.SetItems(DisplayedQuest.ID, DisplayedQuest.Items);
		NewDelivery.OnDeliveryFailed += DeliveryFailed;
		NewDelivery.OnDeliveryMouseEntered += OnDeliveryHoverStart;
		NewDelivery.OnDeliveryMouseExited += OnDeliveryHoverFinish;
		Deliveries = Deliveries.Append(NewDelivery).ToArray();
	}

	private void UpdateDeliveries(int Distance) { foreach (Delivery delivery in Deliveries) delivery.Damage(Distance); }

	private void DeliveryFailed(int FailedDeliveryID)
	{
		Delivery FailedDelivery = new();
		foreach (Delivery delivery in Deliveries) if (FailedDeliveryID == delivery.ID) { FailedDelivery = delivery; break; }
		FailedDelivery.OnDeliveryFailed -= DeliveryFailed;
		FailedDelivery.OnDeliveryMouseEntered -= OnDeliveryHoverStart;
		FailedDelivery.OnDeliveryMouseExited -= OnDeliveryHoverFinish;
		AcceptedQuests.Remove(FailedDeliveryID);
		int Payout = FailedDelivery.GetPayout(true);
		PopupBalance(-Payout);
		Balance -= Payout;
		FailedDelivery.QueueFree();
		GD.Print(string.Format("Delivery [{0}] failed!", FailedDeliveryID));
	}

	private void OnFuelButtonPressed()
	{
		int Cost = CurrentLocation.GetFuelCost();
		if (Balance >= Cost)
		{
			if (CurrentLocation.BuyFuel(1))
			{
				Balance -= Cost;
				FuelLevel += 1;
				UpdateFuelLevelLabel();
				PopupBalance(-Cost);
				foreach (Location location in AllLocations.Values) location.MarketCrashUpdate();
			}
		}
	}

	private void ClearQuestHighlight() { DisplayedQuest.Destination.ClearHighlight(); }

	private void HighlightNeighbours()
	{
		long[] Connections = StarMap.GetPointConnections(CurrentLocation.ID);
		foreach (long id in Connections)
		{
			AllLocations[(int)id].Choosable();
			HighlightSingleConnection(CurrentLocation.ID, (int)id);
		}
	}

	private void ClearHighlightNeighbours()
	{
		long[] Connections = StarMap.GetPointConnections(CurrentLocation.ID);
		foreach (long id in Connections)
		{
			AllLocations[(int)id].ClearChoosable();
			StopHighlightSingleConnection(CurrentLocation.ID, (int)id);
		}
	}

	private void PopupBalance(int Difference)
	{
		Label PopupLabel = new();
		if (Difference > 0) { PopupLabel.Text = "$" + Difference.ToString(); PopupLabel.Modulate = Colors.Green; }
		else { PopupLabel.Text = Difference.ToString().Insert(1, "$"); PopupLabel.Modulate = Colors.Red; }
		AddChild(PopupLabel);
		PopupLabel.Position = new Godot.Vector2(512 + Rnd.Next(-64, 64), 16);
		Tween LabelTween = GetTree().CreateTween();
		LabelTween.TweenProperty(PopupLabel, "position:y", (float)Rnd.NextDouble() * 20.0f + 40.0f, 1.0d);
		LabelTween.TweenProperty(PopupLabel, "modulate", Colors.Transparent, 0.3d).SetDelay(0.7d);
        LabelTween.TweenCallback(Callable.From(PopupLabel.QueueFree)).SetDelay(1.0d);
	}

	private void UpdateFuelLevelLabel()
	{
		FuelLevelLabel.Text = CurrentLocation.GetFuelLevel().ToString() + "\nFuel left";
	}

	private int GetLocationID(string NameOfLocation)
	{
		foreach (Location loc in AllLocations.Values) { if (loc.LocationName == NameOfLocation) { return loc.ID; } }
		GD.PrintErr("Could not find location with a name: ", NameOfLocation);
		return -1;
	}

	private int GetJumpDistance(Location From, Location To)
	{
		Godot.Vector2[] Path = StarMap.GetPointPath(From.ID, To.ID);
		Godot.Vector2 Distance = Path[1] - Path[0];
		if (Distance.X == 0f) return (int)Mathf.Abs(Distance.Y);
		else return (int)Mathf.Abs(Distance.X);
	}

	private ItemData GetItemByName(string NameOfItem)
	{
		foreach (ItemData itemData in DeliveryItems) if (itemData.ItemName == NameOfItem) return itemData;
		return new();
	}

	private Delivery GetDeliveryByID(int IdOfDelivery)
	{
		foreach (Delivery delivery in Deliveries)
		if (IdOfDelivery == delivery.ID) return delivery;
		return new();
	}

	private ModifierIcon GetNewModifierIcon(string newTag, Dictionary<string, string> newTagData)
	{
		ModifierIcon NewModifier = ModifierIconScene.Instantiate<ModifierIcon>();
		NewModifier.Tag = newTag;
		NewModifier.SetSprite(TagFrames[newTag]);
		switch (newTag)
		{
			case "Timed":
				NewModifier.SetData((20 - int.Parse(newTagData["tier"]) * 5).ToString());
				break;
			case "Fragile":
				NewModifier.SetData(newTagData["jumps"]);
				break;
			case "Segmented":
				NewModifier.SetData(newTagData["middle-man-name"]);
				break;
		}
		return NewModifier;
	}
}