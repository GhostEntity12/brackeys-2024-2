using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndScreen : MonoBehaviour
{
    string successTitle = "Your Kingdom has Survived the Storm";
    string failTitle = "Your Kingdom has Fallen into Ruin";
    string successBody = "{0} survived the storm";
    string failBody = "No-one survived the storm";

    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI body;
    [SerializeField] CanvasGroup cg;

	private void Start()
	{
		body.text = string.Format(successBody, 100);
	}

	public void ShowEndScreen(bool wonGame, int survivors = 0)
    {
        title.text = wonGame ? successTitle : failTitle;
        body.text = wonGame ? string.Format(successBody, survivors) : failBody;

        LeanTween.alphaCanvas(cg, 1, 2f);
        cg.interactable = cg.blocksRaycasts = true;

    }
}
