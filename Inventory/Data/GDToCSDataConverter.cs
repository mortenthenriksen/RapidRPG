using Godot;

namespace Game.Autoload;

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


