using Game.Autoload;
using Godot;
using Godot.Collections;

namespace Game.Manager;


public partial class EquippedItemsManager : Node
{
    public static EquippedItemsManager Instance { get; private set; }

    [Export]
    private GridContainer equippedSlots;

    private Array<string> equippedItems;
    private Dictionary itemDataJson;
    
    private float baseDefense;
    private float addedDamage;



    public override void _Ready()
    {
        equippedSlots.Connect("slot_interacted", Callable.From(OnSlotInteracted));
        itemDataJson = GetItemDataDictionary();
    }


    public override void _Process(double delta)
    {        
        // GetDefenceFromEquippedItems();
    }

    private void OnSlotInteracted()
    {
        GetDefenceFromEquippedItems();
        GetDamageFromEquippedItems();
    }

    private float GetDefenceFromEquippedItems()
    {
        baseDefense = 0;
        equippedItems = (Array<string>)equippedSlots.Call("get_equipped_items");
        foreach (var element in equippedItems)
        {
            if (GetItemDataDictionary().ContainsKey(element))
            {
                if (itemDataJson.ContainsKey(element))
                {
                    var valueDict = (Dictionary)itemDataJson[element];
                    if (valueDict.ContainsKey("Defense"))
                    {
                        baseDefense += (float)valueDict["Defense"];
                    }
                }
            }
        }
        return baseDefense;
    }

    private float GetDamageFromEquippedItems()
    {
        addedDamage = 0;
        equippedItems = (Array<string>)equippedSlots.Call("get_equipped_items");
        foreach (var element in equippedItems)
        {
            if (GetItemDataDictionary().ContainsKey(element))
            {
                if (itemDataJson.ContainsKey(element))
                {
                    var valueDict = (Dictionary)itemDataJson[element];
                    if (valueDict.ContainsKey("Attack"))
                    {
                        addedDamage += (float)valueDict["Attack"];
                    }
                }
            }
        }
        return addedDamage;
    }


    private Dictionary GetItemDataDictionary()
    {
        return GDToCSDataConverter.Instance.GetValuesDictionaries();
    }

    public float GetTotalDefence()
    {
        return baseDefense;
    }

    public float GetAddedDamage()
    {
        return addedDamage;
    }
    
    		
    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
        {
            Instance = this;
        }
    }
}

