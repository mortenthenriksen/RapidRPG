using Godot;
using System;

public partial class MonsterSpawner : Node
{


	WeightedGroup<string> group = new WeightedGroup<string>(){
			{"object1", 50},
			{"object2", 25},
			{"object3", 25},
		};

}
