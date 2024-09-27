using Godot;
using System;

public partial class Item : HBoxContainer
{
	private int ItemHP;

	public int HP { get { return ItemHP; } }

	public int Fragility;
	public string ItemName;

	public override void _Ready()
	{
		ItemHP = 100;
	}

	public void SetItem(ItemData newItemData)
	{
		ItemName = newItemData.ItemName;
		// GetNode<TextureRect>("ItemIcon").Texture = newItemData.Texture;
		Fragility = newItemData.Fragility;
		// GetNode<Label>("DamageLabel").Text = Fragility.ToString();
	}

	public void Damage(int Distance)
	{
		Distance = Math.Clamp(Distance, 1, 6);
		int Damage = Fragility * Distance;
		if (ItemHP > Damage)
		{
			ItemHP -= Damage;
		}
		else
		{
			ItemHP = 0;
		}
	}
}
