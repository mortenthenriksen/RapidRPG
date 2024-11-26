using Godot;

namespace Game.UI;

public partial class WindowResizing : CanvasLayer
{
    private Button smallWindowButton;
    private Button largeWindowButton;

    public override void _Ready()
    {
        smallWindowButton = GetNode<Button>("VBoxContainer/SmallWindowButton");
        largeWindowButton = GetNode<Button>("VBoxContainer/LargeWindowButton");

        smallWindowButton.Pressed += OnSmallWindowButtonPressed;
        largeWindowButton.Pressed += OnLargeWindowButtonPressed;
    }

    private void OnSmallWindowButtonPressed()
    {
        Vector2I smallWindowSize = new Vector2I(960, 540);
        DisplayServer.WindowSetSize(smallWindowSize); 
        GetViewport().CanvasTransform = new Transform2D(0.5f, 0, 0, 0.5f, 0, 0); 
        DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
    }

    private void OnLargeWindowButtonPressed()
    {
        Vector2I largeWindowSize = new Vector2I(1920, 1080);
        DisplayServer.WindowSetSize(largeWindowSize);
        GetViewport().CanvasTransform = new Transform2D(1, 0, 0, 1, 0, 0);
        DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
    }
}
