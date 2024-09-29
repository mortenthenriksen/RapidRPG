
using Game.Autoload;
using Godot;
using Godot.Collections;

namespace Game.Manager;

[GlobalClass]

public partial class ToolTipManager : Node
{
    public static ToolTipManager Instance { get; private set; }

    [Export]
    private GridContainer equippedSlots;


    public override void _Ready()
    {
        // to connect to a GDScript that has an argument, has to be passed like this apparently :))
        equippedSlots.Connect("custom_mouse_entered", Callable.From((string itemName) => OnMouseEntered(itemName)));
    }
    private void OnMouseEntered(string itemName)
    {
        var itemData = GetItemDataDictionary();
        if (itemData.ContainsKey(itemName))
        {
            var itemInfo = itemData[itemName];
            GD.Print("Item Data: ", itemInfo);
        }
        else
        {
            GD.Print("Item not found: ", itemName);
        }
    }


    private Dictionary GetItemDataDictionary()
    {
        return GDToCSDataConverter.Instance.GetValuesDictionaries();
    }



    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
        {
            Instance = this;
        }
    }
}
