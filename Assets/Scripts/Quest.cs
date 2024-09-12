using System.Collections.Generic;
using UnityEngine;

public class ScriptableQuest : ScriptableObject
{
	public Quest data = new();
}

[System.Serializable]
public class Quest
{
	public enum Resources
	{
		Gold,
		Wood,
		Medicine,
		Food,
		People
	}
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
	public Quest.Resources resource;
	public int quantity;
}