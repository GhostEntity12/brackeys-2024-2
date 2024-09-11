using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class QuestCreator : EditorWindow
{
	// Saving Quests to disk
	readonly string path = $"{Application.dataPath}/Resources/";
	readonly string fileName = "quest.json";
	private QuestData questData = new();

	private Quest workingQuest = null;

	private string editingID;

	//[SerializeField]
	//private VisualTreeAsset m_VisualTreeAsset = default;

	/// <summary>
	/// Function to open the Window from the toolbar
	/// </summary>
	[MenuItem("Tools/QuestCreator")]
	public static void ShowWindow()
	{
		QuestCreator wnd = GetWindow<QuestCreator>();
		wnd.titleContent = new GUIContent("QuestCreator");
	}

	public void CreateGUI()
	{
		// Load the Quests into memory
		ReadFile();
		// Each editor window contains a root VisualElement object
		VisualElement root = rootVisualElement;

		// Load styles
		var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/QuestCreator.uss");
		root.styleSheets.Add(styleSheet);

		// Create the splitView
		TwoPaneSplitView splitView = new(0, 300, TwoPaneSplitViewOrientation.Horizontal);
		root.Add(splitView);
		VisualElement leftPane = new();
		VisualElement rightPane = new() { name = "editPanel" };
		splitView.Add(leftPane);
		splitView.Add(rightPane);

		// Add the box to create new Quests
		InstantiateCreatePanel(leftPane);
		InstantiateLoadPanel(leftPane);


		//InstantiateEditPanel(null);
		// Instantiate UXML
		//VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
		//root.Add(labelFromUXML);
	}

	private void InstantiateCreatePanel(VisualElement parent)
	{
		GroupBox createPanel = new();

		Label header = new("Create a Quest");
		header.AddToClassList("headerText");

		TextField idEntry = new("Quest ID") { name = "createQuestID" };

		HelpBox statusBox = new("Create Status", HelpBoxMessageType.Info) { name = "createStatus" };
		statusBox.style.display = DisplayStyle.None;

		Button createButton = new(CreateQuest) { text = "Create" };

		parent.Add(createPanel);
		createPanel.Add(header);
		createPanel.Add(idEntry);
		createPanel.Add(statusBox);
		createPanel.Add(createButton);
	}

	private void InstantiateLoadPanel(VisualElement parent)
	{
		GroupBox loadPanel = new();

		Label header = new("Load a Quest");
		header.AddToClassList("headerText");

		ListView questListDisplay = new() { name = "questListDisplay" };

		// Template for each item in the list
		questListDisplay.makeItem = MakeLoadQuestEntry;
		questListDisplay.bindItem = BindLoadQuestEntry;
		questListDisplay.itemsSource = questData.quests;

		questListDisplay.selectionChanged += OnSelectNewQuest;

		parent.Add(loadPanel);
		loadPanel.Add(header);
		loadPanel.Add(questListDisplay);
	}

	private void InstantiateEditPanel(Quest quest)
	{
		// Generate Fields
		VisualElement root = rootVisualElement.Q("editPanel");
		root.Clear();
		GroupBox questBox = new();

		root.Add(questBox);

		Label questInfoHeader = new($"Editing Quest: {quest.id}") { name = "questEditHeader" };
		questInfoHeader.AddToClassList("headerText");

		TextField questID = new("ID") { name = "questID" };
		TextField questTitle = new("Title") { name = "questTitle" };
		TextField questDescription = new("Description")
		{
			name = "questDescription",
			multiline = true
		};
		questBox.Add(questInfoHeader);
		questBox.Add(questID);
		questBox.Add(questTitle);
		questBox.Add(questDescription);

		var sData = CreateInstance<ScriptableQuest>();

		// Loop through properties and create one field for each top level property.
		//SerializedProperty property = serializedQuest.GetIterator();

		//property.NextVisible(true);
		//property.NextVisible(true);
		//do
		//{
		//	Debug.Log(property.name);
		//	// Create the UIElements PropertyField.
		//	var uieDefaultProperty = new PropertyField(property);

		//	Debug.Log(uieDefaultProperty.hierarchy);

		//	foreach (var child in uieDefaultProperty.Children())
		//	{
		//		Debug.Log(child);
		//	}

		//	root.Add(uieDefaultProperty);
		//}
		//while (property.NextVisible(false));
		// Also

		//property = serializedQuest.FindProperty("id");
		//root.Add(new PropertyField(property));

		// Bind the entire window.
		//root.Bind(serializedQuest);
		//sData.data = workingQuest;
		//var serializedQuest = new SerializedObject(sData);

		HelpBox status = new() { name = "editStatus" };
		status.style.display = DisplayStyle.None;
		root.Add(status);
		Button saveButton = new(SaveQuest) { text = "Save" };
		root.Add(saveButton);
		Button deleteButton = new(DeleteQuest) { text = "Delete" };
		root.Add(deleteButton);

		editingID = quest.id;

		workingQuest = quest;
		Debug.Log($"Loading quest {quest.id}");
		//(root.Q("questID") as TextField).value = quest.id;
		//(root.Q("questTitle") as TextField).value = quest.title;
		//(root.Q("questDescription") as TextField).value = quest.description;

		//root.Q("ad").parent

	}

	private void OnSelectNewQuest(IEnumerable<object> selectedItem) => InstantiateEditPanel(questData.quests[(rootVisualElement.Q("questListDisplay") as ListView).selectedIndex]);

	VisualElement MakeLoadQuestEntry()
	{
		VisualElement parent = new Label();
		return parent;
	}

	void BindLoadQuestEntry(VisualElement item, int index)
	{
		(item as Label).text = questData.quests[index].id;
	}

	void CreateQuest()
	{
		VisualElement root = rootVisualElement;

		// Get the TextField value
		string query = (root.Q("createQuestID") as TextField).value.Trim();

		// Get reference to the HelpBox to tell the user what happened
		HelpBox status = root.Q("createStatus") as HelpBox;
		status.style.display = DisplayStyle.Flex;

		// Query is empty
		if (query.Trim() == string.Empty)
		{
			status.messageType = HelpBoxMessageType.Warning;
			status.text = "Cannot create a quest without an ID";
			return;
		}

		// Search for quests with the same ID as the query
		if (questData.quests.Any(q => q.id == query))
		{
			status.messageType = HelpBoxMessageType.Error;
			status.text = $"A quest with ID {query} already exists";
			return;
		}

		// No issues found
		status.text = $"Quest {query} created successfully";
		status.messageType = HelpBoxMessageType.Info;

		// New quest created
		ScriptableQuest newQuest = CreateInstance<ScriptableQuest>();
		// Set the ID
		newQuest.data.id = query;
		// Load the quest's data in the right panel
		InstantiateEditPanel(newQuest.data);
		// Write the quest to disk
		questData.quests.Add(newQuest.data);
		WriteFile();

		// Update the list of quests in the left panel
		(root.Q("questListDisplay") as ListView).Rebuild();
	}

	void SaveQuest()
	{
		VisualElement root = rootVisualElement;
		string newID = (root.Q("questID") as TextField).value.Trim();
		HelpBox status = (root.Q("editStatus") as HelpBox);
		status.style.display = DisplayStyle.Flex;

		// If the editingID has changed, check that it's not going to overwrite an existing quest
		if (editingID != newID && questData.quests.Any(q => q.id == newID))
		{
			status.text = $"A Quest with the ID {newID} already exists";
			status.messageType = HelpBoxMessageType.Error;
			return;
		}

		status.text = $"Quest {newID} saved successfully";
		status.messageType = HelpBoxMessageType.Info;

		// Debug 
		Debug.Log($"Overwriting {editingID} with new values from {newID}");
		// Update the values of workingQuest
		// TODO

		// Replace the data of the quest with the same editingID with workingQuest
		questData.quests[questData.quests.IndexOf(questData.quests.First(q => q.id == editingID))] = workingQuest;
		editingID = newID;
		WriteFile();
	}

	void DeleteQuest()
	{
		questData.quests.Remove(questData.quests.First(q => q.id == editingID));
		rootVisualElement.Q("editPanel").Clear();
		(rootVisualElement.Q("questListDisplay") as ListView).Rebuild();
		WriteFile();
	}

	private string WriteFile()
	{
		Debug.Log($"Writing all {questData.quests.Count} quests to disk at {path}{fileName}");
		string jsonRaw = JsonUtility.ToJson(questData);
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
			Debug.Log("Quest file found, reading.");
		}
		catch (FileNotFoundException)
		{
			Debug.Log("No quest file found, creating new file.");
			jsonRaw = WriteFile();
		}
		questData = (QuestData)JsonUtility.FromJson(jsonRaw, typeof(QuestData));
		Debug.Log($"Loaded {questData.quests.Count} quests into memory.");
	}
}

public class QuestData
{
	public List<Quest> quests = new();
}