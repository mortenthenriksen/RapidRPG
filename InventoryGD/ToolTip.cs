using Godot;
using System;

public partial class ToolTip : Node2D
{
    private bool isShowing = false;

    public override void _Ready()
    {
        Hide(); // Hide the tooltip initially
    }

    public override void _Process(double delta)
    {
        if (isShowing)
        {
            FollowCursor();
        }
    }

    public void UpdateToolTip(string itemName)
    {
        var itemNameLabel = GetNodeOrNull<Label>("TextureRect/MarginContainer/VBoxContainer/ItemName");
        if (itemNameLabel == null)
        {
            GD.PrintErr("Node not found: TextureRect/MarginContainer/VBoxContainer/ItemName");
            return;
        }
        itemNameLabel.Text = itemName;
        UpdateStats("Defence", "10");
    }

    private void UpdateStats(string stat1, string valueOfStat1)
    {
        var statLabel = GetNodeOrNull<Label>("TextureRect/MarginContainer/VBoxContainer/Stat1/Stat");
        var differenceLabel = GetNodeOrNull<Label>("TextureRect/MarginContainer/VBoxContainer/Stat1/Difference");

        if (statLabel == null || differenceLabel == null)
        {
            GD.PrintErr("Node not found: TextureRect/MarginContainer/VBoxContainer/Stat1/Stat or Difference");
            return;
        }

        statLabel.Text = stat1; 
        differenceLabel.Text = valueOfStat1; 
    }

    private void FollowCursor()
    {
        Godot.Vector2 offset = new Godot.Vector2(300, 200);
        Position = GetGlobalMousePosition() - offset;
    }

    public void ShowToolTip()
    {
        if (!isShowing) 
        {
            Show();
            isShowing = true;
        }
    }
    
    public void HideToolTip()
    {
        if (isShowing)
        {
            Hide();
            isShowing = false;
        }
    }
}