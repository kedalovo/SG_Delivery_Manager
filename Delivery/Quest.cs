using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Quest : Node
{
	private ItemData[] items = Array.Empty<ItemData>();
	private Location destination;
	private int id = -1;
	public string[] Tags = Array.Empty<string>();
	public Dictionary<string, string>[] TagsData = Array.Empty<Dictionary<string, string>>();

	public ItemData[] Items
	{
		get => items;
		set 
		{
			if (items.Length == 0) items = value;
			else GD.Print("Quest already contains items");
		}
	}
	public int ID
	{
		get => id;
		set
		{
			if (id == -1) id = value;
			else GD.Print("Quest already has ID: " + id.ToString());
		}
	}
	public Location Destination
	{
		get => destination;
		set
		{
			if (destination is null) destination = value;
			else GD.Print("Quest already has destination: " + destination.LocationName);
		}
	}
	
	public bool HasTag(string tag) { return Tags.Contains(tag); }

	public void AddTag(string newTag, Dictionary<string, string> newTagData)
	{
		Tags = Tags.Append(newTag).ToArray();
		TagsData = TagsData.Append(newTagData).ToArray();
	}
}