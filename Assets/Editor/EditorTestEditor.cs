using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class EditorTestEditor : EditorWindow
{
	readonly string path = $"{Application.dataPath}/Resources/";
	readonly string fileName = "data.json";
	EditorTestData data;
	int currentIndex = 0;

	[MenuItem("Tools/EditorTest")]
	public static void ShowWindow()
	{
		EditorTestEditor wnd = GetWindow<EditorTestEditor>();
		wnd.titleContent = new GUIContent("EditorTest");
	}

	public void OnEnable()
	{
		ReadFile();
		VisualElement root = rootVisualElement;

		TextField id = new("ID") { bindingPath = "data.id" };
		TextField title = new("Title") { bindingPath = "data.title" };
		TextField description = new("Description") { bindingPath = "data.description" };
		Button nextButton = new(NextTestItem) { text = "Next" };

		root.Add(id);
		root.Add(title);
		root.Add(description);
		root.Add(nextButton);

		TextField newId = new("ID") { name = "addID" };
		TextField newTitle = new("Title") { name = "addTitle" };
		TextField newDescription = new("Description") { name = "addDescription" };
		Button addButton = new(AddTestItem) { text = "Add" };

		root.Add(newId);
		root.Add(newTitle);
		root.Add(newDescription);
		root.Add(addButton);

		Button writeButton = new(Save) { text = "Write to Disk" };

		root.Bind(CreateSerializedObject(data.list[0]));

		Debug.Log(data.list.Count);
	}

	SerializedObject CreateSerializedObject(EditorTest data)
	{
		ScriptableEditorTest obj = CreateInstance<ScriptableEditorTest>();
		obj.data = data;
		return new(obj);
	}

	void NextTestItem()
	{
		currentIndex += 1;
		if (currentIndex >= data.list.Count)
		{
			currentIndex = 0;
		}
		rootVisualElement.Bind(CreateSerializedObject(data.list[currentIndex]));
	}

	void AddTestItem()
	{
		string id = (rootVisualElement.Q("addID") as TextField).text;
		string title = (rootVisualElement.Q("addTitle") as TextField).text;
		string description = (rootVisualElement.Q("addDescription") as TextField).text;
		data.list.Add(new EditorTest(id, title, description));

		currentIndex = data.list.Count - 1;
		rootVisualElement.Bind(CreateSerializedObject(data.list[currentIndex]));
		Save();
	}

	void Save() => WriteFile();

	private string WriteFile()
	{
		Debug.Log($"Writing all {data.list.Count} test data to disk at {path}{fileName}");
		string jsonRaw = JsonUtility.ToJson(data);
		File.WriteAllText($"{path}{fileName}", jsonRaw);
		AssetDatabase.Refresh();
		return jsonRaw;
	}


	private void ReadFile()
	{
		string jsonRaw;
		try
		{
			jsonRaw = File.ReadAllText($"{path}{fileName}");
			Debug.Log("Test data file found, reading.");
		}
		catch (FileNotFoundException)
		{
			Debug.Log("No test data file found, creating new file.");
			CreateFile();
			jsonRaw = WriteFile();
		}
		data = (EditorTestData)JsonUtility.FromJson(jsonRaw, typeof(EditorTestData));
		Debug.Log($"Loaded {data.list.Count} quests into memory.");
	}

	private void CreateFile()
	{
		data = new();
		for (int i = 0; i < 5; i++)
		{
			data.list.Add(new(i));
		}
	}
}
