using GameResources;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
	public InputActions InputActions { get; private set; }
	[field: SerializeField] public QuestScroll QuestScroll { get; private set; }
	[field: SerializeField] public Fade Fade { get; private set; }

	private int gold = 0;
	private int wood = 0;
	private int medicine = 0;
	private int food = 0;
	private int people = 0;

	private bool isStorm;

	private List<Quest> allQuests = new();

	readonly string path = $"{Application.dataPath}/Resources/";
	readonly string fileName = "quest.json";

	[SerializeField] Clock clock;
	[SerializeField] PointOfInterest[] pointsOfInterest;

	[SerializeField] Vector2 timerBounds;
	[SerializeField] float stormQuestFrequencyModifier = 2;
	float timer = 5;


	protected override void Awake()
	{
		base.Awake();
		string jsonRaw = File.ReadAllText($"{path}{fileName}");
		Debug.Log("Quest file found, reading.");
		allQuests = ((QuestData)JsonUtility.FromJson(jsonRaw, typeof(QuestData))).quests;
		Debug.Log($"Loaded {allQuests.Count} from file.");

		pointsOfInterest = FindObjectsOfType<PointOfInterest>();

		clock.OnStormStart += (_, _) => isStorm = true;
		clock.OnStormEnd += (_, _) => isStorm = false;

		InputActions = new();
	}

	private void Update()
	{
		EventsUpdate();
	}

	private void EventsUpdate()
	{
		timer -= Time.deltaTime;
		if (timer > 0) return;

		// Timer hit zero, attempt to spawn a quest
		// Reset timer
		timer = Random.Range(timerBounds.x, timerBounds.y);
		if (isStorm) timer /= stormQuestFrequencyModifier;

		PointOfInterest poi = pointsOfInterest[Random.Range(0, pointsOfInterest.Length)];

		if (!poi.TryGetQuest(isStorm, out Quest q))
		{
			// Chosen PoI has no quests or already has a quest. Halve the timer
			timer *= 0.5f;
			return;
		}

		poi.SetQuest(q);
	}

	public bool GetQuestByID(string id, out Quest quest)
	{
		quest = allQuests.Find(x => x.id == id);
		return quest != null;
	}

	public int GetResourceQuantity(Resource resource) => resource switch
	{
		Resource.Gold => gold,
		Resource.Wood => wood,
		Resource.Medicine => medicine,
		Resource.Food => food,
		Resource.People => people,
		_ => throw new System.NotImplementedException(),
	};

	public int AddResource(ResourceCost resourceCost)
	{
		switch (resourceCost.resource)
		{
			case Resource.Gold:
				gold += resourceCost.quantity;
				return gold;
			case Resource.Wood:
				wood += resourceCost.quantity;
				return wood;
			case Resource.Medicine:
				medicine += resourceCost.quantity;
				return medicine;
			case Resource.Food:
				food += resourceCost.quantity;
				return food;
			case Resource.People:
				people += resourceCost.quantity;
				return people;
			default:
				throw new System.NotImplementedException();
		}
	}
}
