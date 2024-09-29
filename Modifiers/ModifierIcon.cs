using Godot;

public partial class ModifierIcon : CenterContainer
{
    private string tag;
    
    public string Tag
    {
        get => tag;
        set { tag ??= value; }
    }

    private Label DataLabel;
    private Control PlanetControl;

    private string Data;

    public override void _Ready()
    {
        DataLabel = GetNode<Label>("HBox/DataLabel");
        PlanetControl = GetNode<Control>("HBox/PlanetControl");
        DataLabel.Text = Data;
    }

    public void SetSprite(Texture2D newSprite)
    {
        Sprite2D Icon = GetNode<Sprite2D>("Icon");
        Icon.Texture = newSprite;
    }

    public void SetData(string newData)
    {
        if (DataLabel is not null) DataLabel.Text = newData;
        else Data = newData;
    }

    public void SetPlanet(Planet newPlanet)
    {
        newPlanet.Reparent(PlanetControl);
        newPlanet.Position = new Vector2(32, -2);
    }

    public Label GetDataLabel()
    {
        return DataLabel;
    }

    public void HideData()
    {
        DataLabel.Hide();
    }

    public void ShowData()
    {
        DataLabel.Show();
    }
}
