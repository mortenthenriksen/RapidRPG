using System.Collections.Generic;
using System.Linq;
using Godot;


namespace Game.Autoload;

[GlobalClass]

public partial class GDToCSDataConverter : Node
{
    [Export]
    private Node jsonData;
    private Dictionary<string, ItemDataClass> itemData;
    private readonly static string filePath = "res://InventoryGD/Data/ItemData.json";


    public override void _Ready()
    {
        var godotDict = (Godot.Collections.Dictionary)jsonData.Call("LoadData", filePath);
        itemData = new Dictionary<string, ItemDataClass>();

        foreach (var key in godotDict.Keys)
        {
            var value = godotDict[key];
            var valueDict = (Godot.Collections.Dictionary)value;
            if (valueDict.ContainsKey("Defense"))
            {
                GD.Print(valueDict["Defense"]);
            }
        }

    }
}



internal class ItemDataClass
{
    public string ItemCategory { get; set; }
    public int Defense { get; set; }
    public int? AddHealth { get; set; }
    public int? AddEnergy { get; set; }
    public int StackSize { get; set; }
    public string Description { get; set; }
    public int? ItemAttack { get; set; }
    public double? ItemSpeed { get; set; }
}