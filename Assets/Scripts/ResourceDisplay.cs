using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceDisplay : MonoBehaviour
{
	TextMeshProUGUI text;
	string resourceIcon;
	private void Awake()
	{
		text = GetComponent<TextMeshProUGUI>();
		resourceIcon = text.text;
	}

	public void UpdateText(int amount)
	{
		text.text = $"{resourceIcon} {amount}";
	}
}
