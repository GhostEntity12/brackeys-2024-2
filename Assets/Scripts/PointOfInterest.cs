using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(EventTrigger), typeof(BoxCollider))]
public class PointOfInterest : MonoBehaviour
{
	[SerializeField] private List<string> questIDs = new();

	private List<Quest> calmQuests = new();
	private List<Quest> stormQuests = new();

	public Quest ActiveQuest { get; private set; }

	private float questDespawnTimer = 30;
	private float timer;
	private EventTrigger eventTrigger;
	[SerializeField] SpriteRenderer flag;
	[SerializeField] List<LineRenderer> linesToPoint;
	List<Vector3> path;

	private void Awake()
	{
		eventTrigger = GetComponent<EventTrigger>();
	}

	public Vector2 Location { get; private set; }

	// Start is called before the first frame update
	void Start()
	{
		AddQuests(questIDs);
		Location = new(transform.position.x, transform.position.z);


		EventTrigger.Entry entry = new()
		{
			eventID = EventTriggerType.PointerClick
		};
		entry.callback.AddListener((data) =>
		{
			OnClicked((PointerEventData)data);
		});
		eventTrigger.triggers.Add(entry);
		path = CalculatePath();
	}

	private void Update()
	{
		if (ActiveQuest == null) return;

		timer -= Time.deltaTime;
		if (timer > 0) return;

		ClearQuest();
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

	public void SetQuest(Quest q)
	{
		// Set the active quest
		ActiveQuest = q;
		// Set the quest despawn timer
		timer = questDespawnTimer;
		// Set the flag
		flag.enabled = true;
	}

	public void ClearQuest()
	{
		ActiveQuest = null;
		flag.enabled = false;
	}


	public bool TryGetQuest(bool isStorm, out Quest quest)
	{
		// A bit jank but works
		List<Quest> listToUse = isStorm ? stormQuests : calmQuests;
		quest = null;
		if (listToUse.Count == 0 || ActiveQuest != null) return false;
		else
		{
			quest = listToUse[0];
			listToUse.RemoveAt(0);
			return true;
		}
	}

	public void OnClicked(PointerEventData data)
	{
		if (ActiveQuest != null)
		{
			GameManager.Instance.QuestScroll.SetQuest(ActiveQuest, this);
			GameManager.Instance.QuestScroll.OpenScroll(data.position);
		}

	}
	public List<Vector3> CalculatePath()
	{
		List<Vector3> points = new();
		foreach (LineRenderer line in linesToPoint)
		{
			Vector3[] positions = new Vector3[line.positionCount];
			line.GetPositions(positions);
			points.AddRange(positions);
		}

		return new(points.Distinct());
	}

	public Queue<Vector3> PathTo() => new(path);

	public Queue<Vector3> PathFrom()
	{
		List<Vector3> copy = new(path);
		copy.Reverse();
		return new(copy);
	}
}
