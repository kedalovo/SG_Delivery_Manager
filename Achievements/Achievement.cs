using Godot;
using System;

[Tool]
public partial class Achievement : HBoxContainer
{
    private string description;
    private Texture2D icon_texture;
    private bool enabled;

    [Export(PropertyHint.MultilineText)]
    public string Description
    {
        get => description;
        set { DescriptionLabel ??= GetNode<Label>("DescriptionLabel"); DescriptionLabel.Text = value; description = value; }
    }
    [Export]
    public Texture2D IconTexture
    {
        get => icon_texture;
        set { Icon ??= GetNode<TextureRect>("Icon"); Icon.Texture = value; icon_texture = value; }
    }
    [Export]
    public bool Enabled
    {
        get => enabled;
        set
        {
            if (value == true) Modulate = Colors.White;
            else Modulate = new Color("484848");
            enabled = value;
        }
    }

    private TextureRect Icon;
    private Label DescriptionLabel;

    public override void _Ready()
    {
        Modulate = new Color("484848");
        Icon = GetNode<TextureRect>("Icon");
        DescriptionLabel = GetNode<Label>("DescriptionLabel");
    }
}
