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
	
	private AStar2D StarMap;

	private Ship Spaceship;

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
	private AudioStreamPlayer AcceptButtonSound;
	private Button FuelButton;
	private AudioStreamPlayer FuelButtonSound;

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
	private Label FuelPriceLabel;

	private HBoxContainer ModifiersHBox;
	private PackedScene ModifierIconScene;

	private SpriteFrames TimedFrames;
	private SpriteFrames FragileFrames;
	private SpriteFrames SegmentedFrames;

	private int HazardCounter;
	private Texture2D MarketCrashIcon;
	private Texture2D DisenteryIcon;
	private Texture2D RumIcon;
	private Texture2D HecticIcon;
	private Texture2D BacteriaIcon;
	private Texture2D ShutdownIcon;
	private Texture2D FaultyIcon;

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

	private Control VictoryScreen;
	private Control FailScreen;

	private Tween AchievementTween;

	private bool LeapDay = false;

	private Dictionary<string, PackedScene> PlanetScenes;
	private Dictionary<string, Color[]> PlanetColorPresets;
	private Dictionary<string, uint> PlanetSeeds;

	private Node2D AsteroidContainer;

	private Location LastPressedLocation;
	private int LastJumpDistance;

	private AudioStreamPlayer LocationPickSound;
	private AudioStreamPlayer LocationArrivalSound;

	private bool IsQuestAccepted;

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

		StarMap.AddPoint(l1.ID, Vector2.Zero);
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

		string conColor = "505050";

		DrawingConnections[new [] {0, 1}] = new Dictionary<string, string> {{"distance", "3"}, {"color", conColor}, {"line_width", "1"}};
		DrawingConnections[new [] {1, 2}] = new Dictionary<string, string> {{"distance", "1"}, {"color", conColor}, {"line_width", "1"}};
		DrawingConnections[new [] {2, 3}] = new Dictionary<string, string> {{"distance", "2"}, {"color", conColor}, {"line_width", "1"}};
		DrawingConnections[new [] {3, 4}] = new Dictionary<string, string> {{"distance", "1"}, {"color", conColor}, {"line_width", "1"}};
		DrawingConnections[new [] {4, 5}] = new Dictionary<string, string> {{"distance", "2"}, {"color", conColor}, {"line_width", "1"}};
		DrawingConnections[new [] {5, 6}] = new Dictionary<string, string> {{"distance", "1"}, {"color", conColor}, {"line_width", "1"}};
		DrawingConnections[new [] {6, 7}] = new Dictionary<string, string> {{"distance", "3"}, {"color", conColor}, {"line_width", "1"}};
		DrawingConnections[new [] {6, 8}] = new Dictionary<string, string> {{"distance", "3"}, {"color", conColor}, {"line_width", "1"}};
		DrawingConnections[new [] {8, 9}] = new Dictionary<string, string> {{"distance", "1"}, {"color", conColor}, {"line_width", "1"}};
		DrawingConnections[new [] {9, 5}] = new Dictionary<string, string> {{"distance", "3"}, {"color", conColor}, {"line_width", "1"}};
		DrawingConnections[new [] {9, 2}] = new Dictionary<string, string> {{"distance", "2"}, {"color", conColor}, {"line_width", "1"}};
		DrawingConnections[new [] {9, 10}] = new Dictionary<string, string> {{"distance", "1"}, {"color", conColor}, {"line_width", "1"}};
		DrawingConnections[new [] {10, 11}] = new Dictionary<string, string> {{"distance", "1"}, {"color", conColor}, {"line_width", "1"}};
		DrawingConnections[new [] {11, 12}] = new Dictionary<string, string> {{"distance", "1"}, {"color", conColor}, {"line_width", "1"}};
		DrawingConnections[new [] {8, 12}] = new Dictionary<string, string> {{"distance", "2"}, {"color", conColor}, {"line_width", "1"}};
		DrawingConnections[new [] {12, 13}] = new Dictionary<string, string> {{"distance", "2"}, {"color", conColor}, {"line_width", "1"}};
		DrawingConnections[new [] {13, 0}] = new Dictionary<string, string> {{"distance", "3"}, {"color", conColor}, {"line_width", "1"}};
		DrawingConnections[new [] {11, 14}] = new Dictionary<string, string> {{"distance", "1"}, {"color", conColor}, {"line_width", "1"}};
		DrawingConnections[new [] {14, 15}] = new Dictionary<string, string> {{"distance", "1"}, {"color", conColor}, {"line_width", "1"}};
		DrawingConnections[new [] {15, 1}] = new Dictionary<string, string> {{"distance", "1"}, {"color", conColor}, {"line_width", "1"}};

		#endregion ConnectionsDeclaration

		#region OtherDeclaration

		Spaceship = GetNode<Ship>("Ship");

		NPCBodySprite = GetNode<Sprite2D>("UI/UIHBox/NPCControl/Panel/SpriteContainer/BodySprite");
		NPCFaceSprite = GetNode<Sprite2D>("UI/UIHBox/NPCControl/Panel/SpriteContainer/FaceSprite");
		NPCNameLabel = GetNode<Label>("UI/UIHBox/NPCControl/NamePanel/NameLabel");
		NPCContainer = GetNode<Node2D>("UI/UIHBox/NPCControl/Panel/SpriteContainer");

		DestinationLabel = GetNode<Label>("UI/NewQuestContainer/PanelContainer/MarginContainer/VBox/HBox/DestinationLabel");
		DestinationPlanet = GetNode<Control>("UI/NewQuestContainer/PanelContainer/MarginContainer/VBox/HBox/PlanetSpace");
		DeliveryContentsLabel = GetNode<RichTextLabel>("UI/NewQuestContainer/PanelContainer/MarginContainer/VBox/DeliveryContentsLabel");
		AcceptButton = GetNode<Button>("UI/NewQuestContainer/PanelContainer/MarginContainer/VBox/HBox/AcceptButton");
		AcceptButtonSound = GetNode<AudioStreamPlayer>("UI/NewQuestContainer/PanelContainer/MarginContainer/VBox/HBox/AcceptButton/AudioStreamPlayer");
		FuelButton = GetNode<Button>("UI/UIHBox/VBox/FuelPanel/FuelVBox/HBox/FuelButton");
		FuelButtonSound = GetNode<AudioStreamPlayer>("UI/UIHBox/VBox/FuelPanel/FuelVBox/HBox/FuelButton/AudioStreamPlayer");

		DeliveryItems = Array.Empty<ItemData>();
		
		DeliveryItems = DeliveryItems.Append(new ItemData() { ItemName = "Wood", Fragility = 1}).ToArray();
		DeliveryItems = DeliveryItems.Append(new ItemData() { ItemName = "Cosmic propaganda", Fragility = 2}).ToArray();
		DeliveryItems = DeliveryItems.Append(new ItemData() { ItemName = "Asteroid samples", Fragility = 3}).ToArray();
		DeliveryItems = DeliveryItems.Append(new ItemData() { ItemName = "Pencils", Fragility = 4}).ToArray();
		DeliveryItems = DeliveryItems.Append(new ItemData() { ItemName = "Lab vials", Fragility = 5}).ToArray();
		DeliveryItems = DeliveryItems.Append(new ItemData() { ItemName = "Miniature Glass City", Fragility = 6}).ToArray();
		DeliveryItems = DeliveryItems.Append(new ItemData() { ItemName = "Cheesecake", Fragility = 7}).ToArray();
		DeliveryItems = DeliveryItems.Append(new ItemData() { ItemName = "Stuffed Crust Pizza", Fragility = 8}).ToArray();
		DeliveryItems = DeliveryItems.Append(new ItemData() { ItemName = "Archeology Findings", Fragility = 9}).ToArray();
		DeliveryItems = DeliveryItems.Append(new ItemData() { ItemName = "Old explosives", Fragility = 10}).ToArray();

		QuestCounter = 1;
		HazardCounter = 0;

		AcceptedQuests = new();
		Deliveries = Array.Empty<Delivery>();
		
		DeliveryScene = GD.Load<PackedScene>("res://Delivery/Delivery.tscn");

		DeliveriesContainer = GetNode<VBoxContainer>("UI/DeliveriesContainer/ScrollContainer/VBoxContainer");

		MoneyLabel = GetNode<Label>("UI/UIHBox/VBox/BalancePanel/BalanceHBox/BalanceLabel");

		FuelLabel = GetNode<Label>("UI/UIHBox/VBox/FuelPanel/FuelVBox/LabelsHBox/FuelLabel");
		FuelLevelLabel = GetNode<Label>("UI/UIHBox/VBox/LocationFuelPanel/HBox/FuelLabel");
		FuelPriceLabel = GetNode<Label>("UI/UIHBox/VBox/FuelPanel/FuelVBox/HBox/FuelPriceLabel");

		ModifiersHBox = GetNode<HBoxContainer>("UI/NewQuestContainer/PanelContainer/MarginContainer/VBox/ModifiersHBox");
		ModifierIconScene = GD.Load<PackedScene>("res://Modifiers/ModifierIcon.tscn");

		MarketCrashIcon = GD.Load<Texture2D>("res://Location/Hazards/FuelCrash.png");
		DisenteryIcon = GD.Load<Texture2D>("res://Location/Hazards/HeatLeeches.png");
		RumIcon = GD.Load<Texture2D>("res://Location/Hazards/Pirates.png");
		HecticIcon = GD.Load<Texture2D>("res://Location/Hazards/Timed.png");
		BacteriaIcon = GD.Load<Texture2D>("res://Location/Hazards/Fragile.png");
		ShutdownIcon = GD.Load<Texture2D>("res://Location/Hazards/Shutdown.png");
		FaultyIcon = GD.Load<Texture2D>("res://Location/Hazards/Faulty.png");

		AchievementQueue = Array.Empty<Achievement>();

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

		VictoryScreen = GetNode<Control>("VictoryScreen");
		FailScreen = GetNode<Control>("FailScreen");

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

		l1.LocationAvailable += OnLocationAvailable;
		l2.LocationAvailable += OnLocationAvailable;
		l3.LocationAvailable += OnLocationAvailable;
		l4.LocationAvailable += OnLocationAvailable;
		l5.LocationAvailable += OnLocationAvailable;
		l6.LocationAvailable += OnLocationAvailable;
		l7.LocationAvailable += OnLocationAvailable;
		l8.LocationAvailable += OnLocationAvailable;
		l9.LocationAvailable += OnLocationAvailable;
		l10.LocationAvailable += OnLocationAvailable;
		l11.LocationAvailable += OnLocationAvailable;
		l12.LocationAvailable += OnLocationAvailable;
		l13.LocationAvailable += OnLocationAvailable;
		l14.LocationAvailable += OnLocationAvailable;
		l15.LocationAvailable += OnLocationAvailable;
		l16.LocationAvailable += OnLocationAvailable;

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

		LocationPickSound = GetNode<AudioStreamPlayer>("LocationPickSound");
		LocationArrivalSound = GetNode<AudioStreamPlayer>("LocationArrivalSound");

		#endregion OtherDeclaration

		CurrentLocation = l2;
		Spaceship.Transform = CurrentLocation.Transform;

		Balance = 0;
		FuelLevel = 10;

		CreateNewNPC();
		DisplayQuest(CreateNewQuest(0, 1));
		FailScreen.Hide();
		UpdateLocationFuelLevel();

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
			// Engine.TimeScale = 0.2f;
			// CreateNewHazard();
		}
		if (@event.IsActionPressed("esc"))
		{
			GetTree().Quit();
		}
		if (@event.IsActionPressed("achievement_menu") && !FailScreen.Visible && !VictoryScreen.Visible)
		{
			AchievementsContainer.Visible = !AchievementsContainer.Visible;
		}
		if (@event.IsActionPressed("buy_fuel") && !FailScreen.Visible && !VictoryScreen.Visible)
		{
			OnFuelButtonPressed();
			FuelButtonSound.Play();
		}
		if (@event.IsActionPressed("accept_quest") && !FailScreen.Visible && !VictoryScreen.Visible)
		{
			OnAcceptQuestButtonPressed();
			AcceptButtonSound.Play();
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
		NewPlanet.SetPlanetRotation(-0.75f + RandomRotation);
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
				NewPlanet.SetPlanetRotation(-1.2f);
				break;
			case "Star":
				NewPlanet.RelativeScale = 2f;
				NewPlanet.GUIZoom = 2f;
				NewPlanet.SetPlanetRotation(2.0f + RandomRotation);
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
		NewPlanet.SetPlanetRotation(-0.75f + RandomRotation);
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
				NewPlanet.SetPlanetRotation(-1.2f);
				break;
			case "Star":
				NewPlanet.RelativeScale = 2f;
				NewPlanet.GUIZoom = 2f;
				NewPlanet.SetPlanetRotation(2.0f + RandomRotation);
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

	private void HighlightPath(int idFrom, int idTo, Color color)
	{
		Dictionary<int[], Dictionary<string, string>> newPath = new();
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
			GD.Print("\t\t|Adding PathCounter... ", PathCounter);
			PathCounter++;
		}
		DrawingPaths[PathCounter] = newPath;
		ReadableConnections[PathCounter] = new int[] {idFrom, idTo};
	}

	private void StopHighlightPath(int idFrom, int idTo)
	{
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

	private Quest CreateNewQuest(int NumOfItems = 0, int newQuestTier = -1, int tagDifficulty = 0)
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
			GD.Print("\t\t|Filling with items...");
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
		switch (Rnd.Next(1 + Mathf.Clamp(tagDifficulty, 0, 3)))
		{
			case 2:
				string newTag = "";
				Dictionary<string, string> newTagData = new();
				switch (Rnd.Next(3))
				{
					case 0:
						newTag = "Timed";
						newTagData["tier"] = (Rnd.Next(3) + 1).ToString();
						newTagData["texture_path"] = "res://UI/Icons/Modifiers/Timed.png";
						break;
					case 1:
						newTag = "Fragile";
						newTagData["jumps"] = (StarMap.GetIdPath(CurrentLocation.ID, ChosenLocation.ID).Length + Rnd.Next(2)).ToString();
						newTagData["texture_path"] = "res://UI/Icons/Modifiers/Fragile.png";
						break;
					case 2:
						newTag = "Segmented";
						newTagData["middle-man-id"] = StarMap.GetIdPath(CurrentLocation.ID, ChosenLocation.ID)[Rnd.Next(1, StarMap.GetIdPath(CurrentLocation.ID, ChosenLocation.ID).Length-1)].ToString();
						newTagData["middle-man-name"] = AllLocations[int.Parse(newTagData["middle-man-id"])].LocationName;
						newTagData["middle-man-met"] = "false";
						newTagData["texture_path"] = "res://UI/Icons/Modifiers/Segmented.png";
						break;
				}
				NewQuest.AddTag(newTag, newTagData);
				break;
			case 3:
				NewQuest.AddTag("Timed", new Dictionary<string, string>() { {"tier", (Rnd.Next(3) + 1).ToString()}, {"texture_path", "res://UI/Icons/Modifiers/Timed.png"} });
				NewQuest.AddTag("Fragile", new Dictionary<string, string>() { {"jumps", (StarMap.GetIdPath(CurrentLocation.ID, ChosenLocation.ID).Length + Rnd.Next(2)).ToString()},
					{"texture_path", "res://UI/Icons/Modifiers/Fragile.png"} });
				break;
			case 4:
				NewQuest.AddTag("Timed", new Dictionary<string, string>() { {"tier", (Rnd.Next(3) + 1).ToString()}, {"texture_path", "res://UI/Icons/Modifiers/Timed.png"} });
				string temp1 = StarMap.GetIdPath(CurrentLocation.ID, ChosenLocation.ID)[Rnd.Next(1, StarMap.GetIdPath(CurrentLocation.ID, ChosenLocation.ID).Length-1)].ToString();
				NewQuest.AddTag("Segmented", new Dictionary<string, string>() {
					{"middle-man-id", temp1},
					{"middle-man-name", AllLocations[int.Parse(temp1)].LocationName},
					{"middle-man-met", "false"},
					{"texture_path", "res://UI/Icons/Modifiers/Segmented.png"} });
				break;
		}
		NewQuest.Destination = ChosenLocation;
		NewQuest.ID = QuestCounter;
		return NewQuest;
	}

	private void OnLocationPressed(int pressedID)
	{
		Location pressedLocation = AllLocations[pressedID];
		if (CurrentLocation != pressedLocation && StarMap.ArePointsConnected(CurrentLocation.ID, pressedID) && !StarMap.IsPointDisabled(pressedID))
		{
			int jumpDistance = GetJumpDistance(CurrentLocation, pressedLocation);
			if (CurrentLocation.Hazards.Contains("Disentery")) jumpDistance *= int.Parse(CurrentLocation.GetHazardData("Disentery")["jumpCost"]);
			if (jumpDistance <= FuelLevel && Spaceship.IsAvailable())
			{
				LastPressedLocation = pressedLocation;
				LastJumpDistance = jumpDistance;
				LocationPickSound.Play();
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
		LocationArrivalSound.Play(1.0f);
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
		UpdateLocationFuelLevel();

		float payoutModifier = 0.0f;
		// Creating and displaying quest according to the current difficulty
		if (ACH_TotalDeliveries < 10)
		{
			DisplayQuest(CreateNewQuest(2 + Rnd.Next(-1, 2), 0, 0));
			payoutModifier = 0.3f;
		}
		else if (ACH_TotalDeliveries < 20)
		{
			foreach(Location loc in AllLocations.Values) { loc.SetMaxFuelLevel(7); loc.SetFuelCost(25); }
			DisplayQuest(CreateNewQuest(3 + Rnd.Next(-1, 2), Rnd.Next(2), 2));
			payoutModifier = 0.6f;
		}
		else
		{
			foreach(Location loc in AllLocations.Values) { loc.SetMaxFuelLevel(5); loc.SetFuelCost(50); }
			DisplayQuest(CreateNewQuest(4 + Rnd.Next(-1, 2), Rnd.Next(1, 3), 3));
			payoutModifier = 1.0f;
		}
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
				Payout = Mathf.FloorToInt(Payout * payoutModifier * (1.0f + delivery.TotalDistance / 5.0f));
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
		if (CurrentLocation.Hazards.Contains("MarketCrash")) FuelPriceLabel.Text = "$" + (CurrentLocation.GetFuelCost() * int.Parse(CurrentLocation.GetHazardData("MarketCrash")["fuelCost"])).ToString();
		else FuelPriceLabel.Text = "$" + CurrentLocation.GetFuelCost().ToString();
		if (CurrentLocation.Hazards.Contains("Rum"))
		{
			int tax = 0;
			Dictionary<string, string> data = CurrentLocation.GetHazardData("Rum");
			if (Balance > 700) tax = Mathf.CeilToInt(int.Parse(data["secondThreshold"]) / 100f * Balance);
			else if (Balance > 300) tax = Mathf.CeilToInt(int.Parse(data["firstThreshold"]) / 100f * Balance);
			Balance -= tax;
			PopupBalance(-tax);
			CurrentLocation.RemoveHazard("Rum");
		}
		if (CurrentLocation.Hazards.Contains("Hectic"))
		{
			foreach (Delivery delivery in Deliveries)
			{
				Dictionary<string, string> newTagData = new() { { "tier", CurrentLocation.GetHazardData("Hectic")["tier"] }, {"texture_path", "res://UI/Icons/Modifiers/Timed.png"} };
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
				newTagData["texture_path"] = "res://UI/Icons/Modifiers/Fragile.png";
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
			if (Balance >= CurrentLocation.GetFuelCost() * int.Parse(CurrentLocation.GetHazardData("MarketCrash")["fuelCost"])) return;
		}
		else if (Balance >= CurrentLocation.GetFuelCost() && CurrentLocation.GetFuelLevel() > 0) return;
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
		else while (location == CurrentLocation)
		{
			GD.Print("\t\t|Picking location...");
			location = AllLocations[Rnd.Next(AllLocations.Count)];
		}
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
		IsQuestAccepted = false;
		CreateNewPlanet(DestinationPlanet, DeliveryLocation.PlanetType, DeliveryLocation.PlanetPreset, 40);
		HighlightPath(CurrentLocation.ID, DeliveryLocation.ID, DeliveryLocation.LocationColor);
	}

	private void EnableQuestButtons()
	{
		AcceptButton.Disabled = false;
		AcceptButton.Visible = true;
	}

	private void DisableQuestButtons()
	{
		AcceptButton.Disabled = true;
		AcceptButton.Visible = false;
	}

	private void OnAcceptQuestButtonPressed()
	{
		if (IsQuestAccepted) return;
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
		IsQuestAccepted = true;
		// Creating entry for currently displayed quest like: [ID]:[DisplayedQuest]
		AcceptedQuests[DisplayedQuest.ID] = DisplayedQuest;
		// Adding a new destination to display on the map
		DisplayedQuest.Destination.AddQuest(DisplayedQuest.ID);
		Delivery NewDelivery = DeliveryScene.Instantiate<Delivery>();
		// Adding a delivery in the cargo hold
		DeliveriesContainer.AddChild(NewDelivery);
		GodotArray[] Result = Array.Empty<GodotArray>();
		NewDelivery.SetPlanet(CreateNewPlanet(NewDelivery, DisplayedQuest.Destination.PlanetType, DisplayedQuest.Destination.PlanetPreset, 25), DisplayedQuest.Destination.LocationColor);
		GD.Print("Creating new Delivery with ", DisplayedQuest.Tags.Length, " tags...");
		foreach (int idx in Enumerable.Range(0, DisplayedQuest.Tags.Length))
		{
			string tag = DisplayedQuest.Tags[idx];
			Dictionary<string, string> tagData = DisplayedQuest.TagsData[idx];
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
		float payoutModifier = 1.0f;
		if (ACH_TotalDeliveries < 10)
		{
			payoutModifier = 0.3f;
		}
		else if (ACH_TotalDeliveries < 20)
		{
			payoutModifier = 0.6f;
		}
		else
		{
			payoutModifier = 1.0f;
		}
		Item[] items = FailedDelivery.GetItems();
		foreach (Item item in items) Payout += item.Fragility * 10;
		Payout = Mathf.FloorToInt(Payout * payoutModifier * (1.0f + FailedDelivery.TotalDistance / 10.0f));
		PopupBalance(-Payout);
		Balance -= Payout;
		FailedDelivery.QueueFree();
		PopupFail("Delivery failed!\n\tReason: " + FailReason);
		ACH_TotalFails++;
	}

	private void PopupFail(string failText)
	{
		Label newFailLabel = new() { Text = failText, Modulate = Colors.Red };
		AddChild(newFailLabel);
		newFailLabel.ZIndex = 1;
		newFailLabel.Position = new Vector2(DeliveriesContainer.GlobalPosition.X + Rnd.Next(-300, -100), DeliveriesContainer.GlobalPosition.Y + 400.0f);
		Tween LabelTween = GetTree().CreateTween();
		LabelTween.TweenProperty(newFailLabel, "position:y", DeliveriesContainer.GlobalPosition.Y + 400.0f - (float)Rnd.NextDouble() * 5.0f - 40.0f, 5.0d);
		LabelTween.TweenProperty(newFailLabel, "modulate", Colors.Transparent, 0.3d).SetDelay(4.7d);
		LabelTween.TweenCallback(Callable.From(newFailLabel.QueueFree)).SetDelay(1.0d);
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
				UpdateLocationFuelLevel();
			}
		}
	}

	private void UpdateLocationFuelLevel()
	{
		FuelLevelLabel.Text = CurrentLocation.GetFuelLevel().ToString();
	}

	private void ClearQuestHighlight() { DisplayedQuest.Destination.ClearHighlight(); }

	private void PopupBalance(int Difference)
	{
		Label PopupLabel = new();
		if (Difference > 0) { PopupLabel.Text = "$" + Difference.ToString(); PopupLabel.Modulate = Colors.Green; }
		else { PopupLabel.Text = Difference.ToString().Insert(1, "$"); PopupLabel.Modulate = Colors.Red; }
		AddChild(PopupLabel);
		PopupLabel.ZIndex = 1;
		PopupLabel.Position = new Vector2(MoneyLabel.GlobalPosition.X + Rnd.Next(-100, 20), MoneyLabel.GlobalPosition.Y - 80);
		Tween LabelTween = GetTree().CreateTween();
		LabelTween.TweenProperty(PopupLabel, "position:y", MoneyLabel.GlobalPosition.Y - (float)Rnd.NextDouble() * 20.0f - 85.0f, 1.0d);
		LabelTween.TweenProperty(PopupLabel, "modulate", Colors.Transparent, 0.3d).SetDelay(0.7d);
		LabelTween.TweenCallback(Callable.From(PopupLabel.QueueFree)).SetDelay(1.0d);
	}

	private void CheckAchievements()
	{
		if (ACH_CompletedQuests > 3) CompleteAchievement(Achievements.MAKING_MULTIPLE_DELIVERIES_3);
		if (ACH_CompletedQuests > 2) CompleteAchievement(Achievements.MAKING_MULTIPLE_DELIVERIES_2);
		if (ACH_CompletedQuests > 1) CompleteAchievement(Achievements.MAKING_MULTIPLE_DELIVERIES_1);

		if (Deliveries.Length > 3) CompleteAchievement(Achievements.HAVING_MULTIPLE_DELIVERIES_3);
		if (Deliveries.Length > 2) CompleteAchievement(Achievements.HAVING_MULTIPLE_DELIVERIES_2);
		if (Deliveries.Length > 1) CompleteAchievement(Achievements.HAVING_MULTIPLE_DELIVERIES_1);

		if (ACH_TotalFails > 4) CompleteAchievement(Achievements.TOTAL_FAILS_3);
		if (ACH_TotalFails > 2) CompleteAchievement(Achievements.TOTAL_FAILS_2);
		if (ACH_TotalFails > 1) CompleteAchievement(Achievements.TOTAL_FAILS_1);

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
				CompleteAchievement(Achievements.TOTAL_DIFFERENT_ITEMS_1);
				CompleteAchievement(Achievements.TOTAL_DIFFERENT_ITEMS_2);
				break;
			case 3:
				CompleteAchievement(Achievements.TOTAL_DIFFERENT_ITEMS_1);
				CompleteAchievement(Achievements.TOTAL_DIFFERENT_ITEMS_2);
				CompleteAchievement(Achievements.TOTAL_DIFFERENT_ITEMS_3);
				break;
		}

		if (ACH_TotalItems > 99) CompleteAchievement(Achievements.TOTAL_ITEMS_3);
		if (ACH_TotalItems > 49) CompleteAchievement(Achievements.TOTAL_ITEMS_2);
		if (ACH_TotalItems > 19) CompleteAchievement(Achievements.TOTAL_ITEMS_1);

		if (ACH_TotalDeliveries > 49) CompleteAchievement(Achievements.TOTAL_DELIVERIES_3);
		if (ACH_TotalDeliveries > 24) CompleteAchievement(Achievements.TOTAL_DELIVERIES_2);
		if (ACH_TotalDeliveries > 14) CompleteAchievement(Achievements.TOTAL_DELIVERIES_1);
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
			GD.PrintErr("Path from ", From.LocationName, " to ", To.LocationName, " is ", Path.Length, "\n\t", To.LocationName, " disabled: ", StarMap.IsPointDisabled(To.ID));
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
		AcceptButtonSound.Play();
		VictoryScreen.Hide();
	}

	private void OnExitButtonPressed()
	{
		GetTree().Quit();
	}

	private void GameOver()
	{
		FailScreen.Show();
	}

	private void OnRetryButtonPressed()
	{
		GD.Print("Reload: " + GetTree().ReloadCurrentScene());
	}

	private void OnAcceptQuestButtonDown()
	{
		AcceptButtonSound.Play();
	}

	private void OnFuelButtonDown()
	{
		FuelButtonSound.Play();
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
		switch (newTag)
		{
			case "Timed":
				NewModifier.SetSprite(GD.Load<Texture2D>("res://UI/Icons/Modifiers/Timed.png"));
				NewModifier.SetData((20 - int.Parse(newTagData["tier"]) * 5).ToString());
				NewModifier.TooltipText = "You better be fast!\nThis delivery has a timer on it.";
				break;
			case "Fragile":
				NewModifier.SetSprite(GD.Load<Texture2D>("res://UI/Icons/Modifiers/Fragile.png"));
				NewModifier.SetData(newTagData["jumps"]);
				NewModifier.TooltipText = "These boxes seem more fragile than usual...\nEach jump is important, deliver the items in provided number of jumps.";
				break;
			case "Segmented":
				NewModifier.SetSprite(GD.Load<Texture2D>("res://UI/Icons/Modifiers/Segmented.png"));
				NewModifier.SetData(newTagData["middle-man-name"]);
				NewModifier.TooltipText = "I like how certain planets influence my goods, here's the path...\nMake sure you go to the specified planet before completing the order.";
				break;
		}
		return NewModifier;
	}
}
