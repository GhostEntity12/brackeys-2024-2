using System.Data.Common;
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
	SerializedObject workingSerializedObject;

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

		// Load styles
		var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/QuestCreator.uss");
		root.styleSheets.Add(styleSheet);
		Label header = new("Editing Quest");
		header.AddToClassList("header1");
		Button nextButton = new(NextTestItem) { text = "Next" };
		BindableElement dataBind = new() { bindingPath = "data" };
		TextField id = new("ID") { bindingPath = "id" };
		TextField title = new("Title") { bindingPath = "title" };
		TextField description = new("Description") { bindingPath = "description" };
		GroupBox optionsBox = new();
		Label optionsLabel = new("Options");
		optionsLabel.AddToClassList("header2");
		ListView options = new()
		{
			name = "questOptionsList",
			bindingPath = "questOptions",
			virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
			reorderable = true,
			reorderMode = ListViewReorderMode.Animated,
			makeItem = () =>
			{
				Foldout optionHeader = new() { text = "Option" };
				BindableElement optionBind = new() { bindingPath = "questOptions" };
				TextField description = new("Description") { bindingPath = "description" };
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

		root.Add(header);
		root.Add(nextButton);
		root.Add(dataBind);
		dataBind.Add(id);
		dataBind.Add(title);
		dataBind.Add(description);
		dataBind.Add(optionsBox);
		optionsBox.Add(optionsLabel);
		optionsBox.Add(options);

		Button writeButton = new(Save) { text = "Write to Disk" };

		root.Add(writeButton);

		SetWorkingSerializedObject(data.list[currentIndex]);
	}

	private void SetWorkingSerializedObject(EditorTest data)
	{
		ScriptableEditorTest obj = CreateInstance<ScriptableEditorTest>();
		obj.data = data;
		workingSerializedObject = new SerializedObject(obj);
		rootVisualElement.Bind(workingSerializedObject);
	}

	void NextTestItem()
	{
		currentIndex += 1;
		if (currentIndex >= data.list.Count)
		{
			currentIndex = 0;
		}
		SetWorkingSerializedObject(data.list[currentIndex]);
	}

	void AddTestItem()
	{
		string id = (rootVisualElement.Q("addID") as TextField).text;
		string title = (rootVisualElement.Q("addTitle") as TextField).text;
		string description = (rootVisualElement.Q("addDescription") as TextField).text;
		data.list.Add(new EditorTest(id, title, description));

		currentIndex = data.list.Count - 1;
		SetWorkingSerializedObject(data.list[currentIndex]);
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
