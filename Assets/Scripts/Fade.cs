using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{
	private CanvasGroup fadeCG;

	private void Awake()
	{
		fadeCG = GetComponent<CanvasGroup>();
	}

	public void FadeIn(float duration = 0.3f)
	{
		LeanTween.alphaCanvas(fadeCG, 1, duration);
		fadeCG.blocksRaycasts = fadeCG.interactable = true;
	}
	public void FadeOut(float duration = 0.3f)
	{
		LeanTween.alphaCanvas(fadeCG, 0, duration);
		fadeCG.blocksRaycasts = fadeCG.interactable = false;
	}
}
