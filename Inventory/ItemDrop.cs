using System.Linq;
using Game.Autoload;
using Game.Characters;
using Godot;
using Godot.Collections;


public partial class ItemDrop : CharacterBody2D
{
    private string itemName;
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
        itemName = WeightedItemDrop();
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
        // var itemDropName = "Sword of Undying Flame";
        var sprite = GetNode<Sprite2D>("Sprite2D");
        var texture = (Texture)ResourceLoader.Load($"res://Inventory/ItemIcons/{itemDropName}.png");
        sprite.Texture = (Texture2D)texture;
        sprite.Scale = new Vector2(1f, 1f);
        itemNameLabel.Text = itemDropName;
        var dict = (Dictionary)itemDataDict[itemDropName];
        if (dict.ContainsKey("UniqueEffect") || dict.ContainsKey("UniqueSetEffect"))
        { 
            audioStreamPlayer2D.Play();
        }
        return itemDropName;
    }

    public void PickupItem(Dave body)
    {
        player = body;
        isBeingPickedUp = true;
    }
}
