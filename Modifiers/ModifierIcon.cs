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

    private string Data;

    public override void _Ready()
    {
        DataLabel = GetNode<Label>("HBox/DataLabel");
        DataLabel.Text = Data;
    }

    public void SetSprite(SpriteFrames newSprite)
    {
        AnimatedSprite2D Icon = GetNode<AnimatedSprite2D>("Icon");
        Icon.SpriteFrames = newSprite;
    }

    public void SetData(string newData)
    {
        if (DataLabel is not null) DataLabel.Text = newData;
        else Data = newData;
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
