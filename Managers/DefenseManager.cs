
using Godot;

namespace Game.Manager;

[GlobalClass]

public partial class DefenseManager : Node
{

    
    public static DefenseManager Instance { get; private set; }





    public override void _Notification(int what)
    {
		if (what == NotificationSceneInstantiated) 
		{
			Instance = this;
		}
    }
}
