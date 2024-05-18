using Godot;
using System;

public partial class GameEngine : Node
{
    

//     private Node2D inventory;
//     private ItemDrop itemDrop;
//     private Node playerInventory;
//     private Dave player;

//     public override void _Ready()
//     {
//         inventory = GetNode<Node2D>("/root/Game/UserInterface/Inventory");
//         playerInventory = GetNode<Node>("/root/PlayerInventory");
//         itemDrop = GetNode<ItemDrop>("/root/Game/ItemDrop");
//         player = GetNode<Dave>("/root/Game/Dave");
//     }


//     public override void _Process(double delta)
//     {
//         // TODO: would like to make the game engine responsible for picking up items, and not the item itself
//         // if (itemDrop.isBeingPickedUp) 
//         // {
//         //     playerInventory.Call("add_item", itemDrop.Name, 1);
//         //     inventory.Call("initialize_inventory");
//         //     QueueFree();
//         // }

//         if (Input.IsActionJustPressed("open_inventory")) 
//         {   
//             if (inventory.Visible == false) 
//             {
//                 inventory.Visible = true;
//             }
//             else 
//             {
//                 inventory.Visible = false;
//             }
//         }


//         if (Input.IsActionJustReleased("scroll_up")) 
//         {
//             playerInventory.Call("active_item_scroll_down");
//         }

//         if (Input.IsActionJustReleased("scroll_down")) 
//         {
//             playerInventory.Call("active_item_scroll_up");
//         }

//     }
}


