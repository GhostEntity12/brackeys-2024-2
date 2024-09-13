using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EditorTest
{
	public string id;
	public string title;
	public string description;

	public List<QuestOption> questOptions = new();

	public EditorTest(int i)
	{
		id = $"test ID {i}";
		title = $"test title {i}";
		description = $"test description {i}";

		questOptions.Add(new());
	}
	public EditorTest(string id, string title, string description)
	{
		this.id = id;
		this.title = title;
		this.description = description;

		questOptions.Add(new());
	}
}


public class ScriptableEditorTest : ScriptableObject
{
	public EditorTest data;
}

public class EditorTestData
{
	public List<EditorTest> list = new();
}