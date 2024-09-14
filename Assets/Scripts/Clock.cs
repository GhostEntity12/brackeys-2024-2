using System;
using UnityEngine;

public class Clock : MonoBehaviour
{
	[SerializeField] RectTransform clockHand;

	[SerializeField] private float calmTime;
	[SerializeField] private float stormTime;

	private float currentTime;
	private float totalTime;

	private bool isStorm = false;
	private bool progressTime = true;

	public event EventHandler OnStormStart;
	public event EventHandler OnStormEnd;

	void Awake()
	{
		totalTime = calmTime + stormTime;
	}

	// Update is called once per frame
	void Update()
	{
		// Don't run if at end of timer or if paused
		if (currentTime == totalTime || !progressTime) return;

		// Increment the currentTime capped at totalTime
		currentTime = Mathf.Min(totalTime, currentTime + Time.deltaTime);

		// Storm starts
		if (!isStorm && currentTime > calmTime)
		{
			isStorm = true;
			OnStormStart?.Invoke(this, EventArgs.Empty);
		}

		// Storm ends (end of game)
		if (isStorm && currentTime == totalTime)
		{
			OnStormEnd?.Invoke(this, EventArgs.Empty);
		}

		clockHand.rotation = Quaternion.Euler(new(0, 0, currentTime / totalTime * -360));
	}
}
