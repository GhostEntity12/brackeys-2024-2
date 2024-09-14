using System.Collections.Generic;
using UnityEngine;

public class PointOfInterest : MonoBehaviour
{
	[SerializeField] private List<string> calmQuestIDs = new();
	[SerializeField] private List<string> stormQuestIDs = new();

	private List<Quest> calmQuests = new();
	private List<Quest> stormQuests = new();

	public Quest ActiveQuest { get; private set; }

	public Vector2 Location { get; private set; }

	// Start is called before the first frame update
	void Start()
	{
		List<Quest> tempList = new();
		foreach (string id in calmQuestIDs)
		{
			if (GameManager.Instance.GetQuestByID(id, out Quest q))
			{
				tempList.Add(q);
			}
			else
			{
				Debug.LogError($"Quest with {id} could not be found");
			}
		}
		tempList.Shuffle();
		calmQuests = new(tempList);
		tempList.Clear();
		foreach (string id in stormQuestIDs)
		{
			if (GameManager.Instance.GetQuestByID(id, out Quest q))
			{
				tempList.Add(q);
			}
			else
			{
				Debug.LogError($"Quest with {id} could not be found");
			}
		}
		tempList.Shuffle();
		stormQuests = new(tempList);
		Location = new(transform.position.x, transform.position.z);
	}

	public void AddQuests(List<string> questIDs)
	{
		foreach (string id in questIDs)
		{
			if (GameManager.Instance.GetQuestByID(id, out Quest q))
			{
				if (q.id.StartsWith("calm", System.StringComparison.OrdinalIgnoreCase))
				{
					calmQuests.Add(q);
				}
				else if (q.id.StartsWith("storm", System.StringComparison.OrdinalIgnoreCase))
				{
					stormQuests.Add(q);
				}
			}
		}
		calmQuests.Shuffle();
		stormQuests.Shuffle();
	}

	public void SetQuest(Quest q) => ActiveQuest = q;


	public bool TryGetQuest(bool isStorm, out Quest quest)
	{
		// A bit jank but works
		List<Quest> listToUse = isStorm ? stormQuests : calmQuests;
		quest = null;
		if (listToUse.Count == 0) return false;
		else
		{
			quest = listToUse[0];
			listToUse.RemoveAt(0);
			return true;
		}
	}

	public int AvailableQuests(bool isStorm) => isStorm ? stormQuests.Count : calmQuests.Count;
}
