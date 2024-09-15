using System.Collections.Generic;
using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestData", menuName = "Assets/QuestData")]
public class ScriptableQuestList : ScriptableObject
{
	public List<Quest> quests;
	void Awake()
	{
		string path = $"{Application.dataPath}/Resources/";
		string fileName = "quest.json";
		string jsonRaw = File.ReadAllText($"{path}{fileName}");
		Debug.Log("Quest file found, reading.");
		quests = ((QuestData)JsonUtility.FromJson(jsonRaw, typeof(QuestData))).quests;
		Debug.Log($"Loaded {quests.Count} from file.");
	}
}