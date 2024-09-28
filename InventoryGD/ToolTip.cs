// using Godot;
// using Godot.Collections;
// using System;

// public partial class ToolTip : Node2D
// {

//     private bool isShowing = false;

//     public override void _Ready()
//     {
//         Hide(); // Hide the tooltip initially
//     }

//     public override void _Process(double delta)
//     {
//         // if (isShowing)
//         // {
//         //     FollowCursor();
//         // }
//     }

//     public void UpdateToolTip(string itemName)
//     {
//         var itemNameLabel = GetNode<Label>("TextureRect/MarginContainer/VBoxContainer/ItemName");
//         itemNameLabel.Text = itemName;
//     }

//     // private void UpdateStats(string stat1, string valueOfStat1)
//     // {
//     //     var statLabel = GetNode<Label>("TextureRect/MarginContainer/VBoxContainer/Stat1/Stat");
//     //     var differenceLabel = GetNode<Label>("TextureRect/MarginContainer/VBoxContainer/Stat1/Difference");

//     //     statLabel.Text = stat1 + ": "; 
//     //     differenceLabel.Text = valueOfStat1; 
//     // }


//     public void UpdateStats(string stat, string value)
//     {
//         var statContainer = GetNode<VBoxContainer>("TextureRect/MarginContainer/VBoxContainer/");
//         var statLabel = new Label();
//         statLabel.Text = $"{stat}: {value}";
//         statContainer.AddChild(statLabel);
//     }




//     private void FollowCursor()
//     {
//         Godot.Vector2 offset = new Godot.Vector2(300, 200);
//         Position = GetGlobalMousePosition() - offset;
//     }

//     public void ShowToolTip()
//     {
//         if (!isShowing) 
//         {
//             Show();
//             isShowing = true;
//         }
//     }
    
//     public void HideToolTip()
//     {
//         if (isShowing)
//         {
//             Hide();
//             isShowing = false;
//         }
//     }
// }