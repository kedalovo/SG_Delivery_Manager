using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Delivery : HBoxContainer
{
	[Signal]
	public delegate void OnDeliveryFailedEventHandler(int DeliveryID);

	private int id;

	public int ID
    {
        get => id;
        set
        {
            if (id == -1) id = value;
            else GD.Print("Delivery already has ID: " + id.ToString());
        }
    }

	private PackedScene ItemScene;
	private HFlowContainer ItemsHFlow;

	private Item[] Items = Array.Empty<Item>();
	private string[] Tags = Array.Empty<string>();
	private Dictionary<string, string>[] TagsData = Array.Empty<Dictionary<string, string>>();
	private Label[] TagLabels = Array.Empty<Label>();

	private Timer DeliveryTimer;

	private int DistanceJumped = 0;
	private int TimedTagIdx = 0;
	private int FragileTagIdx = 0;

	public override void _Ready()
	{
		ItemScene = GD.Load<PackedScene>("res://Item/Item.tscn");
		ItemsHFlow = GetNode<HFlowContainer>("ItemsHFlow");
		DeliveryTimer = GetNode<Timer>("Timer");
	}

    public void SetItems(int newID, ItemData[] newItems)
	{
		id = newID;
		GetNode<Label>("VBox/IDLabel").Text = "[" + id + "]";
		foreach (ItemData newItemData in newItems)
		{
			Item NewItem = ItemScene.Instantiate<Item>();
			ItemsHFlow.AddChild(NewItem);
            Items = Items.Append(NewItem).ToArray();
			NewItem.SetItem(newItemData);
		}
	}

	public void Damage(int Distance)
	{
		bool AllDone = true;
		foreach (Item _Item in Items)
		{
			_Item.Damage(Distance);
			if (_Item.HP > 0) { AllDone = false; }
		}
		if (AllDone) FailQuest();
	}

	public int[] GetItemsDurability()
	{
		int [] ItemsDurability = Array.Empty<int>();
		foreach (Item _Item in Items) { ItemsDurability = ItemsDurability.Append(_Item.HP).ToArray(); }
		return ItemsDurability;
	}

	public int GetPayout(bool IsPenalty = false)
	{
		int Payout = 0;
		if (IsPenalty) foreach (Item _Item in Items) { Payout += _Item.Fragility * 10; }
		else foreach (Item _Item in Items) { Payout += (int)Mathf.Floor(_Item.Fragility * 10 * _Item.HP / 100); }
		for (int i = 0; i < Tags.Length; i++)
		{
			string tag = Tags[i];
			Dictionary<string, string> tagData = TagsData[i];
			switch (tag)
			{
				case "Timed":
					GD.Print("Timed modified payout from ", Payout);
					Payout = Mathf.FloorToInt(Payout * (1f + .2f * int.Parse(tagData["tier"])));
					GD.Print("to ", Payout);
					break;
				case "Fragile":
					GD.Print("Fragile modified payout from ", Payout);
					Payout = Mathf.FloorToInt(Payout * 1.5f);
					GD.Print("to ", Payout);
					break;
			}
		}
		return Payout;
	}

	public bool HasTag(string tag) { return Tags.Contains(tag); }

    public void AddTag(string newTag, Dictionary<string, string> newTagData, ModifierIcon newTagIcon) 
	{
		if (HasTag(newTag)) return;
		Tags = Tags.Append(newTag).ToArray();
		TagsData = TagsData.Append(newTagData).ToArray();
		switch (newTag)
		{
			case "Timed":
				TimedTagIdx = TagsData.Length - 1;
				TagsData[TimedTagIdx]["time"] = (20 - int.Parse(newTagData["tier"])*5).ToString();
				DeliveryTimer.Start();
				break;
			case "Fragile":
				FragileTagIdx = TagsData.Length - 1;
				DistanceJumped = 0;
				break;
		}
		GetNode("VBox").AddChild(newTagIcon);
		TagLabels = TagLabels.Append(newTagIcon.GetDataLabel()).ToArray();
	}

	public void Jump()
	{
		DistanceJumped++;
		for (int i = 0; i < Tags.Length; i++)
		{
			if (Tags[i] == "Fragile") 
			{
				TagLabels[i].Text = (int.Parse(TagsData[i]["jumps"]) - DistanceJumped).ToString();
				if (int.Parse(TagLabels[i].Text) == 0) FailQuest();
				break;
			}
		}
	}

	private void FailQuest()
	{
		EmitSignal(SignalName.OnDeliveryFailed, id);
	}

	public void OnDeliveryTimerTimeout()
	{
		if (TagsData[TimedTagIdx]["time"] != "0")
		{
			TagsData[TimedTagIdx]["time"] = (int.Parse(TagsData[TimedTagIdx]["time"]) - 1).ToString();
			TagLabels[TimedTagIdx].Text = TagsData[TimedTagIdx]["time"];
		}
		else
		{
			GD.Print(id + ": Timeout!");
			FailQuest();
		}
	}
}