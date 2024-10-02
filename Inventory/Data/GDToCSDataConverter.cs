using System.Collections.Generic;
using System.Linq;
using Godot;


namespace Game.Autoload;

[GlobalClass]

public partial class GDToCSDataConverter : Node
{
    public static GDToCSDataConverter Instance { get; private set; }
    
    [Export]
    private Node jsonData;

    private readonly static string filePath = "res://Inventory/Data/ItemDataFromExcel.json";


    public override void _Ready()
    {
        GetValuesDictionaries();
    }

    public Godot.Collections.Dictionary GetValuesDictionaries()
    {
        var godotDict = (Godot.Collections.Dictionary)jsonData.Call("LoadData", filePath);
        // itemData = new Dictionary<string, ItemDataClass>();

        // // Check if "Iron Helmet" exists in the dictionary
        // if (godotDict.ContainsKey("Tree Branch"))
        // {
        //     var valueDict = (Godot.Collections.Dictionary)godotDict["Tree Branch"];
        //     // Check if "Defense" key exists in the value dictionary
        //     if (valueDict.ContainsKey("Defense"))
        //     {
        //         GD.Print(valueDict["Defense"]);
        //     }
        // }

        return godotDict;
    }

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
            {
                Instance = this;
            }
    }
}


// internal class ItemDataClass
// {
//     public string ItemCategory { get; set; }
//     public int Defense { get; set; }
//     public int? AddHealth { get; set; }
//     public int? AddEnergy { get; set; }
//     public int StackSize { get; set; }
//     public string Description { get; set; }
//     public int? ItemAttack { get; set; }
//     public double? ItemSpeed { get; set; }
// }

