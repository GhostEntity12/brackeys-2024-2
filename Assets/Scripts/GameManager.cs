using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
	public enum Resource
	{
		Gold,
		Wood,
		Medicine,
		Food,
		People
	}
	public InputActions InputActions { get; private set; } 

	private int gold = 0;
	private int wood = 0;
	private int medicine = 0;
	private int food = 0;
	private int people = 0;

	private List<Quest> allQuests = new();

	readonly string path = $"{Application.dataPath}/Resources/";
	readonly string fileName = "quest.json";

	[SerializeField] Clock clock;
	//[SerializeField] QuestBanner banner;
	[SerializeField] PointOfInterest[] pointsOfInterest;

	protected override void Awake()
	{
		base.Awake();
		string jsonRaw = File.ReadAllText($"{path}{fileName}");
		Debug.Log("Quest file found, reading.");
		allQuests = ((QuestData)JsonUtility.FromJson(jsonRaw, typeof(QuestData))).quests;
		Debug.Log($"Loaded {allQuests.Count} from file.");

		pointsOfInterest = FindObjectsOfType<PointOfInterest>();
		InputActions = new();
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

	public int AddResource(Resource resource, int quantity)
	{
		switch (resource)
		{
			case Resource.Gold:
				gold += quantity;
				return gold;
			case Resource.Wood:
				wood += quantity;
				return wood;
			case Resource.Medicine:
				medicine += quantity;
				return medicine;
			case Resource.Food:
				food += quantity;
				return food;
			case Resource.People:
				people += quantity;
				return people;
			default:
				throw new System.NotImplementedException();
		}
	}

	public Quest GetFirstQuest() => allQuests[0];
}
