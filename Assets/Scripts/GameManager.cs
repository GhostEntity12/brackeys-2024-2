using GameResources;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
	public InputActions InputActions { get; private set; }
	[field: SerializeField] public QuestScroll QuestScroll { get; private set; }
	[field: SerializeField] public Fade Fade { get; private set; }
	public KnightManager KnightManager { get; private set; }

	[SerializeField] private int gold = 100;
	[SerializeField] private int wood = 20;
	[SerializeField] private int medicine = 20;
	[SerializeField] private int food = 20;
	[SerializeField] private int people = 0;

	private bool isStorm;

	private List<Quest> allQuests = new();

	//readonly string path = $"{Application.dataPath}/Resources/";
	//readonly string fileName = "quest.json";

	[SerializeField] Clock clock;
	[SerializeField] PointOfInterest[] pointsOfInterest;

	[SerializeField] Vector2 timerBounds;
	[SerializeField] float stormQuestFrequencyModifier = 2;
	float timer = 5;

	readonly List<OptionTimer> timers = new();

	[SerializeField] ScriptableQuestList data;

	[SerializeField] ResourceDisplay goldDisplay, foodDisplay, medicineDisplay, woodDisplay, knightDisplay, peopleDisplay;

	[SerializeField] EndScreen endScreen;

	protected override void Awake()
	{
		base.Awake();
		allQuests = data.quests;
		Debug.Log($"Loaded {allQuests.Count} from file.");
		//string jsonRaw = File.ReadAllText($"{path}{fileName}");
		//Debug.Log("Quest file found, reading.");
		//allQuests = ((QuestData)JsonUtility.FromJson(jsonRaw, typeof(QuestData))).quests;

		InputActions = new();

		KnightManager = GetComponent<KnightManager>();
		pointsOfInterest = FindObjectsOfType<PointOfInterest>();
	}

	private void Start()
	{
		clock.OnStormStart += (_, _) => { isStorm = true; CheckGameOver(); };
		clock.OnStormEnd += (_, _) => EndGame(true);
		KnightManager.SetKnights(15);
		goldDisplay.UpdateText(gold);
		foodDisplay.UpdateText(food);
		medicineDisplay.UpdateText(medicine);
		woodDisplay.UpdateText(wood);
		knightDisplay.UpdateText(KnightManager.AvailableKnights);
		peopleDisplay.UpdateText(people);
	}


	private void Update()
	{
		EventsUpdate();
		for (int i = timers.Count - 1; i >= 0; i--)
		{
			OptionTimer timer = timers[i];
			if (!timer.Tick(Time.deltaTime)) return;

			foreach (ResourceCost cost in timer.GetCosts())
			{
				AddResource(cost);
				timer.location.AddQuests(timer.selection.questIdsToAdd.ToList());
			}
			timers.Remove(timer);
		}
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
				goldDisplay.UpdateText(gold);
				return gold;
			case Resource.Wood:
				wood += resourceCost.quantity;
				woodDisplay.UpdateText(wood);
				return wood;
			case Resource.Medicine:
				medicine += resourceCost.quantity;
				medicineDisplay.UpdateText(medicine);
				return medicine;
			case Resource.Food:
				food += resourceCost.quantity;
				foodDisplay.UpdateText(food);
				return food;
			case Resource.People:
				people += resourceCost.quantity;
				peopleDisplay.UpdateText(people);
				if (isStorm && people == 0)
				{
					EndGame(false);
				}
				return people;
			default:
				throw new System.NotImplementedException();
		}
	}

	void CheckGameOver()
	{
		AddResource(new(Resource.People, 0));
	}

	void EndGame(bool wonGame)
	{
		endScreen.ShowEndScreen(wonGame, people);
		clock.Stop();
	}

	public void QuestOptionSelection(QuestOption selection, PointOfInterest location)
	{
		// Add timer for rewards
		timers.Add(new(selection, location));
		KnightManager.Dispatch(selection.knights, location, selection.duration);
	}

	public void UpdateKnightDisplay(int amount)
	{
		knightDisplay.UpdateText(amount);
	}
}

[System.Serializable]
class OptionTimer
{
	public QuestOption selection;
	public PointOfInterest location;
	readonly List<ResourceCost> costList;
	float timer;

	public OptionTimer(QuestOption option, PointOfInterest location)
	{
		this.selection = option;
		this.costList = option.rewards;
		this.timer = option.duration;
		this.location = location;
	}
	public List<ResourceCost> GetCosts() => costList;

	public bool Tick(float time)
	{
		timer -= time;
		return timer < 0;
	}
}