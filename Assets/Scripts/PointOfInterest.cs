using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointOfInterest : MonoBehaviour
{
	[SerializeField] List<string> calmQuestIDs = new();
	[SerializeField] List<string> stormQuestIDs = new();
	public int CalmQuestCount => calmQuests.Count;
	public int StormQuestCount => stormQuests.Count;

	readonly List<Quest> calmQuests = new();
	readonly List<Quest> stormQuests = new();


	// Start is called before the first frame update
	void Start()
	{
		foreach (string id in calmQuestIDs)
		{
			if (GameManager.Instance.GetQuestByID(id, out Quest q))
			{
				calmQuests.Add(q);
			}
			else
			{
				Debug.LogError($"Quest with {id} could not be found");
			}
		}
		foreach (string id in stormQuestIDs)
		{
			if (GameManager.Instance.GetQuestByID(id, out Quest q))
			{
				stormQuests.Add(q);
			}
			else
			{
				Debug.LogError($"Quest with {id} could not be found");
			}
		}
	}

	public Quest GetStormQuest() => stormQuests[Random.Range(0, StormQuestCount)];
	public Quest GetCalmQuest() => stormQuests[Random.Range(0, CalmQuestCount)];
}
