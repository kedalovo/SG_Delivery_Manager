using Godot;
using System;

public partial class Item : HBoxContainer
{
	private ProgressBar ItemHP;

	public int HP { get { return (int)ItemHP.Value; } }

	public int Fragility;
	public string ItemName;

	public override void _Ready()
	{
		ItemHP = GetNode<ProgressBar>("ItemHP");
	}

	public void SetItem(ItemData newItemData)
	{
		ItemName = newItemData.ItemName;
		GetNode<TextureRect>("ItemIcon").Texture = newItemData.Texture;
		Fragility = newItemData.Fragility;
		GetNode<Label>("DamageLabel").Text = Fragility.ToString();
	}

	public void Damage(int Distance)
	{
		Distance = Math.Clamp(Distance, 1, 3);
		int Damage = Fragility * Distance;
		if (ItemHP.Value > Damage)
		{
			ItemHP.Value -= Damage;
		}
		else
		{
			ItemHP.Value = 0;
		}
		if (ItemHP.Value > 75) { Modulate = Colors.Green; }
		else if (ItemHP.Value > 50) { Modulate = Colors.Yellow; }
		else if (ItemHP.Value > 25) { Modulate = Colors.Orange; }
		else if (ItemHP.Value > 0) { Modulate = Colors.OrangeRed; }
		else { Modulate = Colors.Red; }
	}
}
