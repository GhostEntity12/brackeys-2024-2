using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestScrollOption : MonoBehaviour
{
	[SerializeField] QuestScroll parent;
	[SerializeField] TextMeshProUGUI descriptionText;
	[SerializeField] TextMeshProUGUI knightTimeText;
	[SerializeField] TextMeshProUGUI costsText;
	[SerializeField] TextMeshProUGUI rewardsText;
	[SerializeField] GameObject cross;

	Button button;
	QuestOption option;
	PointOfInterest location;

	private void Awake()
	{
		button = GetComponent<Button>();
	}

	public void SetValues(QuestOption option, PointOfInterest location)
	{
		this.option = option;
		this.location = location;
		if (option == null)
		{
			gameObject.SetActive(false);
			return;
		}

		gameObject.SetActive(true);
		descriptionText.text = option.description;
		knightTimeText.text = $"<sprite=\"Knight\" index=0>{option.knights}  <sprite=\"Time\" index=0>{option.duration:F1}s";
		costsText.text = "<font=\"EagleLake-Regular SDF\">Cost</font>\n" + ConcatResourceCosts(option.costs);
		rewardsText.text = "<font=\"EagleLake-Regular SDF\">Reward</font>\n" + ConcatResourceCosts(option.rewards);
		bool canUse = EvaluatePossible();
		button.interactable = canUse;
		cross.SetActive(!canUse);
	}

	private bool EvaluatePossible()
	{
		if (GameManager.Instance.KnightManager.AvailableKnights < option.knights) return false;

		foreach (ResourceCost cost in option.costs)
		{
			Debug.Log($"{cost.resource}");
			Debug.Log($"Needs {cost.quantity}");
			Debug.Log($"Has {GameManager.Instance.GetResourceQuantity(cost.resource)}");
			if (GameManager.Instance.GetResourceQuantity(cost.resource) < -cost.quantity)
			{
				return false;
			}
		}
		return true;
	}

	public void Select()
	{
		// Immediately remove the resource
		foreach (ResourceCost cost in option.costs)
		{
			GameManager.Instance.AddResource(cost);
		}
		parent.CloseScroll();
		GameManager.Instance.QuestOptionSelection(option, location);
	}

	static string ConcatResourceCosts(List<ResourceCost> costs)
	{
		string[] costStrings = new string[costs.Count];
		for (int i = 0; i < costs.Count; i++)
		{
			costStrings[i] = ($"<sprite=\"{costs[i].resource}\" index=0>{costs[i].quantity}");

		}
		return string.Join("\n", costStrings);
	}
}
