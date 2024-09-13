using TMPro;
using UnityEngine;

public class QuestScroll : MonoBehaviour
{
	[SerializeField] RectTransform paper;
	[SerializeField] TextMeshProUGUI titleText;
	[SerializeField] TextMeshProUGUI descriptionText;
	[SerializeField] QuestScrollOption[] options;
	public void SetQuest(Quest quest)
	{
		titleText.text = quest.title;
		descriptionText.text = quest.description;
		for (int i = 0; i < options.Length; i++)
		{
			if (i < quest.options.Count)
			{
				options[i].SetValues(quest.options[i]);
			}
		}
	}

	[ContextMenu("Open")]
	public void OpenScroll()
	{
		LeanTween.moveX(paper, 0, 0.5f).setEaseOutQuad();
	}
	[ContextMenu("Close")]
	public void CloseScroll()
	{
		LeanTween.moveX(paper, -680, 0.5f).setEaseOutQuad();
	}
}
