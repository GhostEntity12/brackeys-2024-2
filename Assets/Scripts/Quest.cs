using System.Collections.Generic;
using UnityEngine;
using GameResources;

public class ScriptableQuest : ScriptableObject
{
	public Quest data = new();
}

[System.Serializable]
public class Quest
{
	public string id;
	public string title;
	public string description;
	public List<QuestOption> options = new();
}

[System.Serializable]
public class QuestOption
{
	public string description;
	public List<ResourceCost> costs = new();
	public List<ResourceCost> rewards = new();
	public int knights;
	public float duration;
	public string[] questIdsToAdd;
}


[System.Serializable]
public struct ResourceCost
{
	public Resource resource;
	public int quantity;
}

namespace GameResources
{
	public enum Resource
	{
		Gold,
		Wood,
		Medicine,
		Food,
		People
	}
}