using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class QuestScroll : MonoBehaviour
{
	RectTransform parent;
	[SerializeField] RectTransform paper;
	[SerializeField] RectTransform root;
	[SerializeField] TextMeshProUGUI titleText;
	[SerializeField] TextMeshProUGUI descriptionText;
	[SerializeField] QuestScrollOption[] options;
	[SerializeField] CanvasGroup group;
	PointOfInterest eventLocation;

	InputActions actions;

	private void Awake()
	{
		parent = GetComponent<RectTransform>();
	}

	private void Start()
	{
		actions = GameManager.Instance.InputActions;
	}

	public void SetQuest(Quest quest, PointOfInterest location)
	{
		titleText.text = quest.title;
		descriptionText.text = quest.description;
		for (int i = 0; i < options.Length; i++)
		{
			if (i < quest.options.Count)
			{
				options[i].SetValues(quest.options[i], eventLocation);
			}
		}
		eventLocation = location;
	}

	public void OpenScroll(Vector3 startingPos)
	{
		parent.position = startingPos;
		LeanTween.move(parent, Vector3.zero, 0.2f).setEaseInOutCubic();
		LeanTween.scale(parent, Vector3.one, 0.3f).setEaseOutBack();
		LeanTween.moveX(paper, 0, 0.5f).setEaseInOutQuad().setDelay(0.45f);
		LeanTween.moveX(root, 0, 0.5f).setEaseInOutQuad().setDelay(0.45f);
		actions.MainGameplay.Movement.Disable();
		GameManager.Instance.Fade.FadeIn();
		group.interactable = true;
	}
	public void CloseScroll()
	{
		LeanTween.scale(parent, Vector3.zero, 0.3f).setEaseInBack().setDelay(0.35f);
		LeanTween.moveX(paper, -680, 0.5f).setEaseInOutQuad();
		LeanTween.moveX(root, 340, 0.5f).setEaseInOutQuad();
		actions.MainGameplay.Movement.Enable();
		GameManager.Instance.Fade.FadeOut();
		group.interactable = false;
	}
}

