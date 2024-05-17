using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Inventory : Control
{

	private PackedScene inventoryButton;
	private GridContainer gridContainer;
	private List<Item> items = new List<Item>();

	[Export]
	private string itemButtonPath = "res://HelperScenes/Inventory/InventoryButton.tscn";

	[Export]
	public int Capacity {get; set;}

	public InventoryButton GrabbedObject {get; set;}
	public InventoryButton HoverOverButton {get; set;} 
	private Vector2 lastMouseClickedPostion;

	public override void _Ready()
	{
		gridContainer = GetNode<GridContainer>("Panel/GridContainer");
		inventoryButton = ResourceLoader.Load<PackedScene>(itemButtonPath);
		PopulateButtons();

	}

	public override void _Process(double delta)
	{
		GetNode<Area2D>("MouseArea2D").Position = GetTree().Root.GetMousePosition();
		if (HoverOverButton != null) 
		{
			if (Input.IsActionJustPressed("clickLeft")) 
			{
				GrabbedObject = HoverOverButton;
				lastMouseClickedPostion = GetTree().Root.GetMousePosition();
			}

			if (lastMouseClickedPostion.DistanceTo(GetTree().Root.GetMousePosition()) > 2) 
			{
				if (Input.IsActionPressed("clickLeft")) 
				{
					InventoryButton button = GetNode<Area2D>("MouseArea2D").GetNode<InventoryButton>("InventoryButton");
					button.Show();
					button.UpdateItem(GrabbedObject.inventoryItem, 0);
				}
				if (Input.IsActionJustReleased("clickLeft"))
                {
                    SwapButtons(GrabbedObject, HoverOverButton);
                    InventoryButton button = GetNode<Area2D>("MouseArea2D").GetNode<InventoryButton>("InventoryButton");
                    button.Hide();
                }
            }
		}
	}

    private void SwapButtons(InventoryButton button1, InventoryButton button2)
    {
        int buttonindex = button1.GetIndex();
        int button2index = button2.GetIndex();
        gridContainer.MoveChild(button1, button2index);
        gridContainer.MoveChild(button2, buttonindex);
    }

    private void PopulateButtons() 
	{
		for (int i = 0; i < Capacity; i++) {
			InventoryButton currentInventoryButton = inventoryButton.Instantiate<InventoryButton>();
			gridContainer.AddChild(currentInventoryButton);
		}
	}

	// private void PopulateButtons() 
	// {
	// 	foreach (Node child in gridContainer.GetChildren()) {
	// 		if (child is Button) {
	// 			InventoryButton currentInventoryButton = inventoryButton.Instantiate() as InventoryButton;
	// 			child.AddChild(currentInventoryButton);
	// 		}
	// 	}
	// }

	public void Add(Item item) 
	{
		Item currentItem = item.Duplicate() as Item;
		
 		for (int i = 0; i < items.Count; i++) 
		{
			if (items[i].ID == currentItem.ID && items[i].Quantity != items[i].StackSize) 
			{
				if (items[i].Quantity + currentItem.Quantity > items[i].StackSize) 
				{
					items[i].Quantity = currentItem.StackSize;
					currentItem.Quantity = -(currentItem.Quantity - items[i].StackSize);
					UpdateButton(i);
				}
				else 
				{
					items[i].Quantity += currentItem.Quantity;
					currentItem.Quantity = 0;
					UpdateButton(i);
				}
			}
		}

		if (currentItem.Quantity > 0) 
		{
			if (currentItem.Quantity < currentItem.StackSize) 
			{
				items.Add(currentItem);
				UpdateButton(items.Count - 1);
			}
			else 
			{
				Item tempItem = currentItem.Duplicate() as Item; 
				tempItem.Quantity = currentItem.StackSize;
				items.Add(tempItem);
				UpdateButton(items.Count - 1);
				currentItem.Quantity -= currentItem.StackSize;
				Add(currentItem);
			}
		}
	}

	public bool Remove(Item item) 
	{
		if (CanAfford(item)) 
		{
			Item currentItem = item.Duplicate() as Item;
			for (int i = 0; i < items.Count; i++) 
			{
				if (items[i].ID == currentItem.ID) 
				{
					if (items[i].Quantity - currentItem.Quantity < 0) 
					{
						currentItem.Quantity -= items[i].Quantity;
						items[i].Quantity = 0;
						UpdateButton(i);
					}
					else 
					{
						items[i].Quantity -= currentItem.Quantity;
						currentItem.Quantity = 0;
						UpdateButton(i);
					}
				}

				if (currentItem.Quantity <= 0) 
				{
					break;
				}
			}

			items.RemoveAll(x => x.Quantity <= 0);
			if (currentItem.Quantity > 0) 
			{
				Remove(currentItem);
			}
			ReflowButtons();
			return true;
		}
		return false;
	}


	private bool CanAfford(Item item) 
	{
		List<Item> currentItems = items.Where(x => x.ID == item.ID).ToList();
		int i = 0;
		foreach (var item1 in currentItems)
		{
			i += item1.Quantity;
		}
		if (item.Quantity < i )
		{
			return true;
		}
		return false;
	}


	private void ReflowButtons() 
	{
		for (int i = 0; i < Capacity; i++) 
		{
			UpdateButton(i);
		}
	}

	public void UpdateButton(int index) 
	{
		if (items.ElementAtOrDefault(index) != null) 
		{
			gridContainer.GetChild<InventoryButton>(index).UpdateItem(items[index], index);
		}
		else 
		{
			gridContainer.GetChild<InventoryButton>(index).UpdateItem(null, index);
		}
	}


	public void OnAddButtonButtonDown() 
	{
		Add(ResourceLoader.Load<Item>("res://TestItem.tres"));
	}


	public void OnRemoveButtonButtonDown() 
	{
		Remove(ResourceLoader.Load<Item>("res://TestItem.tres"));
	}

	public void OnMouseArea2dAreaEntered(Area2D area) 
	{
		Control button = area.GetParent<Control>();
		if (button is InventoryButton) 
		{
			HoverOverButton = (InventoryButton)button;
		}
	}

	public void OnMouseArea2dAreaExited(Area2D area) => HoverOverButton = null;
}
