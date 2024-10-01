using Godot;
using GodotArray = Godot.Collections.Array;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Main : Node2D
{
	#region Initialization

	private Location CurrentLocation;
	private Dictionary<int, Location> AllLocations;
	private Dictionary<int[], Dictionary<string, string>> DrawingConnections;
	private Dictionary<long, Dictionary<int[], Dictionary<string, string>>> DrawingPaths;
	private Dictionary<long, int[]> ReadableConnections;
	private long PathCounter;
	[Export(PropertyHint.Range, "1, 16,")]
	private int ConSegmentMargin = 1;
	[Export(PropertyHint.Range, "1, 16,")]
	private int ConLineWidth = 1;
	[Export]
	private Color ConLineColor = new();

	private AStar2D StarMap;

	private Ship Spaceship;
	private Label CurrentLocationLabel;

	private string[] NPCFirstNames = {"Jeremy", "Ashley", "Buck", "Zeta", "Froleen", "John", "Kasma", "Amber", "Rapier", "Sultan", "Sorrow", "Hattempati", "Jeelaz"};
	private string[] NPCLastNames = {"Black", "Threes", "Meener", "Fungal", "Spadow", "Fraulein", "Stix", "Pisstorn", "Jarbin", "Maystone", "Yu", "Suntor", "Lephyn"};

	private Sprite2D NPCBodySprite;
	private Sprite2D NPCFaceSprite;
	private Label NPCNameLabel;
	private Node2D NPCContainer;

	private Label DestinationLabel;
	private Control DestinationPlanet;
	private RichTextLabel DeliveryContentsLabel;
	// A list of items to deliver with additional info about them. Key is a string: Name, value is GodotArray: Item texture, durability.
	private ItemData[] DeliveryItems;
	private int QuestCounter;
	// Displayed quest is a quest that is currently displayed on the quest screen.
	private Quest DisplayedQuest;
	// Accepted quests are formerly displayed quests that have been accepted
	private Dictionary<int, Quest> AcceptedQuests;
	private Button AcceptButton;
	private Button DeclineButton;
	private Button FuelButton;

	private VBoxContainer CargoVBox;
	private PackedScene DeliveryScene;

	private Delivery[] Deliveries;

	private VBoxContainer DeliveriesContainer;

	private Label MoneyLabel;
	private int balance;
	private int Balance { get => balance; set { MoneyLabel.Text = "$" + value; balance = value; } }

	private int fuel;
	private int FuelLevel { get => fuel; set { FuelLabel.Text = value.ToString(); fuel = value; } } 
	private Label FuelLabel;
	private Label FuelLevelLabel;

	private HBoxContainer ModifiersHBox;
	private PackedScene ModifierIconScene;

	private SpriteFrames TimedFrames;
	private SpriteFrames FragileFrames;
	private SpriteFrames SegmentedFrames;

	// private Dictionary<string, SpriteFrames> TagFrames;

	private int HazardCounter;
	private Texture2D MarketCrashIcon;
	private Texture2D DisenteryIcon;
	private Texture2D RumIcon;
	private Texture2D HecticIcon;
	private Texture2D BacteriaIcon;
	private Texture2D ShutdownIcon;
	private Texture2D FaultyIcon;

	// private Button AchieventsButton;
	// private PopupPanel AchievementsMenu;

	private enum Achievements
	{
		TOTAL_DELIVERIES_1,
		TOTAL_DELIVERIES_2,
		TOTAL_DELIVERIES_3,
		TOTAL_ITEMS_1,
		TOTAL_ITEMS_2,
		TOTAL_ITEMS_3,
		TOTAL_DIFFERENT_ITEMS_1,
		TOTAL_DIFFERENT_ITEMS_2,
		TOTAL_DIFFERENT_ITEMS_3,
		TOTAL_FAILS_1,
		TOTAL_FAILS_2,
		TOTAL_FAILS_3,
		HAVING_MULTIPLE_DELIVERIES_1,
		HAVING_MULTIPLE_DELIVERIES_2,
		HAVING_MULTIPLE_DELIVERIES_3,
		MAKING_MULTIPLE_DELIVERIES_1,
		MAKING_MULTIPLE_DELIVERIES_2,
		MAKING_MULTIPLE_DELIVERIES_3,
	}

	private MarginContainer AchievementsContainer;
	private VBoxContainer AchievementsVBoxContainer;
	private PanelContainer AchievementRect;
	private Achievement[] AchievementQueue;

	private int ACH_TotalDeliveries;
	private int ACH_TotalItems;
	private Dictionary<string, int> ACH_TotalDifferentItems;
	private int ACH_TotalFails;
	private int ACH_CompletedQuests;

	private PopupPanel VictoryScreen;
	private PopupPanel FailScreen;

	private Tween AchievementTween;

	private bool LeapDay = false;

	// private Texture2D AlphaMini;
	// private Texture2D AuroraMini;
	// private Texture2D BetaMini;
	// private Texture2D BorealisMini;
	// private Texture2D CupidMini;
	// private Texture2D DreadMini;
	// private Texture2D EpsilonMini;
	// private Texture2D FateMini;
	// private Texture2D GammaMini;
	// private Texture2D OmegaMini;
	// private Texture2D QuatroMini;
	// private Texture2D SigmaMini;
	// private Texture2D ThetaMini;
	// private Texture2D TitanMini;
	// private Texture2D TrifectaMini;
	// private Texture2D ZeppelinMini;

	private Dictionary<string, PackedScene> PlanetScenes;
	private Dictionary<string, Color[]> PlanetColorPresets;
	private Dictionary<string, uint> PlanetSeeds;

	private Node2D AsteroidContainer;

	private Location LastPressedLocation;
	private int LastJumpDistance;

	private Random Rnd;

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

		AllLocations = new();

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

		StarMap = new();

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

		DrawingConnections = new();
        DrawingPaths = new();
		ReadableConnections = new();

		DrawingConnections[new [] {0, 1}] = new Dictionary<string, string> {{"distance", "3"}, {"color", "ffffff"}, {"line_width", "1"}};
		DrawingConnections[new [] {1, 2}] = new Dictionary<string, string> {{"distance", "1"}, {"color", "ffffff"}, {"line_width", "1"}};
		DrawingConnections[new [] {2, 3}] = new Dictionary<string, string> {{"distance", "2"}, {"color", "ffffff"}, {"line_width", "1"}};
		DrawingConnections[new [] {3, 4}] = new Dictionary<string, string> {{"distance", "1"}, {"color", "ffffff"}, {"line_width", "1"}};
		DrawingConnections[new [] {4, 5}] = new Dictionary<string, string> {{"distance", "2"}, {"color", "ffffff"}, {"line_width", "1"}};
		DrawingConnections[new [] {5, 6}] = new Dictionary<string, string> {{"distance", "1"}, {"color", "ffffff"}, {"line_width", "1"}};
		DrawingConnections[new [] {6, 7}] = new Dictionary<string, string> {{"distance", "3"}, {"color", "ffffff"}, {"line_width", "1"}};
		DrawingConnections[new [] {6, 8}] = new Dictionary<string, string> {{"distance", "3"}, {"color", "ffffff"}, {"line_width", "1"}};
		DrawingConnections[new [] {8, 9}] = new Dictionary<string, string> {{"distance", "1"}, {"color", "ffffff"}, {"line_width", "1"}};
		DrawingConnections[new [] {9, 5}] = new Dictionary<string, string> {{"distance", "3"}, {"color", "ffffff"}, {"line_width", "1"}};
		DrawingConnections[new [] {9, 2}] = new Dictionary<string, string> {{"distance", "2"}, {"color", "ffffff"}, {"line_width", "1"}};
		DrawingConnections[new [] {9, 10}] = new Dictionary<string, string> {{"distance", "1"}, {"color", "ffffff"}, {"line_width", "1"}};
		DrawingConnections[new [] {10, 11}] = new Dictionary<string, string> {{"distance", "1"}, {"color", "ffffff"}, {"line_width", "1"}};
		DrawingConnections[new [] {11, 12}] = new Dictionary<string, string> {{"distance", "1"}, {"color", "ffffff"}, {"line_width", "1"}};
		DrawingConnections[new [] {8, 12}] = new Dictionary<string, string> {{"distance", "2"}, {"color", "ffffff"}, {"line_width", "1"}};
		DrawingConnections[new [] {12, 13}] = new Dictionary<string, string> {{"distance", "2"}, {"color", "ffffff"}, {"line_width", "1"}};
		DrawingConnections[new [] {13, 0}] = new Dictionary<string, string> {{"distance", "3"}, {"color", "ffffff"}, {"line_width", "1"}};
		DrawingConnections[new [] {11, 14}] = new Dictionary<string, string> {{"distance", "1"}, {"color", "ffffff"}, {"line_width", "1"}};
		DrawingConnections[new [] {14, 15}] = new Dictionary<string, string> {{"distance", "1"}, {"color", "ffffff"}, {"line_width", "1"}};
		DrawingConnections[new [] {15, 1}] = new Dictionary<string, string> {{"distance", "1"}, {"color", "ffffff"}, {"line_width", "1"}};

		#endregion ConnectionsDeclaration

		#region OtherDeclaration

		string HBoxPath = "BottomMenu/PanelContainer/MarginContainer/HBox/";

		Spaceship = GetNode<Ship>("Ship");
		CurrentLocationLabel = GetNode<Label>("LeftMenu/PanelContainer/VBox/CurrentLocationLabel");

		NPCBodySprite = GetNode<Sprite2D>("UI/UIHBox/NPCControl/Panel/SpriteContainer/BodySprite");
		NPCFaceSprite = GetNode<Sprite2D>("UI/UIHBox/NPCControl/Panel/SpriteContainer/FaceSprite");
		NPCNameLabel = GetNode<Label>("UI/UIHBox/NPCControl/NamePanel/NameLabel");
		NPCContainer = GetNode<Node2D>("UI/UIHBox/NPCControl/Panel/SpriteContainer");

		DestinationLabel = GetNode<Label>("UI/NewQuestContainer/PanelContainer/MarginContainer/VBox/HBox/DestinationLabel");
		DestinationPlanet = GetNode<Control>("UI/NewQuestContainer/PanelContainer/MarginContainer/VBox/HBox/PlanetSpace");
		DeliveryContentsLabel = GetNode<RichTextLabel>("UI/NewQuestContainer/PanelContainer/MarginContainer/VBox/DeliveryContentsLabel");
		AcceptButton = GetNode<Button>("UI/NewQuestContainer/PanelContainer/MarginContainer/VBox/HBox/AcceptButton");
		DeclineButton = GetNode<Button>(HBoxPath + "DeliveryVBox/HBox/DeclineButton");
		FuelButton = GetNode<Button>("UI/UIHBox/VBox/FuelPanel/FuelVBox/HBox/FuelButton");

		DeliveryItems = Array.Empty<ItemData>();
		
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

		QuestCounter = 1;
		HazardCounter = 0;

		AcceptedQuests = new();
		Deliveries = Array.Empty<Delivery>();
		
		CargoVBox = GetNode<VBoxContainer>(HBoxPath + "CargoVBox/Scroll/CargoVBox");
		DeliveryScene = GD.Load<PackedScene>("res://Delivery/NewDelivery.tscn");

		DeliveriesContainer = GetNode<VBoxContainer>("UI/DeliveriesContainer/ScrollContainer/VBoxContainer");

		MoneyLabel = GetNode<Label>("UI/UIHBox/VBox/BalancePanel/BalanceHBox/BalanceLabel");

		FuelLabel = GetNode<Label>("UI/UIHBox/VBox/FuelPanel/FuelVBox/LabelsHBox/FuelLabel");

		ModifiersHBox = GetNode<HBoxContainer>("UI/NewQuestContainer/PanelContainer/MarginContainer/VBox/ModifiersHBox");
		ModifierIconScene = GD.Load<PackedScene>("res://Modifiers/ModifierIcon.tscn");

		TimedFrames = GD.Load<SpriteFrames>("res://Modifiers/SpriteFrames/Timed.tres");
		FragileFrames = GD.Load<SpriteFrames>("res://Modifiers/SpriteFrames/Fragile.tres");
		SegmentedFrames = GD.Load<SpriteFrames>("res://Modifiers/SpriteFrames/Segmented.tres");

		// TagFrames = new()
		// {
		// 	["Timed"] = TimedFrames,
		// 	["Fragile"] = FragileFrames,
		// 	["Segmented"] = SegmentedFrames
		// };

		MarketCrashIcon = GD.Load<Texture2D>("res://Location/Hazards/New Icons/FuelCrash.svg");
		DisenteryIcon = GD.Load<Texture2D>("res://Location/Hazards/New Icons/HeatLeeches.svg");
		RumIcon = GD.Load<Texture2D>("res://Location/Hazards/New Icons/Pirates.svg");
		HecticIcon = GD.Load<Texture2D>("res://Location/Hazards/New Icons/Timed.svg");
		BacteriaIcon = GD.Load<Texture2D>("res://Location/Hazards/New Icons/Fragile.svg");
		ShutdownIcon = GD.Load<Texture2D>("res://Location/Hazards/New Icons/Shutdown.svg");
		FaultyIcon = GD.Load<Texture2D>("res://Location/Hazards/New Icons/Faulty.svg");

		AchievementQueue = Array.Empty<Achievement>();

		// AchieventsButton = GetNode<Button>("UI/MarginContainer/PanelContainer/VBox/ExitButton");
		// AchievementsMenu = GetNode<PopupPanel>("AchievementsMenu");

		AchievementsContainer = GetNode<MarginContainer>("UI/AchievementsContainer");
		AchievementsVBoxContainer = GetNode<VBoxContainer>("UI/AchievementsContainer/PanelContainer/Scroll/AchievementsVBox");
		AchievementRect = GetNode<PanelContainer>("UI/PanelContainer");

		ACH_TotalDifferentItems = new();

		ACH_TotalDifferentItems["Wood"] = 0;
		ACH_TotalDifferentItems["Cosmic propaganda"] = 0;
		ACH_TotalDifferentItems["Asteroid samples"] = 0;
		ACH_TotalDifferentItems["Pencils"] = 0;
		ACH_TotalDifferentItems["Lab vials"] = 0;
		ACH_TotalDifferentItems["Miniature Glass City"] = 0;
		ACH_TotalDifferentItems["Cheesecake"] = 0;
		ACH_TotalDifferentItems["Stuffed Crust Pizza"] = 0;
		ACH_TotalDifferentItems["Archeology Findings"] = 0;
		ACH_TotalDifferentItems["Old explosives"] = 0;

		ACH_TotalDeliveries = 0;
		ACH_TotalItems = 0;
		ACH_TotalFails = 0;
		ACH_CompletedQuests = 0;

		VictoryScreen = GetNode<PopupPanel>("VictoryScreen");
		FailScreen = GetNode<PopupPanel>("FailScreen");

		// AlphaMini = GD.Load<Texture2D>("res://UI/Locations/Alpha/Alpha_mini.png");
		// AuroraMini = GD.Load<Texture2D>("res://UI/Locations/Aurora/Aurora_mini.png");
		// BetaMini = GD.Load<Texture2D>("res://UI/Locations/Beta/Beta_mini.png");
		// BorealisMini = GD.Load<Texture2D>("res://UI/Locations/Borealis/Borealis_mini.png");
		// CupidMini = GD.Load<Texture2D>("res://UI/Locations/Cupid/Cupid_mini.png");
		// DreadMini = GD.Load<Texture2D>("res://UI/Locations/Dread/Dread_mini.png");
		// EpsilonMini = GD.Load<Texture2D>("res://UI/Locations/Epsilon/Epsilon_mini.png");
		// FateMini = GD.Load<Texture2D>("res://UI/Locations/Fate/Fate_mini.png");
		// GammaMini = GD.Load<Texture2D>("res://UI/Locations/Gamma/Gamma_mini.png");
		// OmegaMini = GD.Load<Texture2D>("res://UI/Locations/Omega/Omega_mini.png");
		// QuatroMini = GD.Load<Texture2D>("res://UI/Locations/Quatro/Quatro_mini.png");
		// SigmaMini = GD.Load<Texture2D>("res://UI/Locations/Sigma/Sigma_mini.png");
		// ThetaMini = GD.Load<Texture2D>("res://UI/Locations/Theta/Theta_mini.png");
		// TitanMini = GD.Load<Texture2D>("res://UI/Locations/Titan/Titan_mini.png");
		// TrifectaMini = GD.Load<Texture2D>("res://UI/Locations/Trifecta/Trifecta_mini.png");
		// ZeppelinMini = GD.Load<Texture2D>("res://UI/Locations/Zeppelin/Zeppelin_mini.png");

		// l1.SetTexture(AuroraMini);
		// l2.SetTexture(AlphaMini);
		// l3.SetTexture(BetaMini);
		// l4.SetTexture(GammaMini);
		// l5.SetTexture(EpsilonMini);
		// l6.SetTexture(BorealisMini);
		// l7.SetTexture(OmegaMini);
		// l8.SetTexture(ZeppelinMini);
		// l9.SetTexture(TrifectaMini);
		// l10.SetTexture(CupidMini);
		// l11.SetTexture(ThetaMini);
		// l12.SetTexture(FateMini);
		// l13.SetTexture(SigmaMini);
		// l14.SetTexture(QuatroMini);
		// l15.SetTexture(TitanMini);
		// l16.SetTexture(DreadMini);

		l1.LocationPressed += OnLocationPressed;
		l2.LocationPressed += OnLocationPressed;
		l3.LocationPressed += OnLocationPressed;
		l4.LocationPressed += OnLocationPressed;
		l5.LocationPressed += OnLocationPressed;
		l6.LocationPressed += OnLocationPressed;
		l7.LocationPressed += OnLocationPressed;
		l8.LocationPressed += OnLocationPressed;
		l9.LocationPressed += OnLocationPressed;
		l10.LocationPressed += OnLocationPressed;
		l11.LocationPressed += OnLocationPressed;
		l12.LocationPressed += OnLocationPressed;
		l13.LocationPressed += OnLocationPressed;
		l14.LocationPressed += OnLocationPressed;
		l15.LocationPressed += OnLocationPressed;
		l16.LocationPressed += OnLocationPressed;

		PlanetScenes = new() {
			{"Asteroids", GD.Load<PackedScene>("res://PixelPlanets/Asteroids/Asteroid.tscn")},
			{"BlackHole", GD.Load<PackedScene>("res://PixelPlanets/BlackHole/BlackHole.tscn")},
			{"DryTerran", GD.Load<PackedScene>("res://PixelPlanets/DryTerran/DryTerran.tscn")},
			{"Galaxy", GD.Load<PackedScene>("res://PixelPlanets/Galaxy/Galaxy.tscn")},
			{"GasPlanet", GD.Load<PackedScene>("res://PixelPlanets/GasPlanet/GasPlanet.tscn")},
			{"GasPlanetLayers", GD.Load<PackedScene>("res://PixelPlanets/GasPlanetLayers/GasPlanetLayers.tscn")},
			{"IceWorld", GD.Load<PackedScene>("res://PixelPlanets/IceWorld/IceWorld.tscn")},
			{"LandMasses", GD.Load<PackedScene>("res://PixelPlanets/LandMasses/LandMasses.tscn")},
			{"LavaWorld", GD.Load<PackedScene>("res://PixelPlanets/LavaWorld/LavaWorld.tscn")},
			{"NoAtmosphere", GD.Load<PackedScene>("res://PixelPlanets/NoAtmosphere/NoAtmosphere.tscn")},
			{"Rivers", GD.Load<PackedScene>("res://PixelPlanets/Rivers/Rivers.tscn")},
			{"Star", GD.Load<PackedScene>("res://PixelPlanets/Star/Star.tscn")}
		};

		PlanetColorPresets = new() {
			{"Alpha", new Color[] {
				new("ad4860"), new("823a36"), new("563624"), new("2b2312"), new("94aac1"),
				new("4c4a61"), new("d8f0da"), new("b1e1c2"), new("8bd2ba"), new("64c3bf")}},
			{"Aurora", new Color[] {
				new("d69fdc"), new("916fa0"), new("524568"), new("242539"), new("0a0e17"),
				new("000003")}},
			{"Beta", new Color[] {
				new("feb9e8"), new("c8b6cd"), new("899da5"), new("4c7272"), new("1d3a39")}},
			{"Borealis", new Color[] {
				new("23332e"), new("15302e"), new("0a2a2b"), new("042123"), new("84f5fc"),
				new("60d3d7"), new("419e9e"), new("235855")}},
			{"Cupid", new Color[] {
				new("30ffff"), new("2acece"), new("58677b"), new("871d36"), new("8b070d"),
				new("5c1200")}},
			{"Dread", new Color[] {
				new("1a1212"), new("100a0b"), new("000000"), new("2b1519"), new("060606")}},
			{"Epsilon", new Color[] {
				new("96da61"), new("3a7446"), new("2a2549"), new("3a7446"), new("2a2549")}},
			{"Fate", new Color[] {
				new("be4f48"), new("7f4d30"), new("3f3218"), new("50418c"), new("492c5e"),
				new("2f162f"), new("e2e4f3"), new("cac4e6"), new("bfa7da"), new("be8acd")}},
			{"Gamma", new Color[] {
				new("457b9f"), new("374d7f"), new("2a2a5f"), new("aba843"), new("6b8032"),
				new("385622"), new("142b11"), new("e9dfea"), new("d5bfd1"), new("c09eb0"),
				new("ab7e88")}},
			{"Omega", new Color[] {
				new("e3ffff"), new("a6c5e4"), new("7a75b6"), new("7090fa"), new("3972ae"),
				new("2e397f"), new("a5d8ff"), new("c7e6ff"), new("5e70a5"), new("404973")}},
			{"Quatro", new Color[] {
				new("8c3b3b"), new("592d2d"), new("3a1f20"), new("472c2f"), new("301d1d"),
				new("ff6e33"), new("df3327"), new("be102e")}},
			{"Sigma", new Color[] {
				new("41b2be"), new("31698e"), new("21345f"), new("10102f"), new("644658"),
				new("322323"), new("ede7e2"), new("dcd6c5"), new("c9caa9"), new("a9b88c")}},
			{"Theta", new Color[] {
				new("fdf5fa"), new("fdf5fa"), new("ff7aa0"), new("e13c4f"), new("751f13"),
				new("ff7aa0"), new("fdf5fa")}},
			{"Titan", new Color[] {
				new("8c3b3b"), new("592d2d"), new("3a1f20"), new("472c2f"), new("301d1d"),
				new("ff5c1a"), new("e31f10"), new("ff002d")}},
			{"Trifecta", new Color[] {
				new("f6fef3"), new("f6fef3"), new("81b576"), new("3c5b3c"), new("181619"),
				new("81b576"), new("f6fef3")}},
			{"Zeppelin", new Color[] {
				new("000000"), new("ffffeb"), new("ed7b39"), new("ffffeb"), new("fff540"),
				new("ffb84a"), new("ed7b39"), new("bd4035")}},
		};

		PlanetSeeds = new() {
			{"Alpha", 261590536},
			{"Aurora", 1858028818},
			{"Beta", 2085605575},
			{"Borealis", 1977630260},
			{"Cupid", 2195664270},
			{"Dread", 2446316385},
			{"Epsilon", 416601145},
			{"Fate", 2150406137},
			{"Gamma", 4102919597},
			{"Omega", 2195664270},
			{"Quatro", 3652283469},
			{"Sigma", 1862326554},
			{"Theta", 58942833},
			{"Titan", 1889712776},
			{"Trifecta", 692943967},
			{"Zeppelin", 2069105553}
		};

		Rnd = new();

		CreateNewPlanetAtLocation(l1, "GasPlanetLayers", "Aurora");
		CreateNewPlanetAtLocation(l2, "Rivers", "Alpha");
		CreateNewPlanetAtLocation(l3, "DryTerran", "Beta");
		CreateNewPlanetAtLocation(l4, "LandMasses", "Gamma");
		CreateNewPlanetAtLocation(l5, "NoAtmosphere", "Epsilon");
		CreateNewPlanetAtLocation(l6, "GasPlanet", "Borealis");
		CreateNewPlanetAtLocation(l7, "IceWorld", "Omega");
		CreateNewPlanetAtLocation(l8, "BlackHole", "Zeppelin");
		CreateNewPlanetAtLocation(l9, "Star", "Trifecta");
		CreateNewPlanetAtLocation(l10, "GasPlanetLayers", "Cupid");
		CreateNewPlanetAtLocation(l11, "Star", "Theta");
		CreateNewPlanetAtLocation(l12, "Rivers", "Fate");
		CreateNewPlanetAtLocation(l13, "Rivers", "Sigma");
		CreateNewPlanetAtLocation(l14, "LavaWorld", "Quatro");
		CreateNewPlanetAtLocation(l15, "LavaWorld", "Titan");
		CreateNewPlanetAtLocation(l16, "NoAtmosphere", "Dread");

		l1.SetMovement(Rnd.NextSingle() + 1.0f, Rnd.NextSingle(), Color.FromHsv(Rnd.NextSingle(), Rnd.Next(30, 70) / 100.0f, Rnd.Next(50, 100) / 100.0f));
		l2.SetMovement(Rnd.NextSingle() + 1.0f, Rnd.NextSingle(), Color.FromHsv(Rnd.NextSingle(), Rnd.Next(30, 70) / 100.0f, Rnd.Next(50, 100) / 100.0f));
		l3.SetMovement(Rnd.NextSingle() + 1.0f, Rnd.NextSingle(), Color.FromHsv(Rnd.NextSingle(), Rnd.Next(30, 70) / 100.0f, Rnd.Next(50, 100) / 100.0f));
		l4.SetMovement(Rnd.NextSingle() + 1.0f, Rnd.NextSingle(), Color.FromHsv(Rnd.NextSingle(), Rnd.Next(30, 70) / 100.0f, Rnd.Next(50, 100) / 100.0f));
		l5.SetMovement(Rnd.NextSingle() + 1.0f, Rnd.NextSingle(), Color.FromHsv(Rnd.NextSingle(), Rnd.Next(30, 70) / 100.0f, Rnd.Next(50, 100) / 100.0f));
		l6.SetMovement(Rnd.NextSingle() + 1.0f, Rnd.NextSingle(), Color.FromHsv(Rnd.NextSingle(), Rnd.Next(30, 70) / 100.0f, Rnd.Next(50, 100) / 100.0f));
		l7.SetMovement(Rnd.NextSingle() + 1.0f, Rnd.NextSingle(), Color.FromHsv(Rnd.NextSingle(), Rnd.Next(30, 70) / 100.0f, Rnd.Next(50, 100) / 100.0f));
		l8.SetMovement(Rnd.NextSingle() + 1.0f, Rnd.NextSingle(), Color.FromHsv(Rnd.NextSingle(), Rnd.Next(30, 70) / 100.0f, Rnd.Next(50, 100) / 100.0f));
		l9.SetMovement(Rnd.NextSingle() + 1.0f, Rnd.NextSingle(), Color.FromHsv(Rnd.NextSingle(), Rnd.Next(30, 70) / 100.0f, Rnd.Next(50, 100) / 100.0f));
		l10.SetMovement(Rnd.NextSingle() + 1.0f, Rnd.NextSingle(), Color.FromHsv(Rnd.NextSingle(), Rnd.Next(30, 70) / 100.0f, Rnd.Next(50, 100) / 100.0f));
		l11.SetMovement(Rnd.NextSingle() + 1.0f, Rnd.NextSingle(), Color.FromHsv(Rnd.NextSingle(), Rnd.Next(30, 70) / 100.0f, Rnd.Next(50, 100) / 100.0f));
		l12.SetMovement(Rnd.NextSingle() + 1.0f, Rnd.NextSingle(), Color.FromHsv(Rnd.NextSingle(), Rnd.Next(30, 70) / 100.0f, Rnd.Next(50, 100) / 100.0f));
		l13.SetMovement(Rnd.NextSingle() + 1.0f, Rnd.NextSingle(), Color.FromHsv(Rnd.NextSingle(), Rnd.Next(30, 70) / 100.0f, Rnd.Next(50, 100) / 100.0f));
		l14.SetMovement(Rnd.NextSingle() + 1.0f, Rnd.NextSingle(), Color.FromHsv(Rnd.NextSingle(), Rnd.Next(30, 70) / 100.0f, Rnd.Next(50, 100) / 100.0f));
		l15.SetMovement(Rnd.NextSingle() + 1.0f, Rnd.NextSingle(), Color.FromHsv(Rnd.NextSingle(), Rnd.Next(30, 70) / 100.0f, Rnd.Next(50, 100) / 100.0f));
		l16.SetMovement(Rnd.NextSingle() + 1.0f, Rnd.NextSingle(), Color.FromHsv(Rnd.NextSingle(), Rnd.Next(30, 70) / 100.0f, Rnd.Next(50, 100) / 100.0f));

		AsteroidContainer = GetNode<Node2D>("Asteroids");

		#endregion OtherDeclaration

		CurrentLocationLabel.Text = l2.LocationName;
		CurrentLocation = l2;
		Spaceship.Transform = CurrentLocation.Transform;

		Balance = 0;
		FuelLevel = 1000;

		CreateNewNPC();
		DisplayQuest(CreateNewQuest(0, 1));
		FailScreen.Hide();

		AchievementTween = GetTree().CreateTween();
		AchievementTween.Stop();

		// Asteroids should be constantly spawning, no matter what. But they must spawn off screen.
		// Maybe not? What if they are hidden initially and then fade in? Let's try that instead, much more simple.

		// Sure, position spawning problem solved, another problem is time - when do we spawn new ones?
		// I want them all to last 20 seconds each, no need to randomize.
		// Spawning a new one as soon as old ones go out would need the initial ones to be on timer.
		// Maybe I should put a timer in Asteroids node to spawn each asteroid.
		// That way we don't care when old ones disappear, just when new ones appear.
		// So timer spawns asteroid each second, each lasts 20 seconds, so there's roughly 20 asteroids on the screen at each moment.
		// Really, it's less, because they will inevitably go out of bounds, even with spawn restriction.
	}

	public override void _Process(double delta)
	{
		QueueRedraw();
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("test"))
		{
			GD.Print("TESTING");
			// GetTree().Paused = true;
			// CompleteAchievement(Achievements.TOTAL_DELIVERIES_1);
			// CompleteAchievement(Achievements.TOTAL_DELIVERIES_2);
			// CompleteAchievement(Achievements.TOTAL_DELIVERIES_3);
			// CompleteAchievement(Achievements.HAVING_MULTIPLE_DELIVERIES_1);
			// CompleteAchievement(Achievements.HAVING_MULTIPLE_DELIVERIES_2);
			// CompleteAchievement(Achievements.HAVING_MULTIPLE_DELIVERIES_3);
			// CompleteAchievement(Achievements.MAKING_MULTIPLE_DELIVERIES_1);
			// CompleteAchievement(Achievements.MAKING_MULTIPLE_DELIVERIES_2);
			// CompleteAchievement(Achievements.MAKING_MULTIPLE_DELIVERIES_3);
			// CompleteAchievement(Achievements.TOTAL_DIFFERENT_ITEMS_1);
			// CompleteAchievement(Achievements.TOTAL_DIFFERENT_ITEMS_2);
			// CompleteAchievement(Achievements.TOTAL_DIFFERENT_ITEMS_3);
			// CompleteAchievement(Achievements.TOTAL_FAILS_1);
			// CompleteAchievement(Achievements.TOTAL_FAILS_2);
			// CompleteAchievement(Achievements.TOTAL_FAILS_3);
			// CompleteAchievement(Achievements.TOTAL_ITEMS_1);
			// CompleteAchievement(Achievements.TOTAL_ITEMS_2);
			// CompleteAchievement(Achievements.TOTAL_ITEMS_3);
			// GameOver();

			GetTree().Paused = true;
		}
		if (@event.IsActionPressed("esc"))
		{
			GetTree().Quit();
		}
		if (@event.IsActionPressed("achievement_menu"))
		{
			AchievementsContainer.Visible = !AchievementsContainer.Visible;
		}
	}

	private void CreateNewPlanetAtLocation(Location Parent, string PlanetType, string PlanetPreset)
	{
		// switch (PlanetType)
		// {
		// 	case "Asteroids":
		// 		break;
		// 	case "BlackHole":
		// 		break;
		// 	case "DryTerran":
		// 		break;
		// 	case "Galaxy":
		// 		break;
		// 	case "GasPlanet":
		// 		break;
		// 	case "GasPlanetLayers":
		// 		break;
		// 	case "IceWorld":
		// 		break;
		// 	case "LandMasses":
		// 		break;
		// 	case "LavaWorld":
		// 		break;
		// 	case "NoAtmosphere":
		// 		break;
		// 	case "Rivers":
		// 		break;
		// 	case "Star":
		// 		break;
		// }
		// GD.PrintErr("Tried to create planet with type: ", PlanetType, " and preset: ", PlanetPreset, " at location: ", Parent.LocationName);
		Planet NewPlanet = PlanetScenes[PlanetType].Instantiate<Planet>();
		Parent.Visuals.AddChild(NewPlanet);
		Parent.Visuals.MoveChild(NewPlanet, Parent.Visuals.GetChildCount() - 3);
		NewPlanet.Position = new Vector2(-50, -50);
		NewPlanet.SetPixels(100);
		NewPlanet.SetSeed(PlanetSeeds[PlanetPreset]);
		float RandomRotation = Rnd.Next(-50, 50) / 100.0f;
		NewPlanet.SetRotation(-0.75f + RandomRotation);
		NewPlanet.SetColors(PlanetColorPresets[PlanetPreset]);
		switch (PlanetType)
		{
			case "BlackHole":
				NewPlanet.RelativeScale = 2f;
				NewPlanet.GUIZoom = 2f;
				break;
			case "Galaxy":
				NewPlanet.GUIZoom = 2.5f;
				break;
			case "GasPlanetLayers":
				NewPlanet.RelativeScale = 3f;
				NewPlanet.GUIZoom = 2.5f;
				NewPlanet.SetRotation(-1.2f);
				break;
			case "Star":
				NewPlanet.RelativeScale = 2f;
				NewPlanet.GUIZoom = 2f;
				NewPlanet.SetRotation(2.0f + RandomRotation);
				break;
		}
		if (Parent.PlanetType == "") Parent.PlanetType = PlanetType;
		if (Parent.PlanetPreset == "") Parent.PlanetPreset = PlanetPreset;
	}

	private Planet CreateNewPlanet(Node Parent, string PlanetType, string PlanetPreset, int pixels)
	{
		// GD.PrintErr("Tried to create planet with type: ", PlanetType, " and preset: ", PlanetPreset);
		Planet NewPlanet = PlanetScenes[PlanetType].Instantiate<Planet>();
		Parent.AddChild(NewPlanet);
		NewPlanet.Position = new Vector2(30, 22) + new Vector2(-pixels/2, -pixels/2);
		NewPlanet.SetPixels(pixels);
		NewPlanet.SetSeed(PlanetSeeds[PlanetPreset]);
		float RandomRotation = Rnd.Next(-pixels/2, pixels/2) / 100.0f;
		NewPlanet.SetRotation(-0.75f + RandomRotation);
		NewPlanet.SetColors(PlanetColorPresets[PlanetPreset]);
		switch (PlanetType)
		{
			case "BlackHole":
				NewPlanet.RelativeScale = 2f;
				NewPlanet.GUIZoom = 2f;
				break;
			case "Galaxy":
				NewPlanet.GUIZoom = 2.5f;
				break;
			case "GasPlanetLayers":
				NewPlanet.RelativeScale = 3f;
				NewPlanet.GUIZoom = 2.5f;
				NewPlanet.SetRotation(-1.2f);
				break;
			case "Star":
				NewPlanet.RelativeScale = 2f;
				NewPlanet.GUIZoom = 2f;
				NewPlanet.SetRotation(2.0f + RandomRotation);
				break;
		}
		return NewPlanet;
	}

	private void OnAsteroidTimerTimeout()
	{
		CreateNewAsteroid(
			Rnd.Next(20, 80),																// pixels
			Rnd.NextSingle() * 2.0f - 1.0f,													// rotation speed
			new Vector2(Rnd.Next(300, 1620), Rnd.Next(300, 780)),							// initial position
			new Vector2(Rnd.NextSingle() * 2.0f - 1.0f, Rnd.NextSingle() * 2.0f - 1.0f));	// velocity
	}

	private void CreateNewAsteroid(int pixels, float rotationSpeed, Vector2 initialPosition, Vector2 velocity)
	{
		Asteroid newAsteroid = PlanetScenes["Asteroids"].Instantiate<Asteroid>();
		newAsteroid.SetPhysicsProcess(false);
		AsteroidContainer.AddChild(newAsteroid);
		newAsteroid.Position = initialPosition;
		newAsteroid.SetSeed((uint)Rnd.Next());
		newAsteroid.SetPixels(pixels);
		newAsteroid.SetMovementData(rotationSpeed, velocity);
	}

	// private void ChangeConnection(int idFrom, int idTo, int distance, string color = "2ba69a", int segment_margin = 4, int line_width = 3)
	// {
	// 	Dictionary<string, string> newData = new() {{"color", color}, {"distance", distance.ToString()}, {"segment_margin", segment_margin.ToString()}, {"line_width", line_width.ToString()}};
	// 	foreach (KeyValuePair<int[], Dictionary<string, string>> Con in DrawingConnections)
	// 	{
	// 		if (Con.Key.Contains(idFrom) && Con.Key.Contains(idTo)) DrawingConnections[Con.Key] = newData;
	// 	}
	// }

	// private void HighlightSingleConnection(int idFrom, int idTo)
	// {
	// 	foreach (KeyValuePair<int[], Dictionary<string, string>> Con in DrawingConnections)
	// 	{
	// 		if (Con.Key.Contains(idFrom) && Con.Key.Contains(idTo))
	// 		{
	// 			DrawingConnections[Con.Key]["color"] = "2c9eca";
	// 		}
	// 	}
	// }

	// private void StopHighlightSingleConnection(int idFrom, int idTo)
	// {
	// 	foreach (KeyValuePair<int[], Dictionary<string, string>> Con in DrawingConnections)
	// 	{
	// 		if (Con.Key.Contains(idFrom) && Con.Key.Contains(idTo))
	// 		{
	// 			DrawingConnections[Con.Key]["color"] = "2ba69a";
	// 		}
	// 	}
	// }

	private void HighlightPath(int idFrom, int idTo, Color color)
	{
		Dictionary<int[], Dictionary<string, string>> newPath = new();
		// long[] pathID = StarMap.GetIdPath(idFrom, idTo);
		// Godot.Vector2[] pathPoint = StarMap.GetPointPath(idFrom, idTo);
		// for (int i = 0; i < pathID.Length - 1; i++)
		// {
		// 	ChangeConnection((int)pathID[i], (int)pathID[i+1], Mathf.FloorToInt(Math.Abs(pathPoint[i].DistanceTo(pathPoint[i+1]))), color, 3, 4);
		// }
		long[] pathID = StarMap.GetIdPath(idFrom, idTo);
		for (int i = 0; i < pathID.Length - 1; i++)
		{
			int a = (int)pathID[i];
			int b = (int)pathID[i+1];
			foreach (KeyValuePair<int[], Dictionary<string, string>> con in DrawingConnections)
			{
				if (con.Key.Contains(a) && con.Key.Contains(b))
				{
					newPath[con.Key] = con.Value.ToDictionary(entry => entry.Key, entry => entry.Value);
					newPath[con.Key]["color"] = color.ToHtml();
					newPath[con.Key]["line_width"] = "4";
					break;
				}
			}
		}
		PathCounter = 0;
		while (ReadableConnections.ContainsKey(PathCounter))
		{
			PathCounter++;
		}
		DrawingPaths[PathCounter] = newPath;
		ReadableConnections[PathCounter] = new int[] {idFrom, idTo};
	}

	private void StopHighlightPath(int idFrom, int idTo)
	{
		// long[] pathID = StarMap.GetIdPath(idFrom, idTo);
		// Godot.Vector2[] pathPoint = StarMap.GetPointPath(idFrom, idTo);
		// for (int i = 0; i < pathID.Length - 1; i++)
		// {
		// 	ChangeConnection((int)pathID[i], (int)pathID[i+1], Mathf.FloorToInt(Math.Abs(pathPoint[i].DistanceTo(pathPoint[i+1]))));
		// }
		long target = -1;
		foreach (KeyValuePair<long, int[]> Con in ReadableConnections)
		{
			if (Con.Value.Contains(idFrom) && Con.Value.Contains(idTo))
			{
				target = Con.Key;
				break;
			}
		}
		if (target > -1)
		{
			DrawingPaths.Remove(target);
			ReadableConnections.Remove(target);
		}
	}

	public override void _Draw()
	{
		//int distance, string color, int segment_margin, int line_width
		// foreach (KeyValuePair<int[], Dictionary<string, string>> Con in DrawingConnections)
		// {
		// 	Godot.Vector2 From = AllLocations[Con.Key[0]].Position;
		// 	Godot.Vector2 To = AllLocations[Con.Key[1]].Position;
		// 	DrawSegmentedLine(From, To, new Color(Con.Value["color"]), int.Parse(Con.Value["distance"]), int.Parse(Con.Value["segment_margin"]), int.Parse(Con.Value["line_width"]));
		// }
		foreach (KeyValuePair<int[], Dictionary<string, string>> Con in DrawingConnections)
		{
			Control From = AllLocations[Con.Key[0]].Visuals;
			Control To = AllLocations[Con.Key[1]].Visuals;
			DrawConnection(From, To, Con.Value);
		}
		foreach (KeyValuePair<long, Dictionary<int[], Dictionary<string, string>>> path in DrawingPaths)
		{
			foreach (KeyValuePair<int[], Dictionary<string, string>> Con in path.Value)
			{
				Control From = AllLocations[Con.Key[0]].Visuals;
				Control To = AllLocations[Con.Key[1]].Visuals;
				DrawConnection(From, To, Con.Value);
			}
		}
	}

	private void DrawConnection(Control From, Control To, Dictionary<string, string> Data)
	{
		int distance = int.Parse(Data["distance"]);
		Color color = new(Data["color"]);
		int width = int.Parse(Data["line_width"]);
		Vector2 Diff = (From.GlobalPosition - To.GlobalPosition).Normalized() * 70;
		DrawLine(From.GlobalPosition - Diff, To.GlobalPosition + Diff, color, width);
		for (int i = 0; i < distance; i++)
		{
			DrawTriangle(From, To, color, width, i);
			DrawTriangle(To, From, color, width, i);
		}
	}

	private void DrawTriangle(Control From, Control To, Color color, int width, int Offset = 0)
	{
		Vector2 Direction = (From.GlobalPosition - To.GlobalPosition).Normalized();
		Vector2 Base = Direction * 70;
		Vector2 p1 = From.GlobalPosition - Base * 1.25f - Offset * Direction * 17.5f;
		Vector2 p2 = From.GlobalPosition - Base - Offset * Direction * 17.5f + new Vector2(Base.Y, -Base.X) * 0.075f * Mathf.Clamp(width, 1, 2);
		Vector2 p3 = From.GlobalPosition - Base - Offset * Direction * 17.5f + new Vector2(-Base.Y, Base.X) * 0.075f * Mathf.Clamp(width, 1, 2);
		DrawPolygon(new Vector2[] {p1, p2, p3}, new Color[] {color, color, color});
	}

	private void OnDeliveryHoverStart(int deliveryId)
	{
		HighlightPath(CurrentLocation.ID, AcceptedQuests[deliveryId].Destination.ID, AcceptedQuests[deliveryId].Destination.LocationColor);
	}

	private void OnDeliveryHoverFinish(int deliveryId)
	{
		StopHighlightPath(CurrentLocation.ID, AcceptedQuests[deliveryId].Destination.ID);
	}

	private void OnSegmentDone(int deliveryId, int middleManId)
	{
		AllLocations[middleManId].ClearHighlightSegment();
	}

	private void DrawSegmentedLine(Vector2 From, Vector2 To, Color LineColor, int Segments = 1, int SegmentMargin = 1, int LineWidth = 1)
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

	private Quest CreateNewQuest(int NumOfItems = 0, int newQuestTier = -1)
	{
		NumOfItems = Math.Clamp(NumOfItems, 0, 5);
		string[] PossibleItems = Array.Empty<string>();
		foreach (ItemData itemData in DeliveryItems) PossibleItems = PossibleItems.Append(itemData.ItemName).ToArray();
		Quest NewQuest = new();
		int i = 0;
		ItemData[] QuestItems = Array.Empty<ItemData>();

		if (NumOfItems == 0) NumOfItems = Rnd.Next(3, 6);

		int QuestTier = Rnd.Next(3);
		if (newQuestTier > -1) QuestTier = Math.Clamp(newQuestTier, 0, 2);
		int[][] ItemsTiers = new int[][] {new int[] {6, 7, 8, 9, 10}, new int[] {3, 4, 5, 6, 7}, new int[] {1, 2, 3, 4, 5}};

		while (i < NumOfItems)
		{
			ItemData NewItem = DeliveryItems[Rnd.Next(DeliveryItems.Length)];
			if (!QuestItems.Contains(NewItem) & ItemsTiers[QuestTier].Contains(NewItem.Fragility))
			{
				QuestItems = QuestItems.Append(NewItem).ToArray();
				i++;
			}
		}
		NewQuest.Items = QuestItems;

		int[][] DistanceTiers = new int[][] {new int[] {2, 3, 4}, new int[] {3, 4, 5}, new int[] {5, 6, 7}};

		Location[] PossibleLocations = Array.Empty<Location>();
		// Choosing a Location that is not a Current Location, is far enough and isn't disabled
		foreach (Location location in AllLocations.Values)
		{
			if
			(
				location.LocationName != CurrentLocation.LocationName
				&&
				DistanceTiers[QuestTier].Contains(StarMap.GetIdPath(CurrentLocation.ID, location.ID).Length - 1)
				&&
				!location.Hazards.Contains("Shutdown")
			)
			PossibleLocations = PossibleLocations.Append(location).ToArray();
		}
		if (PossibleLocations.Length == 0)
		foreach (Location location in AllLocations.Values)
		{
			if
			(
				location.LocationName != CurrentLocation.LocationName
				&&
				DistanceTiers[QuestTier].Contains(StarMap.GetIdPath(CurrentLocation.ID, location.ID).Length)
				&&
				!location.Hazards.Contains("Shutdown")
			)
			PossibleLocations = PossibleLocations.Append(location).ToArray();
		}
		Location ChosenLocation = PossibleLocations[Rnd.Next(PossibleLocations.Length)];

		// Adding a modifier
		switch (Rnd.Next(5))
		{
			case 0:
				string newTag = "";
				Dictionary<string, string> newTagData = new();
				switch (Rnd.Next(3))
				{
					case 0:
						newTag = "Timed";
						newTagData["tier"] = (Rnd.Next(3) + 1).ToString();
						newTagData["texture_path"] = "res://UI/Icons/Modifiers/Timed.svg";
						break;
					case 1:
						newTag = "Fragile";
						newTagData["jumps"] = (StarMap.GetIdPath(CurrentLocation.ID, ChosenLocation.ID).Length + Rnd.Next(2)).ToString();
						newTagData["texture_path"] = "res://UI/Icons/Modifiers/Fragile.svg";
						break;
					case 2:
						newTag = "Segmented";
						newTagData["middle-man-id"] = StarMap.GetIdPath(CurrentLocation.ID, ChosenLocation.ID)[Rnd.Next(1, StarMap.GetIdPath(CurrentLocation.ID, ChosenLocation.ID).Length-1)].ToString();
						newTagData["middle-man-name"] = AllLocations[int.Parse(newTagData["middle-man-id"])].LocationName;
						newTagData["middle-man-met"] = "false";
						newTagData["texture_path"] = "res://UI/Icons/Modifiers/Segmented.svg";
						break;
				}
				NewQuest.AddTag(newTag, newTagData);
				break;
			case 1:
				NewQuest.AddTag("Timed", new Dictionary<string, string>() { {"tier", (Rnd.Next(3) + 1).ToString()}, {"texture_path", "res://UI/Icons/Modifiers/Timed.svg"} });
				NewQuest.AddTag("Fragile", new Dictionary<string, string>() { {"jumps", (StarMap.GetIdPath(CurrentLocation.ID, ChosenLocation.ID).Length + Rnd.Next(2)).ToString()},
					{"texture_path", "res://UI/Icons/Modifiers/Fragile.svg"} });
				// GD.Print("COMBO");
				break;
			case 2:
				NewQuest.AddTag("Timed", new Dictionary<string, string>() { {"tier", (Rnd.Next(3) + 1).ToString()}, {"texture_path", "res://UI/Icons/Modifiers/Timed.svg"} });
				string temp1 = StarMap.GetIdPath(CurrentLocation.ID, ChosenLocation.ID)[Rnd.Next(1, StarMap.GetIdPath(CurrentLocation.ID, ChosenLocation.ID).Length-1)].ToString();
				NewQuest.AddTag("Segmented", new Dictionary<string, string>() {
					{"middle-man-id", temp1},
					{"middle-man-name", AllLocations[int.Parse(temp1)].LocationName},
					{"middle-man-met", "false"},
					{"texture_path", "res://UI/Icons/Modifiers/Segmented.svg"} });
				break;
		}
		NewQuest.Destination = ChosenLocation;
		NewQuest.ID = QuestCounter;
		return NewQuest;
	}

	private void OnLocationPressed(int pressedID)
	{
		LastPressedLocation = AllLocations[pressedID];
		if (CurrentLocation != LastPressedLocation && StarMap.ArePointsConnected(CurrentLocation.ID, LastPressedLocation.ID) && !StarMap.IsPointDisabled(pressedID))
		{
			LastJumpDistance = GetJumpDistance(CurrentLocation, LastPressedLocation);
			if (CurrentLocation.Hazards.Contains("Disentery")) LastJumpDistance *= int.Parse(CurrentLocation.GetHazardData("Disentery")["jumpCost"]);
			if (LastJumpDistance <= FuelLevel && Spaceship.IsAvailable())
			{
				MoveShip(LastPressedLocation);
			}
		}
	}

	private void MoveShip(Location ToLocation)
	{
		Spaceship.SetMoveTarget(ToLocation.Position, .5f);
	}

	private void OnShipArrived()
	{
		if (CurrentLocation.Hazards.Contains("Disentery")) CurrentLocation.RemoveHazard("Disentery");
		FuelLevel -= LastJumpDistance;
		CreateNewNPC();
		if (LastPressedLocation.Hazards.Contains("Faulty"))
		{
			UpdateDeliveries(LastJumpDistance * 2);
			LastPressedLocation.RemoveHazard("Faulty");
		}
		else UpdateDeliveries(LastJumpDistance);
		StopHighlightPath(CurrentLocation.ID, DisplayedQuest.Destination.ID);
		CurrentLocation = LastPressedLocation;
		DisplayQuest(CreateNewQuest(Rnd.Next(11)));
		CurrentLocationLabel.Text = CurrentLocation.LocationName;
		int[] CompletedQuestIDs = CurrentLocation.GetQuests();
		ACH_CompletedQuests = 0;
		ACH_TotalFails = 0;
		// Getting the payout for completing deliveries
		foreach (int id in CompletedQuestIDs)
		{
			Delivery delivery = GetDeliveryByID(id);
			if (!(delivery.HasTag("Segmented") && delivery.GetTagData("Segmented")["middle-man-met"] == "false"))
			{
				Deliveries = Deliveries.Where(val => val != delivery).ToArray();
				delivery.OnDeliveryFailed -= DeliveryFailed;
				delivery.OnDeliveryMouseEntered -= OnDeliveryHoverStart;
				delivery.OnDeliveryMouseExited -= OnDeliveryHoverFinish;
				delivery.OnSegmentCompleted -= OnSegmentDone;
				AcceptedQuests.Remove(id);
				CurrentLocation.RemoveQuest(id);
				int Payout = 0;
				Item[] items = delivery.GetItems();
				foreach (Item item in items) Payout += (int)Mathf.Floor(item.Fragility * 10 * item.HP / 100);
				if (delivery.HasTag("Timed")) Payout = Mathf.FloorToInt(Payout * (1f + .15f * int.Parse(delivery.GetTagData("Timed")["tier"])));
				if (delivery.HasTag("Fragile")) Payout = Mathf.FloorToInt(Payout * 1.2f);
				if (delivery.HasTag("Segmented")) Payout = Mathf.FloorToInt(Payout * 1.2f);
				Payout = Mathf.FloorToInt(Payout * (1.0f + delivery.TotalDistance / 10.0f));
				PopupBalance(Payout);
				Balance += Payout;
				ACH_TotalDeliveries++;
				ACH_TotalItems += delivery.GetItemsSurvivedNum();
				foreach (Item item in delivery.GetItemsSurvived()) ACH_TotalDifferentItems[item.ItemName]++;
				delivery.QueueFree();
				ACH_CompletedQuests++;
			}
		}
		if (CompletedQuestIDs.Length > 0) CurrentLocation.SetCompleteQuest();
		foreach (Delivery delivery in Deliveries) delivery.Jump(CurrentLocation.ID);
		CheckAchievements();
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
				// delivery.AddTag("Timed", newTagData, GetNewModifierIcon("Timed", newTagData));
				delivery.AddTag("Timed", newTagData);
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
				// delivery.AddTag("Fragile", newTagData, GetNewModifierIcon("Fragile", newTagData));
				delivery.AddTag("Fragile", newTagData);
			}
			CurrentLocation.RemoveHazard("Bacteria");
		}
		if (LeapDay)
		{
			LeapDay = false;
			foreach (Location location in AllLocations.Values) location.AddFuel(1);
		}
		else LeapDay = true;
		foreach (Location location in AllLocations.Values) location.Jump();
		foreach (long i in StarMap.GetPointConnections(CurrentLocation.ID))
		{
			LastJumpDistance = GetJumpDistance(CurrentLocation, AllLocations[(int)i]);
			if (CurrentLocation.Hazards.Contains("Disentery")) LastJumpDistance *= int.Parse(CurrentLocation.GetHazardData("Disentery")["jumpCost"]);
			if (FuelLevel >= LastJumpDistance) return;
		}
		if (CurrentLocation.Hazards.Contains("MarketCrash"))
		{
			if (Balance >= 50 * int.Parse(CurrentLocation.GetHazardData("MarketCrash")["fuelCost"])) return;
		}
		else if (Balance >= 50 && CurrentLocation.GetFuelLevel() > 0) return;
		GameOver();
	}

	private void OnLocationAvailable(int locationID)
	{
		StarMap.SetPointDisabled(locationID, false);
	}

	private void CreateNewNPC()
	{
		NPCContainer.Modulate = new Color(Mathf.Clamp(Rnd.NextSingle(), 0.3f, 0.9f), Mathf.Clamp(Rnd.NextSingle(), 0.3f, 0.9f), Mathf.Clamp(Rnd.NextSingle(), 0.3f, 0.9f));
		NPCNameLabel.Text = string.Format("{0} {1}", NPCFirstNames[Rnd.Next(NPCFirstNames.Length)], NPCLastNames[Rnd.Next(NPCLastNames.Length)]);
		NPCBodySprite.RegionRect = new Rect2(new Vector2(Rnd.Next(10)*48, 0), new Vector2(48, 48));
		NPCFaceSprite.RegionRect = new Rect2(new Vector2(Rnd.Next(10)*32, 0), new Vector2(32, 32));
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
		if (DisplayedQuest is not null)
		{
			ClearQuestHighlight();
			StopHighlightPath(CurrentLocation.ID, DisplayedQuest.Destination.ID);
		}
		foreach (Node child in DestinationPlanet.GetChildren()) child.QueueFree();
		DisplayedQuest?.QueueFree();
		DeliveryContentsLabel.Clear();
		foreach (Node child in ModifiersHBox.GetChildren()) child.QueueFree();
		EnableQuestButtons();
		DisplayedQuest = NewQuest;
		Location DeliveryLocation = NewQuest.Destination;
		DeliveryLocation.Highlight();
		DestinationLabel.Text = DeliveryLocation.LocationName;
		DeliveryContentsLabel.AddText("Hello! I want you to deliver these to station " + DeliveryLocation.LocationName + ":\n\n");
		for (int i = 0; i < NewQuest.Items.Length; i++)
		{
			ItemData item = NewQuest.Items[i];
			DeliveryContentsLabel.AddText(string.Format("\t{0}: {1}\n", i + 1, item.ItemName));
		}
		DeliveryContentsLabel.AddText("\nGood Luck!");
		for (int i = 0; i < NewQuest.Tags.Length; i++)
		{
			string tag = NewQuest.Tags[i];
			Dictionary<string, string> tagData = NewQuest.TagsData[i];
			ModifierIcon newMod = GetNewModifierIcon(tag, tagData);
			ModifiersHBox.AddChild(newMod);
			if (tag == "Segmented")
			{
				Location middleManLocation = AllLocations[int.Parse(tagData["middle-man-id"])];
				Planet newPlanet = CreateNewPlanet(newMod, middleManLocation.PlanetType, middleManLocation.PlanetPreset, 20);
				newMod.SetPlanet(newPlanet);
				newMod.HideData();
			}
		}
		CreateNewPlanet(DestinationPlanet, DeliveryLocation.PlanetType, DeliveryLocation.PlanetPreset, 40);
		HighlightPath(CurrentLocation.ID, DeliveryLocation.ID, DeliveryLocation.LocationColor);
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
		// Adding a delivery in the cargo hold
		DeliveriesContainer.AddChild(NewDelivery);
		// CargoVBox.AddChild(NewDelivery);
		GodotArray[] Result = Array.Empty<GodotArray>();
		NewDelivery.SetPlanet(CreateNewPlanet(NewDelivery, DisplayedQuest.Destination.PlanetType, DisplayedQuest.Destination.PlanetPreset, 25), DisplayedQuest.Destination.LocationColor);
		GD.Print("Creating new Delivery with ", DisplayedQuest.Tags.Length, " tags...");
		foreach (int idx in Enumerable.Range(0, DisplayedQuest.Tags.Length))
		{
			string tag = DisplayedQuest.Tags[idx];
			Dictionary<string, string> tagData = DisplayedQuest.TagsData[idx];
			// NewDelivery.AddTag(tag, tagData, GetNewModifierIcon(tag, tagData));
			NewDelivery.AddTag(tag, tagData);
			if (tag == "Segmented")
			{
				Location segmentDestination = AllLocations[int.Parse(tagData["middle-man-id"])];
				NewDelivery.SetSegmentPlanet(CreateNewPlanet(NewDelivery, segmentDestination.PlanetType, segmentDestination.PlanetPreset, 20));
				segmentDestination.HighlightSegment(DisplayedQuest.Destination.LocationColor);
			}
		}
		if (DisplayedQuest.Tags.Length == 0)
		{
			NewDelivery.SetEmpty();
		}
		// The third parameter is "newDistance", which gets used in payout calculation
		NewDelivery.SetItems(DisplayedQuest.ID, DisplayedQuest.Items, StarMap.GetIdPath(CurrentLocation.ID, DisplayedQuest.Destination.ID).Length);
		NewDelivery.OnDeliveryFailed += DeliveryFailed;
		NewDelivery.OnDeliveryMouseEntered += OnDeliveryHoverStart;
		NewDelivery.OnDeliveryMouseExited += OnDeliveryHoverFinish;
		NewDelivery.OnSegmentCompleted += OnSegmentDone;
		Deliveries = Deliveries.Append(NewDelivery).ToArray();
		CheckAchievements();
	}

	private void UpdateDeliveries(int Distance) { foreach (Delivery delivery in Deliveries) delivery.Damage(Distance); }

	private void DeliveryFailed(int FailedDeliveryID, string FailReason)
	{
		Delivery FailedDelivery = new();
		foreach (Delivery delivery in Deliveries) if (FailedDeliveryID == delivery.ID) { FailedDelivery = delivery; break; }
		Deliveries = Deliveries.Where(v => v != FailedDelivery).ToArray();
		FailedDelivery.OnDeliveryFailed -= DeliveryFailed;
		FailedDelivery.OnDeliveryMouseEntered -= OnDeliveryHoverStart;
		FailedDelivery.OnDeliveryMouseExited -= OnDeliveryHoverFinish;
		FailedDelivery.OnSegmentCompleted -= OnSegmentDone;
		if (FailedDelivery.HasTag("Segmented"))
		{
			AllLocations[int.Parse(FailedDelivery.GetTagData("Segmented")["middle-man-id"])].ClearHighlightSegment();
		}
		AcceptedQuests.Remove(FailedDeliveryID);
		foreach (Location location in AllLocations.Values) if (location.GetQuests().Contains(FailedDeliveryID)) { location.RemoveQuest(FailedDeliveryID); break; }
		int Payout = 0;
		Item[] items = FailedDelivery.GetItems();
		foreach (Item item in items) Payout += item.Fragility * 10;
		Payout = Mathf.FloorToInt(Payout * (1.0f + FailedDelivery.TotalDistance / 10.0f));
		PopupBalance(-Payout);
		Balance -= Payout;
		FailedDelivery.QueueFree();
		GD.Print(string.Format("Delivery [{0}] failed!\n\tReason: {1}", FailedDeliveryID, FailReason));
		ACH_TotalFails++;
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
				PopupBalance(-Cost);
				foreach (Location location in AllLocations.Values) location.MarketCrashUpdate();
			}
		}
	}

	private void ClearQuestHighlight() { DisplayedQuest.Destination.ClearHighlight(); }

	// private void HighlightNeighbours()
	// {
	// 	long[] Connections = StarMap.GetPointConnections(CurrentLocation.ID);
	// 	foreach (long id in Connections)
	// 	{
	// 		// AllLocations[(int)id].Choosable();
	// 		HighlightSingleConnection(CurrentLocation.ID, (int)id);
	// 	}
	// }

	// private void ClearHighlightNeighbours()
	// {
	// 	long[] Connections = StarMap.GetPointConnections(CurrentLocation.ID);
	// 	foreach (long id in Connections)
	// 	{
	// 		// AllLocations[(int)id].ClearChoosable();
	// 		StopHighlightSingleConnection(CurrentLocation.ID, (int)id);
	// 	}
	// }

	private void PopupBalance(int Difference)
	{
		Label PopupLabel = new();
		if (Difference > 0) { PopupLabel.Text = "$" + Difference.ToString(); PopupLabel.Modulate = Colors.Green; }
		else { PopupLabel.Text = Difference.ToString().Insert(1, "$"); PopupLabel.Modulate = Colors.Red; }
		AddChild(PopupLabel);
		PopupLabel.Position = new Godot.Vector2(MoneyLabel.GlobalPosition.X + Rnd.Next(-100, 20), MoneyLabel.GlobalPosition.Y - 32);
		Tween LabelTween = GetTree().CreateTween();
		LabelTween.TweenProperty(PopupLabel, "position:y", MoneyLabel.GlobalPosition.Y - (float)Rnd.NextDouble() * 5.0f - 40.0f, 1.0d);
		LabelTween.TweenProperty(PopupLabel, "modulate", Colors.Transparent, 0.3d).SetDelay(0.7d);
		LabelTween.TweenCallback(Callable.From(PopupLabel.QueueFree)).SetDelay(1.0d);
	}

	// private void OnAchievementsButtonPressed()
	// {
	// 	if (AchievementsMenu.Visible) AchievementsMenu.Hide();
	// 	else AchievementsMenu.Show();
	// }

	private void CheckAchievements()
	{
		if (ACH_CompletedQuests > 9) CompleteAchievement(Achievements.MAKING_MULTIPLE_DELIVERIES_3);
		else if (ACH_CompletedQuests > 4) CompleteAchievement(Achievements.MAKING_MULTIPLE_DELIVERIES_2);
		else if (ACH_CompletedQuests > 2) CompleteAchievement(Achievements.MAKING_MULTIPLE_DELIVERIES_1);

		if (Deliveries.Length > 9) CompleteAchievement(Achievements.HAVING_MULTIPLE_DELIVERIES_3);
		else if (Deliveries.Length > 4) CompleteAchievement(Achievements.HAVING_MULTIPLE_DELIVERIES_2);
		else if (Deliveries.Length > 2) CompleteAchievement(Achievements.HAVING_MULTIPLE_DELIVERIES_1);

		if (ACH_TotalFails > 4) CompleteAchievement(Achievements.TOTAL_FAILS_3);
		else if (ACH_TotalFails > 2) CompleteAchievement(Achievements.TOTAL_FAILS_2);
		else if (ACH_TotalFails > 1) CompleteAchievement(Achievements.TOTAL_FAILS_1);

		int tier = 3;
		foreach (KeyValuePair<string, int> item in ACH_TotalDifferentItems) if (item.Value < 15) { tier--; break; }
		foreach (KeyValuePair<string, int> item in ACH_TotalDifferentItems) if (item.Value < 10) { tier--; break; }
		foreach (KeyValuePair<string, int> item in ACH_TotalDifferentItems) if (item.Value < 5) { tier--; break; }
		switch (tier)
		{
			case 1:
				CompleteAchievement(Achievements.TOTAL_DIFFERENT_ITEMS_1);
				break;
			case 2:
				CompleteAchievement(Achievements.TOTAL_DIFFERENT_ITEMS_2);
				break;
			case 3:
				CompleteAchievement(Achievements.TOTAL_DIFFERENT_ITEMS_3);
				break;
		}

		if (ACH_TotalItems > 99) CompleteAchievement(Achievements.TOTAL_ITEMS_3);
		else if (ACH_TotalItems > 49) CompleteAchievement(Achievements.TOTAL_ITEMS_2);
		else if (ACH_TotalItems > 19) CompleteAchievement(Achievements.TOTAL_ITEMS_1);

		if (ACH_TotalDeliveries > 49) CompleteAchievement(Achievements.TOTAL_DELIVERIES_3);
		else if (ACH_TotalDeliveries > 24) CompleteAchievement(Achievements.TOTAL_DELIVERIES_2);
		else if (ACH_TotalDeliveries > 14) CompleteAchievement(Achievements.TOTAL_DELIVERIES_1);
	}

	private void CompleteAchievement(Achievements achievement)
	{
		switch (achievement)
		{
			case Achievements.TOTAL_DELIVERIES_1:
				foreach (Achievement child in AchievementsVBoxContainer.GetChildren()) if (child.Description.StartsWith("Novice hauler")) { QueueAchievement(child); break; }
				break;
			case Achievements.TOTAL_DELIVERIES_2:
				foreach (Achievement child in AchievementsVBoxContainer.GetChildren()) if (child.Description.StartsWith("Advanced hauler")) { QueueAchievement(child); break; }
				break;
			case Achievements.TOTAL_DELIVERIES_3:
				foreach (Achievement child in AchievementsVBoxContainer.GetChildren()) if (child.Description.StartsWith("Master hauler")) { QueueAchievement(child); break; }
				break;
			case Achievements.TOTAL_ITEMS_1:
				foreach (Achievement child in AchievementsVBoxContainer.GetChildren()) if (child.Description.StartsWith("Small collector")) { QueueAchievement(child); break; }
				break;
			case Achievements.TOTAL_ITEMS_2:
				foreach (Achievement child in AchievementsVBoxContainer.GetChildren()) if (child.Description.StartsWith("Mediocre collector")) { QueueAchievement(child); break; }
				break;
			case Achievements.TOTAL_ITEMS_3:
				foreach (Achievement child in AchievementsVBoxContainer.GetChildren()) if (child.Description.StartsWith("Expert collector")) { QueueAchievement(child); break; }
				break;
			case Achievements.TOTAL_DIFFERENT_ITEMS_1:
				foreach (Achievement child in AchievementsVBoxContainer.GetChildren()) if (child.Description.StartsWith("Variety newbie")) { QueueAchievement(child); break; }
				break;
			case Achievements.TOTAL_DIFFERENT_ITEMS_2:
				foreach (Achievement child in AchievementsVBoxContainer.GetChildren()) if (child.Description.StartsWith("Variety enthusiast")) { QueueAchievement(child); break; }
				break;
			case Achievements.TOTAL_DIFFERENT_ITEMS_3:
				foreach (Achievement child in AchievementsVBoxContainer.GetChildren()) if (child.Description.StartsWith("Variety perfectionist")) { QueueAchievement(child); break; }
				break;
			case Achievements.TOTAL_FAILS_1:
				foreach (Achievement child in AchievementsVBoxContainer.GetChildren()) if (child.Description.StartsWith("It happens")) { QueueAchievement(child); break; }
				break;
			case Achievements.TOTAL_FAILS_2:
				foreach (Achievement child in AchievementsVBoxContainer.GetChildren()) if (child.Description.StartsWith("We learn from your mistakes")) { QueueAchievement(child); break; }
				break;
			case Achievements.TOTAL_FAILS_3:
				foreach (Achievement child in AchievementsVBoxContainer.GetChildren()) if (child.Description.StartsWith("Self induced suffering")) { QueueAchievement(child); break; }
				break;
			case Achievements.HAVING_MULTIPLE_DELIVERIES_1:
				foreach (Achievement child in AchievementsVBoxContainer.GetChildren()) if (child.Description.StartsWith("Beginner multitasker")) { QueueAchievement(child); break; }
				break;
			case Achievements.HAVING_MULTIPLE_DELIVERIES_2:
				foreach (Achievement child in AchievementsVBoxContainer.GetChildren()) if (child.Description.StartsWith("Multitasking addict")) { QueueAchievement(child); break; }
				break;
			case Achievements.HAVING_MULTIPLE_DELIVERIES_3:
				foreach (Achievement child in AchievementsVBoxContainer.GetChildren()) if (child.Description.StartsWith("Indian god")) { QueueAchievement(child); break; }
				break;
			case Achievements.MAKING_MULTIPLE_DELIVERIES_1:
				foreach (Achievement child in AchievementsVBoxContainer.GetChildren()) if (child.Description.StartsWith("Keeping promises")) { QueueAchievement(child); break; }
				break;
			case Achievements.MAKING_MULTIPLE_DELIVERIES_2:
				foreach (Achievement child in AchievementsVBoxContainer.GetChildren()) if (child.Description.StartsWith("Oathkeeper")) { QueueAchievement(child); break; }
				break;
			case Achievements.MAKING_MULTIPLE_DELIVERIES_3:
				foreach (Achievement child in AchievementsVBoxContainer.GetChildren()) if (child.Description.StartsWith("Sworn protector")) { QueueAchievement(child); break; }
				break;
		}
	}

	private void QueueAchievement(Achievement newAchievement)
	{
		if (!newAchievement.Enabled)
		{
			GD.Print("Queued achievement: ", newAchievement.Description[..newAchievement.Description.IndexOf('.')]);
			newAchievement.Enabled = true;
			AchievementQueue = AchievementQueue.Append(newAchievement).ToArray();
			if (!AchievementTween.IsRunning())
			{
				PlayNextAchievement();
				GD.Print("\tPlaying...");
			}
		}
	}

	private void PlayNextAchievement()
	{
		if (AchievementQueue.Length == 0)
		{
			CheckCompletion();
			GD.Print("Animation queue is clear");
			return;
		}
		AchievementTween = GetTree().CreateTween();
		AchievementTween.TweenProperty(AchievementRect, "modulate:a", 1.0f, 2.0d);
		AchievementTween.TweenProperty(AchievementRect, "modulate:a", 0.0f, 2.0d).SetDelay(2.0d);
		AchievementTween.Finished += PlayNextAchievement;
		if (AchievementQueue.Length == 1)
		{
			AchievementQueue = Array.Empty<Achievement>();
			GD.Print("Clearing animation queue...");
		}
		else
		{
			AchievementQueue = AchievementQueue[1..AchievementQueue.Length];
			GD.Print("Animation queue reduced by 1");
		}
	}

	private int GetLocationID(string NameOfLocation)
	{
		foreach (Location loc in AllLocations.Values) { if (loc.LocationName == NameOfLocation) { return loc.ID; } }
		GD.PrintErr("Could not find location with a name: ", NameOfLocation);
		return -1;
	}

	private int GetJumpDistance(Location From, Location To)
	{
		Vector2[] Path = StarMap.GetPointPath(From.ID, To.ID);
		if (Path.Length != 2)
		{
			GD.PrintErr("Path from ", From.LocationName, " to ", To.LocationName, " is ", Path.Length);
		}
		Vector2 Distance = Path[1] - Path[0];
		if (Distance.X == 0f) return (int)Mathf.Abs(Distance.Y);
		else return (int)Mathf.Abs(Distance.X);
	}

	private void CheckCompletion()
	{
		foreach (Achievement achievement in AchievementsVBoxContainer.GetChildren()) if (!achievement.Enabled) return;
		VictoryScreen.Show();
	}

	private void OnKeepPlayingButtonPressed()
	{
		VictoryScreen.Hide();
	}

	private void GameOver()
	{
		FailScreen.Show();
	}

	private void OnRetryButtonPressed()
	{
		GD.Print("Reload: " + GetTree().ReloadCurrentScene());
		// foreach (ItemData item in DeliveryItems) item.QueueFree();
		// foreach (KeyValuePair<int, Quest> item in AcceptedQuests) item.Value.QueueFree();
		// foreach (Delivery item in Deliveries)
		// {
		// 	item.OnDeliveryFailed -= DeliveryFailed;
		// 	item.OnDeliveryMouseEntered -= OnDeliveryHoverStart;
		// 	item.OnDeliveryMouseExited -= OnDeliveryHoverFinish;
		// 	item.QueueFree();
		// }
		// foreach (KeyValuePair<int, Location> item in AllLocations) 
		// {
		// 	item.Value.ClearHighlight();
		// 	string[] hazards = Array.Empty<string>();
		// 	// item.Value.Hazards.CopyTo(hazards, 0);
		// 	foreach (string hazard in item.Value.Hazards) item.Value.RemoveHazard(hazard);
		// 	item.Value._Ready();
		// }
		// foreach (Achievement item in AchievementsContainer.GetChildren()) item.Enabled = false;
		// _Ready();
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
		// NewModifier.SetSprite(TagFrames[newTag]);
		switch (newTag)
		{
			case "Timed":
				NewModifier.SetSprite(GD.Load<Texture2D>("res://UI/Icons/Modifiers/Timed.svg"));
				NewModifier.SetData((20 - int.Parse(newTagData["tier"]) * 5).ToString());
				NewModifier.TooltipText = "You better be fast!\nThis delivery has a timer on it.";
				break;
			case "Fragile":
				NewModifier.SetSprite(GD.Load<Texture2D>("res://UI/Icons/Modifiers/Fragile.svg"));
				NewModifier.SetData(newTagData["jumps"]);
				NewModifier.TooltipText = "These boxes seem more fragile than usual...\nEach jump is important, deliver the items in provided number of jumps.";
				break;
			case "Segmented":
				NewModifier.SetSprite(GD.Load<Texture2D>("res://UI/Icons/Modifiers/Segmented.svg"));
				NewModifier.SetData(newTagData["middle-man-name"]);
				NewModifier.TooltipText = "I like how certain planets influence my goods, here's the path...\nMake sure you go to the specified planet before completing the order.";
				break;
		}
		return NewModifier;
	}
}
