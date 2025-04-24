using System;
using Game.Autoload;
using Game.Characters;
using Godot;
using Godot.Collections;


public partial class ItemDrop : CharacterBody2D
{
    public string itemName;
    private Node InventoryLogic;
    private Dave player = null;
    private Label itemNameLabel;
    private AudioStreamPlayer2D audioStreamPlayer2D;
    private Dictionary itemDataDict;
    private Dictionary itemDataDictValue;
    private bool isBeingPickedUp = false;
    private int itemsInData;

    WeightedGroup<string> weightedItemDrops = new WeightedGroup<string>();

    public override void _Ready()
    {
        InventoryLogic = GetNode<Node>("/root/InventoryLogic");
        itemNameLabel = GetNode<Label>("ItemNameLabel");
        audioStreamPlayer2D = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
        itemDataDict = GDToCSDataConverter.Instance.GetValuesDictionaries();
        foreach (var item in itemDataDict)
        {
            string itemName = (string)item.Key;
            itemDataDictValue = (Dictionary)itemDataDict[item.Key];
            // checks if i remember to add a weight to each item :) else just puts it to 0
            int itemWeight = itemDataDictValue.ContainsKey("Weight") ? (int)itemDataDictValue["Weight"] : 0;  
            weightedItemDrops.Add(itemName, itemWeight);
        }

        if (string.IsNullOrEmpty(itemName))
        {
            itemName = WeightedItemDrop();
        }
        else
        {
            UpdateItem(itemName);
        }
    }

    private void UpdateItem(string itemName)
    {
        var sprite = GetNode<Sprite2D>("Sprite2D");
        var texture = (Texture)ResourceLoader.Load($"res://Inventory/ItemIcons/{itemName}.png");
        sprite.Texture = (Texture2D)texture;
        sprite.Scale = new Vector2(1f, 1f);
        itemNameLabel.Text = itemName;
        var dict = (Dictionary)itemDataDict[itemName];
        if (dict.ContainsKey("UniqueEffect") || dict.ContainsKey("UniqueSetEffect"))
        { 
            audioStreamPlayer2D.Play();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (isBeingPickedUp) 
        {
            InventoryLogic.Call("add_item", itemName, 1);
            QueueFree();
        }
    }

    public string WeightedItemDrop() 
    {
        var itemDropName = weightedItemDrops.GetItem();
        UpdateItem(itemDropName);
        return itemDropName;
    }

    public void SetItemName(string name)
    {
        itemName = name;
    }

    public void PickupItem(Dave body)
    {
        player = body;
        isBeingPickedUp = true;
    }
}
