using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestScrollOption : MonoBehaviour
{
	[SerializeField] Transform root;
	[SerializeField] TextMeshProUGUI descriptionText;
	[SerializeField] TextMeshProUGUI knightTimeText;
	[SerializeField] TextMeshProUGUI costsText;
	[SerializeField] TextMeshProUGUI rewardsText;

	QuestOption option;
	public void SetValues(QuestOption option)
	{
		this.option = option;
		if (option == null)
		{
			root.gameObject.SetActive(false);
			return;
		}

		root.gameObject.SetActive(true);
		descriptionText.text = option.description;
		knightTimeText.text = $"<sprite=\"Knight\" index=0>{option.knights}  <sprite=\"Time\" index=0>{option.duration:F1}s";
		costsText.text = "<font=\"EagleLake-Regular SDF\">Cost</font>\n" + ConcatResourceCosts(option.costs); 
		rewardsText.text = "<font=\"EagleLake-Regular SDF\">Reward</font>\n" + ConcatResourceCosts(option.rewards); 
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
