using Godot;
using System;

public partial class InventoryButton : Button
{

	public Item inventoryItem;
	private TextureRect icon;
	private Label quantityLabel;
    private int index;


	public override void _Ready()
	{

		icon = GetNode<TextureRect>("TextureRect");
		quantityLabel = GetNode<Label>("Label");
	}

    public void UpdateItem(Item item, int index) 
	{
        this.index = index;
        inventoryItem = item;
		if (item == null) 
		{
			icon.Texture = null;
			quantityLabel.Text = string.Empty;
		}
		else  
		{
			icon.Texture = item.Icon;
			quantityLabel.Text = item.Quantity.ToString();
		}
    }
}
