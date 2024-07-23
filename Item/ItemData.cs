using System;
using Godot;

public partial class ItemData : Node
{
    private string itemName;
    private Texture2D texture;
    private int fragility = -1;

    public string ItemName
    {
        get => itemName;
        set { if (itemName is null) itemName = value; else GD.Print(string.Format("Can't name \"{0}\", item already has a name: \"{1}\""), value, itemName); }
    }
    public Texture2D Texture
    {
        get => texture;
        set { if (texture is null) texture = value; else GD.Print("Item already has a texture"); }
    }
    public int Fragility
    {
        get => fragility;
        set { if (fragility == -1) fragility = Math.Clamp(value, 1, 10); else GD.Print(string.Format("Can't set fragility to {0}, item already has fragility of {1}", value, fragility)); }
    }
}
