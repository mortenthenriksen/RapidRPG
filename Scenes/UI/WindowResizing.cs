using Game.Characters;
using Game.Manager;
using Godot;

namespace Game.UI;

public partial class WindowResizing : CanvasLayer
{
    private Button smallWindowButton;
    private Button largeWindowButton;
    private Button levelUpButton;
    private Window root;

    public override void _Ready()
    {
        smallWindowButton = GetNode<Button>("VBoxContainer/SmallWindowButton");
        largeWindowButton = GetNode<Button>("VBoxContainer/LargeWindowButton");
        levelUpButton = GetNode<Button>("VBoxContainer/LevelUpButton");
        root = GetTree().Root;

        smallWindowButton.Pressed += OnSmallWindowButtonPressed;
        largeWindowButton.Pressed += OnLargeWindowButtonPressed;
        levelUpButton.Pressed += OnLevelUpButtonPressed;


        // Set initial viewport settings
        root.ContentScaleMode = Window.ContentScaleModeEnum.CanvasItems;
        root.ContentScaleFactor = 1.0f;
    }

    private void OnSmallWindowButtonPressed()
    {
        Vector2I smallWindowSize = new(1920 / 2, 1080 / 2);
        root.ContentScaleFactor = 1.0f;
        DisplayServer.WindowSetSize(smallWindowSize);
        DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
        root.Size = smallWindowSize;
    }

    private void OnLargeWindowButtonPressed()
    {
        Vector2I largeWindowSize = new(1200 * 2, 675 * 2);
        root.ContentScaleFactor = 1f; // 1920/1200 = 1.6
        DisplayServer.WindowSetSize(largeWindowSize);
        DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
        root.Size = largeWindowSize;
    }
    
    private void OnLevelUpButtonPressed()
    {
        LevelManager.Instance.IncreaseLevel();
    }
}