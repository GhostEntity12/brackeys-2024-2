using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class QuestCreationEditor : EditorWindow
{
	// Saving Quests to disk
	readonly string path = $"{Application.dataPath}/Resources/";
	readonly string fileName = "quest.json";

	private QuestData quests = new();

	SerializedObject workingSerializedObject;

	//[SerializeField]
	//private VisualTreeAsset m_VisualTreeAsset = default;

	/// <summary>
	/// Function to open the Window from the toolbar
	/// </summary>
	[MenuItem("Tools/QuestCreator")]
	public static void ShowWindow()
	{
		QuestCreationEditor wnd = GetWindow<QuestCreationEditor>();
		wnd.titleContent = new GUIContent("QuestCreator");
	}

	public void OnEnable()
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
		ScrollView rightPane = new() { name = "editPanel" };
		splitView.Add(leftPane);
		splitView.Add(rightPane);

		// Add the box to create new Quests
		InstantiateCreatePanel(leftPane);
		InstantiateLoadPanel(leftPane);

		// Add the save button
		Button writeButton = new(SaveAll) { text = "Write to Disk", name = "saveButton" };
		leftPane.Add(writeButton);
	}

	private void InstantiateCreatePanel(VisualElement parent)
	{
		GroupBox createPanel = new();

		Label header = new("Create a Quest");
		header.AddToClassList("header1");

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
		GroupBox loadPanel = new() { name = "loadPanel" };

		Label header = new("Load a Quest");
		header.AddToClassList("header1");

		ListView questListDisplay = new()
		{
			name = "questListDisplay",
			makeItem = () => new Label(),
			bindItem = (element, index) => (element as Label).text = quests.quests[index].id,
			itemsSource = quests.quests,
			reorderable = true,
		};

		questListDisplay.selectionChanged += OnSelectNewQuest;

		parent.Add(loadPanel);
		loadPanel.Add(header);
		loadPanel.Add(questListDisplay);
	}

	private void InstantiateEditPanel(Quest quest)
	{
		// Generate Fields
		// Clear the root element so the new panel can be created
		VisualElement root = rootVisualElement.Q("editPanel");
		root.Clear();

		// Parent box
		GroupBox questBox = new() { name = "questBox" };
		root.Add(questBox);

		Label questInfoHeader = new($"Editing Quest: {quest.id}") { name = "questEditHeader" };
		questInfoHeader.AddToClassList("header1");
		BindableElement dataBind = new() { bindingPath = "data" };
		TextField id = new("ID") { bindingPath = "id" };
		TextField title = new("Title") { bindingPath = "title" };
		TextField description = new("Description") { bindingPath = "description", multiline = true };
		GroupBox optionsBox = new();
		Label optionsLabel = new("Options");
		optionsLabel.AddToClassList("header2");
		ListView options = new()
		{
			name = "questOptionsList",
			bindingPath = "options",
			virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
			reorderable = true,
			reorderMode = ListViewReorderMode.Animated,
			makeItem = () =>
			{
				Foldout optionHeader = new() { text = "Option" };
				BindableElement optionBind = new() { bindingPath = "options" };
				TextField description = new("Description") { bindingPath = "description", multiline = true };
				IntegerField knights = new("Knights") { bindingPath = "knights" };
				FloatField duration = new("Duration") { bindingPath = "duration" };
				VisualElement costsAndRewardsContainer = new() { name = "costsAndRewardsContainer" };
				GroupBox costsBox = new();
				costsBox.AddToClassList("costsAndRewards");
				Label costLabel = new("Costs");
				costLabel.AddToClassList("header3");
				ListView costs = new()
				{
					name = "optionCostsList",
					bindingPath = "costs",
					virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
					reorderable = true,
					reorderMode = ListViewReorderMode.Animated,
					makeItem = () =>
					{
						VisualElement costHeader = new();
						BindableElement costBind = new() { bindingPath = "costs" };
						costBind.AddToClassList("resourceCostParent");
						EnumField costResource = new() { bindingPath = "resource" };
						costResource.AddToClassList("resourceCostComponent");
						IntegerField costQuantity = new() { bindingPath = "quantity" };
						costQuantity.AddToClassList("resourceCostComponent");
						costHeader.Add(costBind);
						costBind.Add(costResource);
						costBind.Add(costQuantity);
						return costHeader;
					}
				};
				GroupBox rewardsBox = new();
				rewardsBox.AddToClassList("costsAndRewards");
				Label rewardsLabel = new("Rewards");
				rewardsLabel.AddToClassList("header3");
				ListView rewards = new()
				{
					name = "optionRewardsList",
					bindingPath = "rewards",
					virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
					reorderable = true,
					reorderMode = ListViewReorderMode.Animated,
					makeItem = () =>
					{
						VisualElement rewardHeader = new();
						BindableElement rewardBind = new() { bindingPath = "rewards" };
						rewardBind.AddToClassList("resourceCostParent");
						EnumField rewardResource = new() { bindingPath = "resource" };
						rewardResource.AddToClassList("resourceCostComponent");
						IntegerField rewardQuantity = new() { bindingPath = "quantity" };
						rewardQuantity.AddToClassList("resourceCostComponent");
						rewardHeader.Add(rewardBind);
						rewardBind.Add(rewardResource);
						rewardBind.Add(rewardQuantity);
						return rewardHeader;
					}
				};
				optionHeader.Add(optionBind);
				optionBind.Add(description);
				optionBind.Add(knights);
				optionBind.Add(duration);
				optionBind.Add(costsAndRewardsContainer);
				costsAndRewardsContainer.Add(costsBox);
				costsBox.Add(costLabel);
				costsBox.Add(costs);
				costsAndRewardsContainer.Add(rewardsBox);
				rewardsBox.Add(rewardsLabel);
				rewardsBox.Add(rewards);
				return optionHeader;
			}
		};
		Button deleteButton = new(DeleteQuest) { text = "Delete Quest", name = "deleteButton" };
		questBox.Add(questInfoHeader);
		questBox.Add(dataBind);
		dataBind.Add(id);
		dataBind.Add(title);
		dataBind.Add(description);
		dataBind.Add(optionsBox);
		optionsBox.Add(optionsLabel);
		optionsBox.Add(options);
		root.Add(deleteButton);

		SetWorkingSerializedObject(quest);

	}

	private void SetWorkingSerializedObject(Quest data)
	{
		// Create the scriptable quest
		ScriptableQuest obj = CreateInstance<ScriptableQuest>();
		// Assign the data
		obj.data = data;
		// Set the workingSerializedObject
		workingSerializedObject = new SerializedObject(obj);
		// Bind the object
		rootVisualElement.Q("editPanel").Bind(workingSerializedObject);
	}

	private void OnSelectNewQuest(IEnumerable<object> selectedItem)
	{
		int index = (rootVisualElement.Q("questListDisplay") as ListView).selectedIndex;
		if (index == -1) return;
		Quest selectedQuest = quests.quests[index];
		InstantiateEditPanel(selectedQuest);
	}

	void CreateQuest()
	{
		VisualElement root = rootVisualElement;

		// Get the TextField
		TextField id = (root.Q("createQuestID") as TextField);

		// Get the TextField value
		string query = id.value.Trim();

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
		if (quests.quests.Any(q => q.id == query))
		{
			status.messageType = HelpBoxMessageType.Error;
			status.text = $"A quest with ID '{query}' already exists";
			return;
		}

		// No issues found
		status.text = $"Quest '{query}' created successfully";
		status.messageType = HelpBoxMessageType.Info;
		id.value = string.Empty;

		// New quest created
		Quest newQuest = new()
		{
			// Set the ID
			id = query
		};
		// Load the quest's data in the right panel
		InstantiateEditPanel(newQuest);
		// Write the quest to disk
		quests.quests.Add(newQuest);
		WriteFile();

		// Update the list of quests in the left panel
		ListView questList = (root.Q("questListDisplay") as ListView);
		questList.Rebuild();
		questList.selectedIndex = quests.quests.Count - 1;
	}

	void SaveAll()
	{
		WriteFile();

		// Update the list of quests in the left panel
		(rootVisualElement.Q("questListDisplay") as ListView).Rebuild();
	}

	void DeleteQuest()
	{
		ListView questList = (rootVisualElement.Q("questListDisplay") as ListView);
		quests.quests.RemoveAt(questList.selectedIndex);
		rootVisualElement.Q("editPanel").Clear();
		questList.Rebuild();
		questList.ClearSelection();
		WriteFile();
	}

	private string WriteFile()
	{
		Debug.Log($"Writing all {quests.quests.Count} quests to disk at {path}{fileName}");
		string jsonRaw = JsonUtility.ToJson(quests);
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
		quests = (QuestData)JsonUtility.FromJson(jsonRaw, typeof(QuestData));
		Debug.Log($"Loaded {quests.quests.Count} quests into memory.");
	}
}

public class QuestData
{
	public List<Quest> quests = new();
}